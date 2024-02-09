using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine.SettingsManagement;

public class PercentageOptionField : IOptionField
{
    private InputManager mInputManager;
    private Vector2 mPosition;
    private Rectangle mRectangle;
    private IOption mOption;
    private Texture2D mTexture;
    private Texture2D mHoverTexture;

    private string mOptionText;
    private SpriteFont mFont;
    private bool mIsSelected;
    private int mBufferedOptionValue;
    private const int OptionTextDrawPositionXOffset = 8;
    private const int OptionTextDrawPositionYOffset = 6;
    private int mOptionValueDrawPositionXOffset;
    private int mOptionValueDrawPositionYOffset;
    private Vector2 mOptionNameTextPosition;
    private Vector2 mValueTextPosition;
    private bool mIsHovering;

    public PercentageOptionField(IOption option, Texture2D hoverTexture, Texture2D texture, SpriteFont font, InputManager inputManager, int width, int height, int positionX, int positionY)
    {
        mRectangle = new Rectangle(positionX, positionY, width, height);
        mInputManager = inputManager;
        mOption = option;
        mTexture = texture;
        mHoverTexture = hoverTexture;
        mPosition = new Vector2(positionX, positionY);
        mOptionText = option.Name;
        mFont = font;
        mBufferedOptionValue = option.Value;
        mOptionValueDrawPositionXOffset = option.Name.Length * 18;
        mOptionValueDrawPositionYOffset = 6;
    }

    public void DrawTexture(SpriteBatch spriteBatch)
    {
        Texture2D textureToDraw = mIsHovering ? mHoverTexture : mTexture;
        spriteBatch.Draw(textureToDraw, mRectangle, Color.White);
    }

    public void DrawText(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(mFont, mOptionText, mOptionNameTextPosition, Color.White);
    }

    public void DrawBufferedValue(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(mFont, mBufferedOptionValue.ToString(), mValueTextPosition, Color.White);
    }

    public void ProcessInput(InputState inputState, InputManager inputManager)
    {
        if (inputManager.IsActionInputted(inputState, ActionType.PressKey0))
        {
            mBufferedOptionValue *= 10;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey1))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 1;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey2))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 2;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey3))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 3;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey4))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 4;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey5))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 5;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey6))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 6;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey7))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 7;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey8))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 8;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressKey9))
        {
            mBufferedOptionValue = mBufferedOptionValue * 10 + 9;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressBackSpaceKey))
        {
            mBufferedOptionValue = 0;
        }
        else if (inputManager.IsActionInputted(inputState, ActionType.PressEnterKey))
        {
            SetOptionValue();
            SaveOptionValue();
        }
    }

    private void SetOptionValue()
    {
        if (mBufferedOptionValue < mOption.MinValue)
        {
            mBufferedOptionValue = mOption.MinValue;
        }
        else if (mBufferedOptionValue > mOption.MaxValue)
        {
            mBufferedOptionValue = mOption.MaxValue;
        }
        mOption.SetValue(mBufferedOptionValue);
    }

    private void SaveOptionValue()
    {
        mOption.SaveOption();
    }

    public void ChangePosition(float x, float y)
    {
        mRectangle.X = (int)x; mRectangle.Y = (int)y;
        mValueTextPosition = CalculateValueTextPosition();
        mOptionNameTextPosition = CalculateOptionNameTextPosition();
    }

    public Vector2 CalculateOptionNameTextPosition()
    {
        Vector2 textSize = mFont.MeasureString(mOptionText);
        return new Vector2(
            mRectangle.X + OptionTextDrawPositionXOffset,
            mRectangle.Y + OptionTextDrawPositionYOffset
        );
    }

    public Vector2 CalculateValueTextPosition()
    {
        Vector2 textSize = mFont.MeasureString(mOptionText);
        return new Vector2(
            mRectangle.X + mOptionValueDrawPositionXOffset,
            mRectangle.Y + mOptionValueDrawPositionYOffset
        );
    }




    public void Update(InputState inputState)
    {
        Vector2 worldMousePosition = inputState.mMousePosition;
        mIsHovering = mRectangle.Contains(worldMousePosition);

        // throw event when LMB clicked while on button
        if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonClick) && mRectangle.Contains(worldMousePosition))
        {
            Select();
            mBufferedOptionValue = 0;

        }
        if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonClick) && !mRectangle.Contains(worldMousePosition))
        {
            Deselect();
            mBufferedOptionValue = mOption.Value;
        }
        if (mIsSelected)
        {
            ProcessInput(inputState, mInputManager);
        }

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawTexture(spriteBatch);
        DrawText(spriteBatch);
        DrawBufferedValue(spriteBatch);
    }

    public void Select()
    {
        mIsSelected = true;
    }

    public void Deselect()
    {
        mIsSelected = false;
    }
}
