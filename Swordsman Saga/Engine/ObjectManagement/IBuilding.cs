using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DataTypes.Grids;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.GameObjects.Buildings;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.GameElements.Screens.HUDs;
namespace Swordsman_Saga.Engine.ObjectManagement;

interface IBuilding: IStaticObject, ICollidableObject, ISelectableObject, IUpdateableObject
{
    int CompletionTimer { get; set; }
    Texture2D TexturePreview { get; set; }
    Texture2D TexturePreviewSelected { get; set; }

    Texture2D TextureBeingBuilt { get; set; }
    Texture2D TextureBeingBuiltSelected { get; set; }

    ResourceHud ResourceHud { get; set; }


    int BuildState { get; set; }


    void IDataPersistence.SaveData(ref GameData data)
    {
        if (data.mHealth.ContainsKey(Id))
        {
            data.mHealth.Remove(Id);
        }
        if (data.mObjectPosition.ContainsKey(Id))
        {
            data.mObjectPosition.Remove(Id);
        }

        if (data.mGameObjects.ContainsKey(Id))
        {
            data.mGameObjects.Remove(Id);
        }
        if (data.mUnitPlayer.ContainsKey(Id))
        {
            data.mUnitPlayer.Remove(Id);
        }

        if (data.mCompletionTimers.ContainsKey(Id))
        {
            data.mCompletionTimers.Remove(Id);
        }

        if (data.mBuildStates.ContainsKey(Id))
        {
            data.mBuildStates.Remove(Id);
        }

        data.mBuildStates.Add(Id, BuildState);
        data.mCompletionTimers.Add(Id, CompletionTimer);
        data.mUnitPlayer.Add(Id, Team);
        data.mGameObjects.Add(Id, TypeToString());
        data.mHealth.Add(Id, Health);
        data.mObjectPosition.Add(Id, Position);

    }

    void IUpdateableObject.Update(GameTime gameTime)
    {
        if (this is Quarry quarry)
        {
            quarry.Update(gameTime);
        }
        else if (this is LumberCamp lumberCamp)
        {
            lumberCamp.Update(gameTime);
        }
        else if (this is Barracks barracks)
        {
            barracks.Update(gameTime);
        }
        else if (this is TownHall townHall)
        {
            townHall.Update(gameTime);
        }
    }

    void IDrawableObject.Draw(SpriteBatch spriteBatch, DiamondGrid grid, bool showHitbox, bool showTextureRectangle)
    {

        //spriteBatch.Draw(Texture, new Rectangle(TextureRectangle.X - (int)HitboxOffset.X, TextureRectangle.Y - (int)HitboxOffset.Y, TextureRectangle.Width, TextureRectangle.Height), sourceRectangle, Color.Green, 0f, Vector2.Zero, spriteEffects, 0f);
        if (BuildState == 0)
        {
            spriteBatch.Draw(IsSelected ? TexturePreviewSelected : TexturePreview, new Vector2(TextureRectangle.X, TextureRectangle.Y), null, Color.White, 0f, Vector2.Zero, new Vector2(TextureRectangle.Width / (float)Texture.Width, TextureRectangle.Height / (float)Texture.Height), SpriteEffects.None, 0f);
        }
        else if (BuildState == 1)
        {
            spriteBatch.Draw(IsSelected ? TextureBeingBuiltSelected : TextureBeingBuilt, new Vector2(TextureRectangle.X, TextureRectangle.Y), null, Color.White, 0f, Vector2.Zero, new Vector2(TextureRectangle.Width / (float)Texture.Width, TextureRectangle.Height / (float)Texture.Height), SpriteEffects.None, 0f);
        }
        else if (BuildState == 2)
        {
            spriteBatch.Draw(IsSelected ? TextureIsSelected : Texture, new Vector2(TextureRectangle.X, TextureRectangle.Y), null, Color.White, 0f, Vector2.Zero, new Vector2(TextureRectangle.Width / (float)Texture.Width, TextureRectangle.Height / (float)Texture.Height), SpriteEffects.None, 0f);

        }

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
    
        
   