using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.SoundManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using TiledCS;// dotnet add package tiledcs
using System.Reflection.Metadata;

namespace Swordsman_Saga.GameElements.Screens
{
    class Map
    {
        public ScreenManager ScreenManager { get; }
        private DynamicContentManager mContentManager;
        private InputManager mInputManager;
        private SoundManager mSoundManager;
        private GraphicsDeviceManager mGraphicsDeviceManager;

        public TiledMap mMap;
        public Dictionary<int, TiledTileset> mTilesets;
        public List<Texture2D> mTilesetTextures;
        public Texture2D mTexture;
        public TiledLayer mCollisionLayer;
        public Vector2 Size { get; private set; }


        public Map(ScreenManager screenManager, DynamicContentManager contentManager, InputManager inputManager, SoundManager soundManager, GraphicsDeviceManager graphicsDeviceManager)
        {
            ScreenManager = screenManager;
            mContentManager = contentManager;
            mInputManager = inputManager;
            mSoundManager = soundManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            Initialize();
        }
        private void Initialize()
        {
            // Set the "Copy to Output Directory" property of these two files to `Copy if newer`
            // by clicking them in the solution explorer.
            // Load map
            // RootDirectory doesn't work with the ContentManager

            Size = new Vector2(1000, 1000);

            mMap = new TiledMap("Content/swordsmen_map.tmx");
            // Load tilesets
            mTilesets = mMap.GetTiledTilesets("Content" + "/"); // DO NOT forget the / at the end

            mCollisionLayer = mMap.Layers.First(l => l.name == "Resources");

            mTilesetTextures = new List<Texture2D>();

            // Not the best way to do this but it works. It looks for "exampleTileset.xnb" file
            // which is the result of building the image file with "Content.mgcb".
            mTilesetTextures.Add(mContentManager.Load<Texture2D>("Floor"));

            /*
            mTilesetTextures.Add(mContentManager.Load<Texture2D>("pine-half04"));

            mTilesetTextures.Add(mContentManager.Load<Texture2D>("pine-half05"));

            mTilesetTextures.Add(mContentManager.Load<Texture2D>("pine-none03"));

            mTilesetTextures.Add(mContentManager.Load<Texture2D>("pine-none06"));

            mTilesetTextures.Add(mContentManager.Load<Texture2D>("STONE1"));

            mTilesetTextures.Add(mContentManager.Load<Texture2D>("TOWNHALL"));
            */
            // add sound effects to the soundmanager
            // mSoundManager.AddSound("clickTreeTile", "Lumber-Camp-Sound", 1);
            // mSoundManager.AddSound("clickStoneTile", "Mining-Camp-Sound", 1);
        }

        // Dont remove this function
        public void Update(GameTime gameTime, InputState inputState, Camera camera)
        {
            // Get mouse position on screen
            Vector2 screenMousePosition = inputState.mMousePosition;
            Vector2 worldMousePosition = camera.ScreenToWorld(screenMousePosition);

            if (Debugger.IsAttached)
            {
                foreach (var action in inputState.mInputs)
                {
                    Console.WriteLine(action);
                }
            }

            var tileLayers = mMap.Layers.Where(x => x.type == TiledLayerType.TileLayer);
            Size = new Vector2(tileLayers.ElementAt(0).height * mMap.Width, tileLayers.ElementAt(0).height * mMap.Width);

            // Check if mouse is in the bounds of a Tiled object
            if (mInputManager.IsActionInputted(inputState, ActionType.PlayTileSound))
            {
                // Search for the object that has been clicked
                foreach (var obj in mCollisionLayer.objects)
                {

                    var x = obj.x / mMap.TileHeight;
                    var y = obj.y / mMap.TileHeight;

                    var tileX = x * mMap.TileWidth;
                    var tileY = y * mMap.TileHeight;
                    // Convert cartesian coordinates to isometric coordinates and 
                    // account for Monogame drawing at top left position
                    var isoX = ((tileX - tileY) / 1) - ((mMap.TileWidth / 2) * x);
                    var isoY = ((tileY + tileX) / 2) - ((mMap.TileHeight / 2) * x);

                    var objRect = new Rectangle((int)isoX, (int)isoY, (int)obj.width * 2, (int)obj.height);

                    // Play Sound effect when Wood or Stone is clicked
                    if (objRect.Contains(worldMousePosition) && obj.type == "WOOD")
                    {
                        mSoundManager.PlaySound("clickTreeTile", .5f, false);
                    }
                    if (objRect.Contains(worldMousePosition) && obj.type == "STONE")
                    {
                        mSoundManager.PlaySound("clickStoneTile", .5f, false);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var tileLayers = mMap.Layers.Where(x => x.type == TiledLayerType.TileLayer);

            for (var y = 0; y < 42; y++)
            {
                for (var x = 0; x < 42; x++)
                {
                    var index = (y * tileLayers.ElementAt(0).width) + x; // Assuming the default render order is used which is from right to bottom
                    var gid = tileLayers.ElementAt(0).data[index]; // The tileset tile index

                    // Gid 0 is used to tell there is no tile set
                    if (gid == 0)
                    {
                        continue;
                    }

                    // Helper method to fetch the right TieldMapTileset instance
                    // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                    var mapTileset = mMap.GetTiledMapTileset(gid);

                    // Retrieve the actual tileset based on the firstgid property of the connection aobject we retrieved just now
                    var tileset = mTilesets[mapTileset.firstgid];

                    // Use the connection object as well as the tileset to figure out the source rectangle
                    var rect = mMap.GetSourceRect(mapTileset, tileset, gid);

                    var tileX = x * mMap.TileWidth;
                    var tileY = y * mMap.TileHeight;

                    // Convert cartesian coordinates to isometric coordinates and 
                    // account for Monogame drawing at top left position
                    var isoX = ((tileX - tileY) / 1) - ((mMap.TileWidth / 2) * x) - 0.5 * mMap.TileWidth;
                    var isoY = ((tileY + tileX) / 2) - ((mMap.TileHeight / 2) * x);

                    // For some reason trees and stones are one tile too low
                    // this corrects it
                    // if (layer.name == "Top Layer")
                    //{
                    //    isoY -= mMap.TileHeight;
                    //}

                    // Create source and destination rectangles
                    // (Where to take texture from and where to draw texture)
                    var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                    var destination = new Rectangle((int) isoX, isoY, tileset.TileWidth, tileset.TileHeight);

                    // match the texture with the current tile
                    // Floor
                    if (gid == 13)
                    {
                        mTexture = mTilesetTextures[0];
                    }
                    // Draw each tile using the rectangles
                    spriteBatch.Draw(mTexture, destination, source, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                }
            }
        }
    }
}