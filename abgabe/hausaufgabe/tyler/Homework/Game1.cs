using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;
using System.Drawing;
using Microsoft.Xna.Framework.Audio;
using Color = Microsoft.Xna.Framework.Color;

namespace Homework
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D uniLogo;
        Texture2D background;

        SoundEffect hit;
        SoundEffect miss;

        MouseState mouseState;

        float uniLogoRadians;
        Vector2 uniLogoPos;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 1024;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            uniLogoRadians = 0f;
            uniLogoPos = new Vector2(0, 0);

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            uniLogo = Content.Load<Texture2D>("Unilogo");
            background = Content.Load<Texture2D>("Background");
            hit = Content.Load<SoundEffect>("Logo_hit");
            miss = Content.Load<SoundEffect>("Logo_miss");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            uniLogoRadians += 0.01f;

            uniLogoPos = new Vector2((float)((float) GraphicsDevice.Viewport.Bounds.Width / 2 + 200 * System.Math.Cos(uniLogoRadians)), (float)((float)GraphicsDevice.Viewport.Bounds.Height / 2 + 200 * System.Math.Sin(uniLogoRadians)));

            mouseState = Mouse.GetState();


            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (mouseState.LeftButton == ButtonState.Pressed)  {
                if ((uniLogoPos - mousePosition).Length() < uniLogo.Width/2 * 0.3f)
                {
                    hit.Play();
                }
                else
                {
                    miss.Play();
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();
            _spriteBatch.Draw(background, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0),
                new Vector2(1f, 1f), SpriteEffects.None, 1);
            _spriteBatch.Draw(uniLogo, new Vector2(uniLogoPos.X, uniLogoPos.Y), null, Color.White, 0f, new Vector2(uniLogo.Width/2, uniLogo.Height/2),
                new Vector2(0.3f, 0.3f), SpriteEffects.None, 1);
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}