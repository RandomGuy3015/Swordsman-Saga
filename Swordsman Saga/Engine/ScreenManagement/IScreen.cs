using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Swordsman_Saga.Engine.InputManagement;


namespace Swordsman_Saga.Engine.ScreenManagement
{
    interface IScreen
    {
        ScreenManager ScreenManager { get; }
        bool UpdateLower { get; }
        bool DrawLower { get; }
        void Update(GameTime gameTime, InputState inputState);
        void Draw(SpriteBatch spriteBatch);
        void DrawWithoutButtons(SpriteBatch spriteBatch);

    }
}
