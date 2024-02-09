using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace frederik
{
    public class Game1 : Game
    {
        // Create all necessary variables
        private Texture2D backgroundTexture;
        private Texture2D logoTexture;
        private Vector2 middlePoint;
        private Vector2 logoPosition;
        private Vector2 logoScale;
        private float logoRadius;
        private SoundEffect soundHit;
        private SoundEffect soundMiss;
        private bool mouseFlipflop;
        private Vector2 mousePosition;


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "MonoGame";
        }

        protected override void Initialize()
        {
            // Initialize all variables that are not content bound
            middlePoint = new Vector2((float)_graphics.PreferredBackBufferWidth / 2, (float)_graphics.PreferredBackBufferHeight / 2);
            logoPosition = new Vector2(
                middlePoint.X + (middlePoint.X / 2) * (float)Math.Sin(0), 
                middlePoint.Y + (middlePoint.Y / 2) * (float)Math.Cos(0));
            logoScale = new Vector2(0.15f, 0.15f);
            mouseFlipflop = true;   // A flipflop to only get the first mouse press
            mousePosition = new Vector2(0, 0);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Content
            backgroundTexture = Content.Load<Texture2D>("Background");
            logoTexture = Content.Load<Texture2D>("Unilogo");
            soundHit = Content.Load<SoundEffect>("Logo_hit");
            soundMiss = Content.Load<SoundEffect>("Logo_miss");

            // Initialize all content bound variables
            logoRadius = (logoScale.X * logoTexture.Width) / 2;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Set new logoposition based on the circular functions and the total game time
            logoPosition.X = middlePoint.X + (2 * middlePoint.Y / 3) * (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / 500);
            logoPosition.Y = middlePoint.Y + (2 * middlePoint.Y / 3) * (float)Math.Cos(gameTime.TotalGameTime.TotalMilliseconds / 500);
            
            if (mouseFlipflop && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                // Trigger flipflop at first mouse press
                mouseFlipflop = false;
                mousePosition.X = Mouse.GetState().Position.X;
                mousePosition.Y = Mouse.GetState().Position.Y;

                // Calculate the length of the vector from the middle of the logo to the cursor
                // And check if it is smaller or equal to the logo radius
                if (Math.Sqrt(Math.Pow(mousePosition.X - logoPosition.X, 2) +
                              Math.Pow(mousePosition.Y - logoPosition.Y, 2)) <= logoRadius)
                {
                    soundHit.Play();
                }
                // Check if cursor is out of the Window
                else if (mousePosition is { X: >= 0, Y: >= 0 } && 
                         mousePosition.X <= _graphics.PreferredBackBufferWidth && 
                         mousePosition.Y <= _graphics.PreferredBackBufferHeight)
                {
                    soundMiss.Play();
                }
            }
            // Reset flipflop
            else if (!mouseFlipflop && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                mouseFlipflop = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // Draw background
            _spriteBatch.Draw(backgroundTexture,
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                Color.White);
            // Draw logo and set its origin to its center
            _spriteBatch.Draw(
                logoTexture,
                logoPosition,
                null,
                Color.White,
                0f,
                new Vector2((float)logoTexture.Width / 2, (float)logoTexture.Height / 2),
                logoScale, 
                SpriteEffects.None,
                0f
                );
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}