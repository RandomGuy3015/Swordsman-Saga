using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DataTypes.Grids;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.GameObjects.Units;

namespace Swordsman_Saga.Engine.ObjectManagement;

interface IUnit : IMovingObject, ISelectableObject, IUpdateableObject, IAttackableObject, IAttackerObject
{
    int AttackRange { get; set; }
    bool IsDead { get; set; }
    int Damage { get; set; }
    //bool IsAttacking { get; set; }
    int Action { get; set; }
    bool Attacked { get; set; }
    bool FlipTexture { get; set; }
    StatisticsManager StatisticsManager { get; set; }

    enum ActionId
    {
        Standing,
        Moving,
        Dying,
        Attacking,
    }
    
    void IUpdateableObject.Update(GameTime gameTime)
    {
        if(IsDead) {return;}
        if (Health == 0)
        {
            Die();
        }


        if (Action == (int)ActionId.Dying)
        {
            Sound.PlaySound("SoundAssets/Unit_death_sound", 1, false, false);
            UpdateAnimation(gameTime.ElapsedGameTime.Milliseconds);
            if (CurrentFrame >= 4)
            {
                
                IsDead = true;
                if (Team == 1)
                {
                    StatisticsManager.EnemyUnitDefeated();
                }
                else
                {
                    StatisticsManager.UnitLost();
                }
                return;
            }    
            return;
        }
        if (IsMoving)
        {
            Action = (int)ActionId.Moving;
            if (Destination.X < Position.X) { FlipTexture = true; } // object is moving left
            else { FlipTexture =false; } 
        }
        // case Unit isn't Moving, but was moving before
        else if (!IsMoving && Action == (int)ActionId.Moving)
        {
            Action = (int)ActionId.Standing;
        }


        if (Action == (int)ActionId.Attacking && CurrentFrame == 3 && !Attacked)
        {
            DoAttack();
            Attacked = true;
        }
        // Go back to doing nothing, such that FightManager can start a new attack
        if (Action == (int)ActionId.Attacking && CurrentFrame == 4) {Action = (int)ActionId.Standing;}
        // Do Worker Update
        if (this is Worker worker)
        {
            worker.Update();
        }
        UpdateAnimation(gameTime.ElapsedGameTime.Milliseconds);
        
    }

    void IDrawableObject.Draw(SpriteBatch spriteBatch, DiamondGrid grid, bool showHitbox, bool showTextureRectangle)
    {
        SpriteEffects spriteEffects = SpriteEffects.None;
        if (FlipTexture)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }
        
        Rectangle sourceRectangle = new Rectangle(FrameWidth * CurrentFrame, FrameHeight * Action, FrameWidth, FrameHeight);
        
        var textureToDraw = Texture;
        if (IsSelected)
        {
            textureToDraw = TextureIsSelected;
        }


        spriteBatch.Draw(textureToDraw, new Vector2(TextureRectangle.X, TextureRectangle.Y), sourceRectangle, Color.White, 0f, new Vector2(0f, 0f), new Vector2(TextureRectangle.Width / (float)sourceRectangle.Width, TextureRectangle.Width / (float)sourceRectangle.Height), spriteEffects, 0f);
        

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
    private void UpdateAnimation(int elapsedTimeMilliseconds) {
        TimePassedSinceLastFrame += elapsedTimeMilliseconds;

        if (this is IRanged && (int)ActionId.Attacking == Action)
        {
            if (TimePassedSinceLastFrame > FrameChangeTime * 3)
            {
                CurrentFrame++;
                TimePassedSinceLastFrame = 0;
            }
            if (CurrentFrame >= TotalFrames)
            {
                CurrentFrame = 0;
            }
        }
        else
        {
            if (TimePassedSinceLastFrame > FrameChangeTime)
            {
                CurrentFrame++;
                TimePassedSinceLastFrame = 0;
            }
            if (CurrentFrame >= TotalFrames)
            {
                CurrentFrame = 0;
            }
        }
    }

    public bool IsAttacking()
    {
        if (Action == (int)ActionId.Attacking) {return true;}
        return false;
    }

    public bool IsDying()
    {
        if (Action == (int)ActionId.Dying) {return true;}
        return false;
    }

    public void StartAttack(Vector2 direction)
    {
        if (direction.X < 0)
        {
            FlipTexture = true;
        }
        else
        {
            FlipTexture = false;
        }
        
        Attacked = false;
        StopMoving();
        Action = (int)ActionId.Attacking;
        CurrentFrame = 0;
    }

    void DoAttack();
    
    
    bool IsInAttackRange(Vector2 targetLocation);
    
    void IAttackableObject.Die()
    {
        if (Action != (int)ActionId.Dying)
        {
            Action = (int)ActionId.Dying;
            CurrentFrame = 0;
            FightManager.RemoveAttacker(this);
        }
    }
}
