using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;


namespace Swordsman_Saga.GameElements.Screens.HUDs;

class FpsCounter : Hud
{

    public long TotalFrames { get; private set; }
    public float TotalSeconds { get; private set; }
    public float AverageFramesPerSecond { get; private set; } // Use if CurrentFPS gets is too wonky
    public float CurrentFramesPerSecond { get; private set; } // Updates slowly
    public float AverageTicksPerSecond { get; private set; } // Updates slowly, because of how MonoGame works it cannot update faster
    // helper vars
    private int mElapsedTicks;
    private int mUpdatesPerAverageTick;
    public const int MaximumSamples = 30; // This controls how often to update AverageFPS and AverageTPS. 50 is ~1s, 100 is ~2s.

    private Queue<float> mSampleBuffer = new();
    private DynamicContentManager mContentManager;

    private bool mIsVisible = true;

    public override ScreenManager ScreenManager { get; set; }
    public override bool UpdateLower => false;
    public override bool DrawLower => false;

    private SpriteFont mFont; // SpriteFont for drawing text

    public FpsCounter(ScreenManager screenManager, DynamicContentManager contentManager)
    {
        ScreenManager = screenManager;

        this.mFont = contentManager.Load<SpriteFont>("basic_font");

    }
    public void ToggleVisibility()
    {
        mIsVisible = !mIsVisible;
    }
    public override void Update(GameTime gameTime, InputState inputState)
    {
        if (!mIsVisible)
        {
            return;
        }

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        CurrentFramesPerSecond = 1.0f / deltaTime;

            mSampleBuffer.Enqueue(CurrentFramesPerSecond);

        if (mSampleBuffer.Count > MaximumSamples)
        {
            mSampleBuffer.Dequeue();
            AverageFramesPerSecond = mSampleBuffer.Average(i => i);
        }
        else
        {
            AverageFramesPerSecond = CurrentFramesPerSecond;
        }

        TotalFrames++;
        TotalSeconds += deltaTime;

        mUpdatesPerAverageTick++;
        if (mUpdatesPerAverageTick > MaximumSamples)
        {
            AverageTicksPerSecond = (float)mElapsedTicks / deltaTime / MaximumSamples - 1.2f;
            mElapsedTicks = 0;
            mUpdatesPerAverageTick = 0;
        }
    }
    public override void DrawWithoutButtons(SpriteBatch spriteBatch)
    {
        Draw(spriteBatch);
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        mElapsedTicks += 1;
        if (mIsVisible && mFont != null)
        {
            string tpsText = $"FPS: {AverageFramesPerSecond:0.0}";
            string fpsText = $"TPS: {AverageTicksPerSecond:0.0}";
            spriteBatch.DrawString(mFont, fpsText, new Vector2(10, 10), Color.White); // Drawing the FPS counter at the top-left corner
            spriteBatch.DrawString(mFont, tpsText, new Vector2(10, 30), Color.White);
        }
    }
}
