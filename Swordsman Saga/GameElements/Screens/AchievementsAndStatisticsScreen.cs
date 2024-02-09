using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SettingsManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.Options;

namespace Swordsman_Saga.GameElements.Screens
{
    sealed class AchievementsAndStatisticsScreen : IScreen
    {
        protected Texture2D BackgroundTexture { get; private set; }

        // Properties
        public bool UpdateLower { get; private set; } = false;
        public bool DrawLower { get; private set; } = false;

        // Managers
        public ScreenManager ScreenManager { get; }
        private readonly DynamicContentManager mContentManager;
        private readonly InputManager mInputManager;
        private readonly SoundManager mSoundManager;
        private readonly GraphicsDeviceManager mGraphicsDeviceManager;

        // Buttons
        private enum ButtonType
        {
            Achievements,
            Statistics,
            Back
        }


        // Fields to enter numbers from keyboard into.

        private readonly List<Button> mButtons = new List<Button>();
        private readonly Texture2D mButtonTexture;
        private readonly Texture2D mButtonHoverTexture;
        private readonly Vector2 mButtonSize;

        // Constructor
        public AchievementsAndStatisticsScreen(ScreenManager screenManager, DynamicContentManager contentManager,
            GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager)
        {
            ScreenManager = screenManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mSoundManager = soundManager;
            mInputManager = inputManager;

            mButtonTexture = mContentManager.Load<Texture2D>("ButtonNew");
            mButtonSize = new Vector2(250, 30);
            BackgroundTexture = mContentManager.Load<Texture2D>("SwordsmanSaga");
            mButtonHoverTexture = mContentManager.Load<Texture2D>("ButtonNewHover");

            InitializeButtons();
        }

        // Initialization Methods
        private void InitializeButtons()
        {
            int startY = (mGraphicsDeviceManager.PreferredBackBufferHeight -
                          ((int)mButtonSize.Y * 3 + 20 * 2)) / 2; // Adjust spacing

            int buttonX = (mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2;

            // Create buttons
            Button achievementsButton = new Button(mButtonHoverTexture, mButtonTexture,
                new Vector2(buttonX, startY),
                mButtonSize,
                mContentManager,
                mInputManager,
                mSoundManager,
                "Achievements");
            achievementsButton.Clicked += () => HandleButtonClick(ButtonType.Achievements);

            Button statisticsButton = new Button(mButtonHoverTexture, mButtonTexture,
                new Vector2(buttonX, startY + (int)mButtonSize.Y + 20),
                mButtonSize,
                mContentManager,
                mInputManager,
                mSoundManager,
                "Statistics");
            statisticsButton.Clicked += () => HandleButtonClick(ButtonType.Statistics);

            Button backButton = new Button(mButtonHoverTexture, mButtonTexture,
                new Vector2(buttonX, startY + 2 * ((int)mButtonSize.Y + 20)),
                mButtonSize,
                mContentManager,
                mInputManager,
                mSoundManager,
                "Back");
            backButton.Clicked += () => HandleButtonClick(ButtonType.Back);

            // Add buttons to the list
            mButtons.Add(achievementsButton);
            mButtons.Add(statisticsButton);
            mButtons.Add(backButton);
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            int startY = (mGraphicsDeviceManager.PreferredBackBufferHeight - ((int)mButtonSize.Y * mButtons.Count + 20 * (mButtons.Count - 1))) / 2;
            int buttonX = (mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2;

            for (int i = 0; i < mButtons.Count; i++)
            {
                var button = mButtons[i];
                button.ChangePosition(buttonX, startY + i * ((int)mButtonSize.Y + 20));
            }
            foreach (var button in mButtons)
            {
                button?.Update(inputState);
            }
        }

        
        private void UpdateButtons(InputState inputState)
        {
            foreach (var button in mButtons)
            {
                button?.Update(inputState);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, mGraphicsDeviceManager.PreferredBackBufferWidth, mGraphicsDeviceManager.PreferredBackBufferHeight), Color.White);

            foreach (var button in mButtons)
            {
                button?.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
        public void DrawWithoutButtons(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, mGraphicsDeviceManager.PreferredBackBufferWidth, mGraphicsDeviceManager.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();
        }
        private void DrawButtons(SpriteBatch spriteBatch)
        {
            foreach (var button in mButtons)
            {
                button?.Draw(spriteBatch);
            }
        }


        // Event Handling
        private void HandleButtonClick(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.Achievements:
                    ScreenManager.AddScreen<AchievementScreen>(false, false, false, false, -1);
                    break;
                case ButtonType.Statistics:
                    ScreenManager.AddScreen<StatisticsScreen>(false, false, false, false, -1);
                    break;
                case ButtonType.Back:
                    ScreenManager.RemoveScreen();
                    break;
            }
        }



        private void Exit()
        {
            ScreenManager.RemoveScreen();
        }

        private void ChangeKeybindings()
        {
            ScreenManager.AddScreen<KeyBindingsScreen>(false, false, false, false, -1);
        }

        private void ToggleFullscreen()
        {
            // Toggle fullscreen mode
            mGraphicsDeviceManager.ToggleFullScreen();
        }
    }
}


