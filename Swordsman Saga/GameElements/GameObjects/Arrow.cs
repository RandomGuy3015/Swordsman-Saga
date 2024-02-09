using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.SoundManagement;

namespace Swordsman_Saga.GameElements.GameObjects;

class Arrow : IGameObject, IUpdateableObject
{
    public bool Enabled { get; }
    public int UpdateOrder { get; }
    public event EventHandler<EventArgs> EnabledChanged;
    public event EventHandler<EventArgs> UpdateOrderChanged;
    
    public int Team { get; set; }
    public Vector2 Position { get; set; }
    public string Id { get; set; }
    public Texture2D Texture { get; }
    public Rectangle HitboxRectangle { get; set; }
    public Vector2 HitboxOffset { get; set; }
    public Vector2 TextureOffset { get; set; }
    public Rectangle TextureRectangle { get; set; }
    public SoundManager Sound { get; set; }
    private DiamondGrid mGrid;

    private Vector2 mDirection;
    private float mRotation;
    private int mTimeToLive = 1000;
    private float mSpeed = 0.5f;
    private int mDamage;

    public bool mRemove = false;

    public Arrow(Vector2 position, Vector2 direction, int damage, int team, DynamicContentManager contentManager, DiamondGrid grid)
    {
        /*
        if (id == null)
        {
            ((IGameObject)this).GenerateGuid();
        }
        else
        {
            Id = id;
        }
        */
        ((IGameObject)this).GenerateGuid();
        Texture = contentManager.Load<Texture2D>("button");
        Team = team;
        Position = position;
        mDirection = direction;
        mDirection.Normalize();
        mRotation = (float)Math.Atan2(direction.Y, direction.X);
        mDamage = damage;
        mGrid = grid;
        ((IGameObject)this).InitializeRectangles(10, 10, 5, 5);
        // TODO: add Rotation of the Arrow
    }


    void IUpdateableObject.Update(GameTime gameTime)
    {
        mTimeToLive -= gameTime.ElapsedGameTime.Milliseconds;
        // Movement
        Position += mDirection * gameTime.ElapsedGameTime.Milliseconds * mSpeed;
        HitboxRectangle = new Rectangle((int)Position.X - (int)HitboxOffset.X, (int)Position.Y - (int)HitboxOffset.Y, HitboxRectangle.Width, HitboxRectangle.Height);
        TextureRectangle = new Rectangle((int)Position.X - (int)TextureOffset.X, (int)Position.Y - (int)TextureOffset.Y, TextureRectangle.Width, TextureRectangle.Height);
        mGrid.UpdateGrid(this);
        // Collision
        // TODO: change to attackableObjects when IUnits are part of the grid
        var collidingObject = mGrid.GetAttackableObjectFromPixel(Position);
        if (collidingObject != null && collidingObject.Team != Team)
        {
            collidingObject.GetDamage(mDamage);
            mRemove = true;
            return;
        } 
        if (mTimeToLive < 0)
        {
            mRemove = true;
        }
    }
    public void Draw(SpriteBatch spriteBatch, DiamondGrid grid, bool showHitbox, bool showTextureRectangle)
    {
        spriteBatch.Draw(Texture, TextureRectangle, Color.White);
    }

    public void SaveData(ref GameData data)
    {
        throw new System.NotImplementedException();
    }
}