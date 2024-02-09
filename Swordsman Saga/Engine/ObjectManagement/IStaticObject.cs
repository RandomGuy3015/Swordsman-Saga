using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Swordsman_Saga.Engine.DataTypes;

namespace Swordsman_Saga.Engine.ObjectManagement;

interface IStaticObject : IGameObject, IAttackableObject
{

    void IDrawableObject.Draw(SpriteBatch spriteBatch, DiamondGrid grid, bool showHitbox, bool showTextureRectangle)
    {
        spriteBatch.Draw(Texture, HitboxRectangle, Color.White);
    }
    
    void IAttackableObject.Die()
    {
        // needs no implementation cause they just get removed from the world when their health is zero (only units need dying function cause they want to play their animation)
        // A more smooth way: give all Attackable Objects the isDead bool which gets turned true here so you dont need to switch cases in RemoveDying in the worldScreen
        return;
    }
}