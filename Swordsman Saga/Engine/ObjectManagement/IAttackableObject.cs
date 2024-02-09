using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Swordsman_Saga.Engine.FightManagement;

namespace Swordsman_Saga.Engine.ObjectManagement;
interface IAttackableObject : IGameObject
{
    /*
    Texture2D TextureIsBeingHit { get; set; }
    Texture2D TextureIsBeingAttacked { get; set; }
    //bool IsBeingAttacked { get; set; }
    //bool IsBeingHit { get; set; }
    */
    FightManager FightManager { get; set; }
    int MaxHealth { get; set; }
    int Health { get; set; }


    void GetDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {            
            FightManager.RemoveTarget(this);
            Health = 0;
            Die();
        }

    }

    void Die();

    void DrawHealthBar(SpriteBatch spriteBatch)
    {
        // note: anfällig für bugs, lieber checkup rein machen mit if health < 0: health -> 0
        if (Health < MaxHealth)
        {
            float percentage = (float)Health / (float)MaxHealth;
            spriteBatch.DrawRectangle(HitboxRectangle.X, TextureRectangle.Y, HitboxRectangle.Width * percentage, 4f, Color.Green, thickness: 2f);
            spriteBatch.DrawRectangle(HitboxRectangle.X + (HitboxRectangle.Width * percentage), TextureRectangle.Y, HitboxRectangle.Width * (1f-percentage), 4f, Color.Red, thickness: 2f);
        }

        long time = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 10;
        if (Health < MaxHealth && time % 1500 < 50)
        {
            Health++;
        }

    }

}
