using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Hausaufgabe
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private Texture2D mBackground;
        private Texture2D mLogo;
        private float mAngle = 0;
        private SoundEffect mLogoHitSound;
        private SoundEffect mLogoMissSound;
        private SoundEffectInstance mLogoHitSoundInstance;
        private SoundEffectInstance mLogoMissSoundInstance;
        private Vector2 mLogoPosition; 


        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            mLogoPosition = new Vector2();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mBackground = Content.Load<Texture2D>("Background");
            mLogo = Content.Load<Texture2D>("UniLogo");
            mLogoHitSound = Content.Load<SoundEffect>("Logo_hit");
            mLogoHitSoundInstance = mLogoHitSound.CreateInstance();
            mLogoHitSoundInstance.IsLooped = false;

            mLogoMissSound = Content.Load<SoundEffect>("Logo_miss");
            mLogoMissSoundInstance = mLogoMissSound.CreateInstance();
            mLogoHitSoundInstance.IsLooped = false;

            mGraphics.PreferredBackBufferWidth = mBackground.Width;
            mGraphics.PreferredBackBufferHeight = mBackground.Height;
            mGraphics.ApplyChanges();

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && mLogoHitSoundInstance.State == SoundState.Stopped && mLogoMissSoundInstance.State == SoundState.Stopped && this.IsActive && GraphicsDevice.Viewport.Bounds.Contains(Mouse.GetState().Position))
            {
                var logoPosition = new Vector2(CalculateXPosition(mAngle), CalculateYPosition(mAngle));
                if (IsOverLogo(logoPosition))
                {
                    mLogoHitSoundInstance.Play();
                }
                else
                {
                    mLogoMissSoundInstance.Play();
                }
            }

            if (Math.Abs(mAngle - 360.0) > 0.00001)
            {
                mAngle += 0.02f;
            }
            else
            {
                mAngle = 0;
            }
            mLogoPosition.X = CalculateXPosition(mAngle);
            mLogoPosition.Y = CalculateYPosition(mAngle);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            mSpriteBatch.Begin();
            var sourceRectangle = new Rectangle(0, 0, mLogo.Width, mLogo.Height);
            mSpriteBatch.Draw(mBackground, new Rectangle(0, 0, mBackground.Width, mBackground.Height), Color.White);
            mSpriteBatch.Draw(mLogo, mLogoPosition, sourceRectangle, Color.White);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }

        private float CalculateXPosition(float angle)
        {
            var xValue = (float)Math.Cos(angle) * mLogo.Width / 2f + mBackground.Width / 2f - mLogo.Width / 2f;
            return xValue;
        }

        private float CalculateYPosition(float angle)
        {
            var yValue = (float)Math.Sin(angle) * mLogo.Height / 2f + mBackground.Height / 2f - mLogo.Height / 2f;
            return yValue;
        }

        private bool IsOverLogo(Vector2 logoPosition)
        {
            return (0 < Mouse.GetState().X - logoPosition.X && Mouse.GetState().X - logoPosition.X < mLogo.Width &&
                    0 < Mouse.GetState().Y - logoPosition.Y && Mouse.GetState().Y - logoPosition.Y < mLogo.Height);
        }
    }
}