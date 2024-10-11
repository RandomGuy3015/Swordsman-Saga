using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using Swordsman_Saga.Engine.DynamicContentManagement;

namespace Swordsman_Saga.Engine.ScreenManagement.MenuManagement;

public class ResourceDisplay
{
    private readonly Texture2D mTexture;
    private int mAmount;
    private Rectangle mTextureRectangle;
    private SpriteFont mFont;


    public ResourceDisplay(Vector2 position, float scale, string type, int value, DynamicContentManager contentManager)
    {
        switch (type)
        {
            case ("wood"):
                mTexture = contentManager.Load<Texture2D>("2DAssets/wood");
                break;
            case ("stone"):
                mTexture = contentManager.Load<Texture2D>("2DAssets/stone");
                break;
        }

        mFont = contentManager.Load<SpriteFont>("basic_font");
        mAmount = value;
        mTextureRectangle = new Rectangle((int)position.X, (int)position.Y, (int)(25 * scale), (int)(25 * scale));
    }

    public void ChangeAmount(int amount)
    {
        if (amount < 0)
        {
            return;
        }
        mAmount = amount;
    }

    public int GetAmount() { return mAmount; }

    public void UpdatePosition(Vector2 position)
    {
        mTextureRectangle.X = (int)position.X;
        mTextureRectangle.Y = (int)position.Y;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(mTexture, mTextureRectangle, Color.White);
        spriteBatch.DrawString(mFont, mAmount.ToString(), new Vector2(mTextureRectangle.X + mTextureRectangle.Width, mTextureRectangle.Y), Color.Black);
    }
}