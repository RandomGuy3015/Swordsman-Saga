using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using System;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.SoundManagement;
using IUpdateable = Swordsman_Saga.Engine.ObjectManagement.IUpdateableObject;

namespace Swordsman_Saga.GameElements.GameObjects.Units;

class Tree : IMapObject
{
    public int Team { get; set; }
    public string Id { get; set; }
    public FightManager FightManager { get; set; }
    public Texture2D Texture { get; set; }
    public Rectangle TextureRectangle { get; set; }
    public Vector2 TextureOffset { get; set; }
    public Rectangle HitboxRectangle { get; set; }
    public Vector2 HitboxOffset { get; set; }
    public Vector2 Position { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public SoundManager Sound { get; set; }

    public Tree(string id, int x, int y, DynamicContentManager contentManager, FightManager fightManager)
    {
        if (id == null)
        {
            ((IGameObject)this).GenerateGuid();
        }
        else
        {
            Id = id;
        }

        Team = -1;

        FightManager = fightManager;
        MaxHealth = 200;
        Health = MaxHealth;
        Random rnd = new Random();
        int xOffset = rnd.Next(-15, 15);
        int yOffset = rnd.Next(-15, 15);
        int xScaleOffset = rnd.Next(-15, 15);
        int yScaleOffset = rnd.Next(-15, 15);
        Position = new Vector2(x, y);
        ((IGameObject)this).InitializeRectangles(185, 200, 50, 50);

        TextureOffset = new Vector2(185 / 2, 200 / 2);
        HitboxOffset = new Vector2(50 / 2, 50 / 4);
        TextureRectangle = new Rectangle((int)Position.X - (int)TextureOffset.X - 10 + xOffset, (int)Position.Y - (int)TextureOffset.Y - 55 + yOffset, 185 + xScaleOffset, 200 + yScaleOffset);

        Texture = contentManager.Load<Texture2D>("2DAssets/tree");
    }

    public Tree(string id, int x, int y, Rectangle textureRectangle, DynamicContentManager contentManager, FightManager fightManager)
    {
        if (id == null)
        {
            ((IGameObject)this).GenerateGuid();
        }
        else
        {
            Id = id;
        }

        FightManager = fightManager;
        MaxHealth = 200;
        Health = MaxHealth;
        Position = new Vector2(x, y);
        ((IGameObject)this).InitializeRectangles(185, 200, 50, 50);

        // Sorry for this horrendous code. drawing code is implemented completely wrong and i debugged the draw method of IGameObject for and hour until i realized it is never used. (deleted it now)
        // So i don't care. not gonna re-write the entire drawing code so it actually works. did that once and someone broke it. just gonna hack this in if i want to offset a texture
        TextureOffset = new Vector2(185 / 2, 200 / 2);
        HitboxOffset = new Vector2(50 / 2, 50 / 4);
        TextureRectangle = textureRectangle;

        Texture = contentManager.Load<Texture2D>("2DAssets/tree");
    }
}