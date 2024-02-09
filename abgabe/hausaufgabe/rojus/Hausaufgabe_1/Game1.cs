using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

public class Game1 : Game
{
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    // Game assets
    Texture2D uniLogo; // texture for rotating university logo
    Texture2D background; // texture for background
    SoundEffect soundA; // Sound A
    SoundEffect soundB; // Sound B

    // Game state
    Vector2 logoPosition; // current position of logo
    Rectangle logoBoundingBox; // used for collision detection

    // Movement
    float angle; // angle for the circular movement
    float circleRadius; // radius of circular path
    Vector2 circleCenter; // center of circular path

    MouseState previousMouseState; // track mouse input

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        // initialize game state variables
        logoPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
        circleCenter = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
        circleRadius = 100f; // define the radius size of the circular movement
        angle = 0f; // starting angle
        IsMouseVisible = true;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // load game content here
        uniLogo = Content.Load<Texture2D>("UniLogo"); // ensure "UniLogo.png" is in the Content.mgcb
        background = Content.Load<Texture2D>("Background"); // ensure "Background.png" is in the Content.mgcb

        soundA = Content.Load<SoundEffect>("SoundA"); // same for SoundA
        soundB = Content.Load<SoundEffect>("SoundB"); // and SoundB

        // adjust the scale of the logo
        float logoScale = 0.3f;

        // calculate the bounding box for the logo based on the scale
        logoBoundingBox = new Rectangle(
            (int)logoPosition.X - (int)(uniLogo.Width * logoScale) / 2,
            (int)logoPosition.Y - (int)(uniLogo.Height * logoScale) / 2,
            (int)(uniLogo.Width * logoScale),
            (int)(uniLogo.Height * logoScale));
    }

    protected override void Update(GameTime gameTime)
    {
        // check for exit
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // update the game state
        var currentMouseState = Mouse.GetState();

        // check for mouse click
        if (previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
        {
            // if the mouse click was within the bounding box of the logo, play sounds
            if (logoBoundingBox.Contains(currentMouseState.Position))
            {
                soundA.Play();
            }
            else
            {
                soundB.Play();
            }
        }

        // update the logo's rotation
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        angle += deltaTime; // this will increment the angle by 1 radian per second

        logoPosition.X = circleCenter.X + (float)Math.Cos(angle) * circleRadius;
        logoPosition.Y = circleCenter.Y + (float)Math.Sin(angle) * circleRadius;

        // update the bounding box position
        logoBoundingBox.Location = new Point((int)(logoPosition.X - logoBoundingBox.Width / 2), (int)(logoPosition.Y - logoBoundingBox.Height / 2));

        previousMouseState = currentMouseState; // save the current state for the next frame

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Begin();

        // draw the background image filling the entire screen
        spriteBatch.Draw(background, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

        float logoScale = 0.3f;
        spriteBatch.Draw(uniLogo, logoPosition, null, Color.White, 0, new Vector2(uniLogo.Width / 2, uniLogo.Height / 2), logoScale, SpriteEffects.None, 0f);

        spriteBatch.End();

        base.Draw(gameTime);
    }
}
