using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.SoundManagement;

namespace Swordsman_Saga.Engine.ScreenManagement.MenuManagement;

public class Button
{
    private readonly Texture2D mTexture;
    private Texture2D mHoverTexture;
    private Vector2 mTextPosition;
    private string mText;
    private Rectangle mRectangle;
    private Color mButtonColor;
    private Color mFontColor;
    private SpriteFont mFont;
    // two states to prevent clicking every update cycle
    private bool mClickState;
    public event Action Clicked;
    private DynamicContentManager mContentManager;
    private SoundManager mSoundManager;
    private InputManager mInputManager;
    private bool mIsHovering;


    // Constructor for scaling Button to Texture size
    public Button(Texture2D hoverTexture, Texture2D texture, Vector2 position, DynamicContentManager contentManager, InputManager inputManager, SoundManager soundManager, string text = "")
    {
        mInputManager = inputManager;
        mSoundManager = soundManager;
        mTexture = texture;
        mHoverTexture = hoverTexture;
        mRectangle = new Rectangle((int)position.X, (int)position.Y, mTexture.Width, mTexture.Height);
        mButtonColor = Color.White;
        mFontColor = Color.White;
        mFont = contentManager.Load<SpriteFont>("basic_font");
        mText = text;
        mClickState = false;
        mTextPosition = CalculateTextPosition();
    }
    // overload for individual scaling
    public Button(Texture2D hoverTexture, Texture2D texture, Vector2 position, Vector2 size, DynamicContentManager contentManager, InputManager inputManager, SoundManager soundManager, string text = "")
    {
        mInputManager = inputManager;
        mSoundManager = soundManager;
        mTexture = texture;
        mHoverTexture = hoverTexture;

        mRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        mButtonColor = Color.White;
        mFontColor = Color.White;
        mFont = contentManager.Load<SpriteFont>("basic_font");
        mText = text;
        mClickState = false;
        mTextPosition = CalculateTextPosition();
    }

    // calculates the position of the text centered in the Button
    public Vector2 CalculateTextPosition()
    {
        Vector2 textSize = mFont.MeasureString(mText);
        return new Vector2(
            mRectangle.X + (mRectangle.Width - textSize.X) / 2,
            mRectangle.Y + (mRectangle.Height - textSize.Y) / 2
        );
    }

    // Change the buttons position
    public void ChangePosition(float x, float y)
    {
        mRectangle.X = (int)x; mRectangle.Y = (int)y;
        mTextPosition = CalculateTextPosition();
    }

    public void Update(InputState inputState)
    {
        Vector2 worldMousePosition = inputState.mMousePosition;

        // Update isHovering based on mouse position
        mIsHovering = mRectangle.Contains(worldMousePosition);

        // Throw event when LMB clicked while on button
        if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonClick) && mRectangle.Contains(worldMousePosition))
        {
            mSoundManager.PlaySound("SoundAssets/Click_sound", 1, false, false);
            Clicked?.Invoke();
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D textureToDraw = mIsHovering ? mHoverTexture : mTexture;
        spriteBatch.Draw(textureToDraw, mRectangle, mButtonColor);
        spriteBatch.DrawString(mFont, mText, mTextPosition, mFontColor);
    }

}