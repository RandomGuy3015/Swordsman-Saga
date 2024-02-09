using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;

namespace HausaufgabeSinan
{
    internal sealed class Game1 : Game
    {
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private MouseState mState;

        private Texture2D mUniLogoSprite;
        private Texture2D mBackgroundSprite;

        private SoundEffect mHitSound;
        private SoundEffect mMissSound;

        // Adjustments for UniLogo 
        private Vector2 mUniLogoPosition = new(640, 512);
        private readonly int mUniLogoRadius = (int)Math.Round((892 * 0.2) / 2);

        // Scales the Sprites in draw function
        private readonly Vector2 mUniLogoScale = new(.2f, .2f);
        private readonly Vector2 mBackgroundScale = new(.8f, .8f);

        // This is for circular movement of Uni Logo in Update function
        private readonly List<Vector2> mPath = CirclePath();
        private int mCurrentPathIndex = 0;

        // This is for slowing down movement of Uni Logo
        private readonly Stopwatch mTimer = new();
        private readonly TimeSpan mUpdateInterval = TimeSpan.FromSeconds(0.5);

        private bool mReleased = true;

        internal Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Set window size
            mGraphics.PreferredBackBufferWidth = 1024;
            mGraphics.PreferredBackBufferHeight = 768;
            mGraphics.ApplyChanges();


            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            // Loads the sprites
            mUniLogoSprite = Content.Load<Texture2D>("UniLogo");
            mBackgroundSprite = Content.Load<Texture2D>("Background");

            // Loads the sound effects
            mHitSound = Content.Load<SoundEffect>("LogoHit");
            mMissSound = Content.Load<SoundEffect>("LogoMiss");

        }

        protected override void Update(GameTime gameTime)
        {
            mState = Mouse.GetState();

            // Plays sound effect when left mouse button is pressed on UniLogo
            if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
            {
                var mouseUniLogoDist = Vector2.Distance(mUniLogoPosition, mState.Position.ToVector2());

                // Mouse is clicking on UniLogo
                if (mouseUniLogoDist < mUniLogoRadius)
                {   
                    // Play hit sound
                    mHitSound.Play();
                }
                else
                {
                    // Play miss sound
                    mMissSound.Play();
                }
                mReleased = false;
            }

            // Changes Released state back
            if (mState.LeftButton == ButtonState.Released)
            {
                mReleased = true;
            }

            // Moves Uni Logo around circular path
            mUniLogoPosition.X = mPath[mCurrentPathIndex].X;
            mUniLogoPosition.Y = mPath[mCurrentPathIndex].Y;

            // Slow down movement speed
            if (!mTimer.IsRunning || mTimer.Elapsed >= mUpdateInterval)
            {
                // Increment the index for the next frame
                mCurrentPathIndex = (mCurrentPathIndex + 1) % mPath.Count;

                mTimer.Restart();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Calculates List of x,y coordinates of circle
        /// with specific radius
        /// </summary>
        /// <returns>List Vector2 of x,y coordinates</returns>
        private static List<Vector2> CirclePath()
        {
            var center = new Vector2(482, 364);
            var path = new List<Vector2>();
            const int radius = 200;

            // Calculate x,y coordinates of circle
            // for every angle and store them in path
            for (var i = 0; i < 360; i++)
            {
                var xValue = (radius * (Math.Cos(i))) + center.X;
                var yValue = (radius * (Math.Sin(i))) + center.Y;
                path.Add(new Vector2((int)Math.Round(xValue), (int)Math.Round(yValue)));
            }
            return path;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            mSpriteBatch.Begin();
            // Draws background sprite in upper left corner and scales it down
            mSpriteBatch.Draw(mBackgroundSprite, new Vector2(0, 0),
                null, Color.White, 0, Vector2.Zero, mBackgroundScale, SpriteEffects.None, 0);
            // Draws UniLogo sprite at specific position and scales it down
            // Subtract radius from position for mouse click
            mSpriteBatch.Draw(mUniLogoSprite, new Vector2(mUniLogoPosition.X - mUniLogoRadius, mUniLogoPosition.Y - mUniLogoRadius),
                null, Color.White, 0, Vector2.Zero, mUniLogoScale, SpriteEffects.None, 0);

            mSpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}