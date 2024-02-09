using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SettingsManagement;
using Swordsman_Saga.Engine.SoundManagement;

namespace Swordsman_Saga.GameElements.Screens
{
    class WinLoseScreen : MenuBaseScreen<WinLoseScreen.ButtonNumeration>
    {
        protected Texture2D VictoryTexture { get; private set; }
        protected Texture2D DefeatTexture { get; private set; }

        private SpriteFont mSpriteFont;
        private bool mWin;
        private bool soundPlayed;
        public enum ButtonNumeration
        {
            NewGame,
            Exit
        }

        public WinLoseScreen(ScreenManager screenManager,
            DynamicContentManager contentManager,
            GraphicsDeviceManager graphicsDeviceManager,
            SoundManager soundManager,
            InputManager inputManager,
            bool win) : base(screenManager,
            contentManager,
            graphicsDeviceManager,
            soundManager,
            inputManager)
        {

            VictoryTexture = mContentManager.Load<Texture2D>("Victory");
            DefeatTexture = mContentManager.Load<Texture2D>("Defeat");
            ShouldDrawBackground = false;
            mWin = win;
            soundPlayed = false;
        }
        public override void Update(GameTime gameTime, InputState inputState)
        {
            base.Update(gameTime, inputState);

            // Play sound once after initialization
            if (!soundPlayed)
            {
                if (mWin)
                {
                    mSoundManager.PlaySound("SoundAssets/Win_sound", 1, false, false);
                }
                else
                {
                    mSoundManager.PlaySound("SoundAssets/Losing_Sound", 1, false, false);
                }
                soundPlayed = true;
            }
        }
        protected override void Initialize()
        {
            UpdateLower = false;
            DrawLower = false;
            mSpriteFont = mContentManager.Load<SpriteFont>("Fonts/Arial32");
            base.Initialize();
            // TODO: Background Picture 

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Begin();
            spriteBatch.Draw(mWin ? VictoryTexture : DefeatTexture, new Rectangle(0, 0, mGraphicsDeviceManager.PreferredBackBufferWidth, mGraphicsDeviceManager.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
            base.Draw(spriteBatch);

        }
        public void DrawWithoutButtons(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.End();
        }

        protected override Action GetButtonClickHandler(ButtonNumeration buttonEnum)
        {
            switch (buttonEnum)
            {
                case ButtonNumeration.Exit:
                    return Exit;
                case ButtonNumeration.NewGame:
                    return NewGame;
                default:
                    return null;
            }
        }

        private void NewGame()
        {
            ScreenManager.ResetToFirst();
            ScreenManager.AddScreen<WorldScreen>(true, false, false, false, -1);
        }

        private void Exit()
        {
            ScreenManager.ResetToFirst();
        }
    }
}
