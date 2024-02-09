using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.Engine.SoundManagement;
using System;

namespace Swordsman_Saga.Engine.ObjectManagement;

interface IMovingObject: ICollidableObject
{
    Vector2 Destination { get; set; }
    float Speed { get; set; }
    Vector2 MoveOffsetInGroup { get; set; }
    List<Vector2> Path { get; set; }
    HashSet<ICollidableObject> AlreadyCollidedWith { get; }

    // Animation variables
    const int FrameWidth = 64; // width of each frame in sprite sheet
    const int FrameHeight = 64; // height
    const int TotalFrames = 6; // total number of frames
    const float FrameChangeTime = 100; // time in milliseconds for each frame
    int CurrentFrame { get; set; }
    float TimePassedSinceLastFrame { get; set; }
    bool IsMoving { get; set; }
    bool IsColliding { get; set; } 
    Vector2 Goal { get; set; }
    Vector2 PathingTo { get; set; }
    bool JustStartedMoving { get; set; }
    bool IsMovingInQueue { get; set; }
    bool PreventCollision { get; set; }
    int CollisionCount { get; set; }
    DateTime LastCollisionTime { get; set; } // Property to store the time of the last collision



    public void Move(GameTime gameTime)
    {
        // Check if there is a path to follow and if the current destination has been reached
        if (Path.Count > 0 && Vector2.Distance(Position, Destination) < Speed * gameTime.ElapsedGameTime.Milliseconds)
        {
            // Update the destination to the next point in the path
            Destination = Path[0];
            Path.RemoveAt(0); // Remove the point that is now the current destination
        }

        Vector2 offset = (Destination - Position);

        if (offset.Length() > Speed * gameTime.ElapsedGameTime.Milliseconds)
        {
            offset.Normalize();
            // Update Positions
            Position += offset * Speed * gameTime.ElapsedGameTime.Milliseconds;
            HitboxRectangle = new Rectangle((int)Position.X - (int)HitboxOffset.X, (int)Position.Y - (int)HitboxOffset.Y, HitboxRectangle.Width, HitboxRectangle.Height);
            TextureRectangle = new Rectangle((int)Position.X - (int)TextureOffset.X, (int)Position.Y - (int)TextureOffset.Y, TextureRectangle.Width, TextureRectangle.Height);
            //UpdateAnimation(gameTime.ElapsedGameTime.Milliseconds);
            PlayMovementSound(Sound);
            IsMoving = true;
        }
        else
        {
            // Check if there are more points in the path to move to
            if (Path.Count > 0)
            {
                // Set the next destination
                Destination = Path[0];
                Path.RemoveAt(0);
                IsMoving = true; // Still moving towards the next point
            }
            else
            {
                IsMoving = false; // No more points in the path, set the flag to false
                HitboxRectangle = new Rectangle((int)Position.X - (int)HitboxOffset.X, (int)Position.Y - (int)HitboxOffset.Y, HitboxRectangle.Width, HitboxRectangle.Height);
                TextureRectangle = new Rectangle((int)Position.X - (int)TextureOffset.X, (int)Position.Y - (int)TextureOffset.Y, TextureRectangle.Width, TextureRectangle.Height);
            }
        }
    }

    public void StopMoving()
    {
        Path.Clear();
        IsMoving = false;
        Destination = Position;
        Goal = Position;
    }
    private void PlayMovementSound(SoundManager soundManager)
    {
        soundManager.PlaySound("SoundAssets/Walking_sound", 1 , false, false);
    }

    // Careful. This eats performance.
    public double FindLengthOfPath()
    {
        double totalLength = 0d;

        for (int i = 0; i < Path.Count - 1; i++)
        {
            totalLength += Vector2.Distance(Path[i], Path[i + 1]);
        }
        return totalLength;
    }
}


