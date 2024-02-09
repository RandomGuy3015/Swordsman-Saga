using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine.ScreenManagement.MenuManagement
{
    abstract class Hud : IScreen
    {
        public abstract ScreenManager ScreenManager { get; set; }
        public abstract bool UpdateLower { get; }
        public abstract bool DrawLower { get; }

        public abstract void Update(GameTime gameTime, InputState inputState);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void DrawWithoutButtons(SpriteBatch spriteBatch);
    }
}
