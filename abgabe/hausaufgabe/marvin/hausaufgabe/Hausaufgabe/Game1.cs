using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace sopra;

public class Game1 : Game
{
    private SpriteBatch mSpriteBatch;
    private Texture2D mLogoTexture;
    private Texture2D mBackgroundTexture;
    private Vector2 mLogoPosition;
    private float mAngle;
    private float mLogoDegreesPerTick = 0.01f;
    private int mHypothenuseLength;
    private int mAdjacentLength;
    private int mCathetusLength;
    private Vector2 mScreenCenter;
    private Vector2 mLogoCenter;
    private const int ScreenWidth = 1200;
    private const int ScreenHeight = 1024;
    private float mAlpha = 0;
    private static int sLogoWidth;
    private static int sLogoHeight;
    private MouseState mMouseState;
    private SoundEffect mHit;
    private SoundEffect mMiss;
    private int mMouseX;
    private int mMouseY;
    private bool mMouseCooldown = false;
    private Rectangle mLogoRectangle;
    
    
    

    public Game1()
    {
        GraphicsDeviceManager graphics;
        graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = ScreenWidth;
        graphics.PreferredBackBufferHeight = ScreenHeight;
        Content.RootDirectory = "Content";
    }
    private bool LogoHit()
    {
        Point mousePoint = new Point(mMouseX, mMouseY);
        return (mLogoRectangle.Contains(mousePoint));

    }

    private void UpdateMouseButton()
    {
        mMouseState = Mouse.GetState();
        mMouseX = mMouseState.X;
        mMouseY = mMouseState.Y;
    }

    private bool IsMouseClicked()
    {
        if (mMouseState.LeftButton == ButtonState.Pressed && mMouseCooldown)
        {
            mMouseCooldown = false;
            return true;
        }
        if (mMouseState.LeftButton == ButtonState.Released && !mMouseCooldown)
        {
            mMouseCooldown = true;
        }
        return false;
    }


    
    protected override void Initialize()
    {
        // set center of screen.
        mScreenCenter.X = ScreenWidth / 2;
        mScreenCenter.Y = ScreenHeight / 2;

        // set logo dimensions
        sLogoWidth = ScreenWidth / 10;
        sLogoHeight = ScreenHeight / 10;
        
        // set rotation circle dimensions.
        mAdjacentLength = ScreenWidth / 10;
        mCathetusLength = ScreenHeight / 10;
        mHypothenuseLength = (int)Math.Sqrt(
            Math.Pow(mAdjacentLength, 2) + Math.Pow(mCathetusLength, 2));
        
        // set starting position of logo.
        mLogoCenter.X = mScreenCenter.X + mHypothenuseLength;
        mLogoCenter.Y = mScreenCenter.Y;
        
        // offset logo position (addressed by top left coordinates)
        mLogoPosition.X = (int) (mLogoCenter.X - sLogoWidth / 2);
        mLogoPosition.Y = (int) (mLogoCenter.Y - sLogoHeight / 2);
        
        // add sound effects to list.
        mMiss = Content.Load<SoundEffect>("logo_hit");
        mHit = Content.Load<SoundEffect>("logo_miss");
        
        // set initial mouse position and state.
        UpdateMouseButton();
        
        // create logoRectangle
        mLogoRectangle = new Rectangle((int)mLogoPosition.X,
            (int)mLogoPosition.Y,
            sLogoWidth,
            sLogoHeight);
        
        // show the mouse courser.
        IsMouseVisible = true;
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        mSpriteBatch = new SpriteBatch(GraphicsDevice);
        mLogoTexture = this.Content.Load<Texture2D>("unilogo");
        mBackgroundTexture = this.Content.Load<Texture2D>("background");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }
        // increase angle by x degrees.
        mAlpha = mAlpha >= 360 ? 0f : mAlpha + mLogoDegreesPerTick;

        // compute new logo position using trigonometry.
        switch (mAlpha)
        {
            case <= 90:
                // logo in first quarter.
                mLogoCenter.X = mScreenCenter.X + (int)(Math.Cos(mAlpha) * mHypothenuseLength);
                mLogoCenter.Y = mScreenCenter.Y - (int)(Math.Sin(mAlpha) * mHypothenuseLength);
                break;
            case <= 180:
                // logo in second quarter.
                mLogoCenter.X = mScreenCenter.X - (int)(Math.Cos(mAlpha) * mHypothenuseLength);
                mLogoCenter.Y = mScreenCenter.Y - (int)(Math.Sin(mAlpha) * mHypothenuseLength);
                break;
            case <= 270:
                // logo in third quarter.
                mLogoCenter.X = mScreenCenter.X - (int)(Math.Cos(mAlpha) * mHypothenuseLength);
                mLogoCenter.Y = mScreenCenter.Y + (int)(Math.Sin(mAlpha) * mHypothenuseLength);
                break;
            case <= 360:
                // logo in fourth quarter.
                mLogoCenter.X = mScreenCenter.X + (int)(Math.Cos(mAlpha) * mHypothenuseLength);
                mLogoCenter.Y = mScreenCenter.Y + (int)(Math.Sin(mAlpha) * mHypothenuseLength);
                break;
        }

        // offset logo position.
        mLogoPosition.X = (int)(mLogoCenter.X - sLogoWidth / 2);
        mLogoPosition.Y = (int)(mLogoCenter.Y - sLogoHeight / 2);
        base.Update(gameTime);

        // create updated logo Rectangle.
        mLogoRectangle = new Rectangle((int)mLogoPosition.X,
            (int)mLogoPosition.Y,
            (int)sLogoWidth,
            (int)sLogoHeight);

        // get mouse state
        UpdateMouseButton();

        // play sound effects.
        var mouseClicked = IsMouseClicked();
        if (mouseClicked && LogoHit())
        {
            mHit.CreateInstance().Play();
        } else if (mouseClicked && !LogoHit())
        {
            mMiss.CreateInstance().Play();
        }

    }

    protected override void Draw(GameTime gameTime)
    {
        // reset screen
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // open a spritebatch.
        mSpriteBatch.Begin();
        
        // draw background.
        mSpriteBatch.Draw(mBackgroundTexture, new Vector2(0, 0), Color.White);
        
        // draw logo.
        mSpriteBatch.Draw(texture: mLogoTexture,
            destinationRectangle: new Rectangle(
                (int) mLogoPosition.X,
                (int) mLogoPosition.Y,
                (int) sLogoWidth,
                (int) sLogoHeight),
            color: Color.White
            );
        mSpriteBatch.End();
        base.Draw(gameTime);
    }
}
