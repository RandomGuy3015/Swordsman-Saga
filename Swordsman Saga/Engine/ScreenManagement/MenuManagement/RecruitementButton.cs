using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine.ScreenManagement.MenuManagement;

public class RecruitementButton
{
    private readonly Texture2D mTexture;
    private ResourceDisplay mWood;
    private ResourceDisplay mStone;
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
    private InputManager mInputManager;
    private readonly Texture2D mButtonHoverTexture;
    private bool mIsHovering;



    public RecruitementButton(Vector2 position, Vector2 size, int woodCost, int stoneCost, DynamicContentManager contentManager, InputManager inputManager, string text = "")
    {
        mInputManager = inputManager;
        mTexture = contentManager.Load<Texture2D>("Button");
        mButtonHoverTexture = contentManager.Load<Texture2D>("ButtonHover");

        mRectangle = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        mButtonColor = Color.White;
        mFontColor = Color.Black;
        mFont = contentManager.Load<SpriteFont>("basic_font");
        mText = text;
        mClickState = false;
        mWood = new ResourceDisplay(new Vector2(0, 0), 1f, "wood", woodCost, contentManager);
        mStone = new ResourceDisplay(new Vector2(0, 0), 1f, "stone", stoneCost, contentManager);
        CalculatePositions();
    }

    public void ChangeText(string text)
    {
        mText = text;
        CalculatePositions();
    }

    public Vector2 GetWoodStoneCost()
    {
        return new Vector2(mWood.GetAmount(), mStone.GetAmount());
    }

    public void SetWoodStoneCost(Vector2 cost)
    {
        mWood.ChangeAmount((int)cost.X);
        mStone.ChangeAmount((int)cost.Y);
    }

    // calculates the position of the button elements
    public void CalculatePositions()
    {
        Vector2 textSize = mFont.MeasureString(mText);
        mTextPosition = new Vector2(
            mRectangle.X + (mRectangle.Width - textSize.X) / 2,
            mRectangle.Y + (mRectangle.Height - textSize.Y) * 2 / 3
        );
        mWood.UpdatePosition(new Vector2(mRectangle.X + 7, mRectangle.Y + 2));
        mStone.UpdatePosition(new Vector2(mRectangle.X + mRectangle.Width / 2 + 5, mRectangle.Y + 2));
    }

    // Change the buttons position
    public void ChangePosition(float x, float y)
    {
        mRectangle.X = (int)x; mRectangle.Y = (int)y;
        CalculatePositions();
    }

    public void Update(InputState inputState)
    {
        Vector2 worldMousePosition = inputState.mMousePosition;
        mIsHovering = mRectangle.Contains(worldMousePosition);
        // throw event when LMB clicked while on button
        if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonClick) && mRectangle.Contains(worldMousePosition))
        {
            Clicked?.Invoke();
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D textureToDraw = mIsHovering ? mButtonHoverTexture : mTexture;
        spriteBatch.Draw(textureToDraw, mRectangle, Color.White);
        spriteBatch.DrawString(mFont, mText, mTextPosition, mFontColor);
        mWood.Draw(spriteBatch);
        mStone.Draw(spriteBatch);
    }
}

