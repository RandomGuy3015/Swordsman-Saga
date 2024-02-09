using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoGame.Extended;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataTypes;
using System;

namespace Swordsman_Saga.Engine.ObjectManagement;
interface IMapObject: IStaticObject, ICollidableObject
{
    void IDataPersistence.SaveData(ref GameData data)
    {
        if (data.mGameObjects.ContainsKey(Id))
        {
            data.mGameObjects.Remove(Id);
        }
        if (data.mObjectPosition.ContainsKey(Id))
        {
            data.mObjectPosition.Remove(Id);
        }
        if (data.mMapRectangles.ContainsKey(Id))
        {
            data.mMapRectangles.Remove(Id);
        }

        if (data.mHealth.ContainsKey(Id))
        {
            data.mHealth.Remove(Id);
        }

        data.mMapRectangles.Add(Id, TextureRectangle);

        data.mObjectPosition.Add(Id, Position);

        data.mGameObjects.Add(Id, TypeToString());
        data.mHealth.Add(Id, Health);
    }
    
    void IDrawableObject.Draw(SpriteBatch spriteBatch, DiamondGrid grid, bool showHitbox, bool showTextureRectangle)
    {
        //spriteBatch.Draw(Texture, new Rectangle(TextureRectangle.X - (int)HitboxOffset.X, TextureRectangle.Y - (int)HitboxOffset.Y, TextureRectangle.Width, TextureRectangle.Height), sourceRectangle, Color.Green, 0f, Vector2.Zero, spriteEffects, 0f);
        spriteBatch.Draw(Texture, new Vector2(TextureRectangle.X, TextureRectangle.Y), null, Color.White, 0f, Vector2.Zero, new Vector2(TextureRectangle.Width / (float)Texture.Width, TextureRectangle.Height / (float)Texture.Height), SpriteEffects.None, 0f);

        if (showHitbox)
        {
            spriteBatch.DrawRectangle(HitboxRectangle, Color.Blue);
        }
        if (showTextureRectangle)
        {
            spriteBatch.DrawRectangle(TextureRectangle, Color.Red);
        }
        DrawHealthBar(spriteBatch);
    }
}
    
        
   