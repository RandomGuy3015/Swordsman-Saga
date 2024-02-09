using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.HUDs;

namespace Swordsman_Saga.GameElements.Screens
{
    sealed class StatisticsScreen : IScreen
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
        private SpriteFont mSpriteFont;
        private StatisticsManager mStatisticsManager;
        
        // Buttons
        private enum ButtonType { Exit }

        private readonly List<Button> mButtons = new List<Button>();
        private readonly Vector2 mButtonSize;
        private readonly Texture2D mButtonTexture;
        private readonly Texture2D mButtonHoverTexture;


        // Constructor
        public StatisticsScreen(ScreenManager screenManager, DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, 
            SoundManager soundManager, InputManager inputManager, StatisticsManager statisticsManager)
        {
            mSoundManager = soundManager;
            mInputManager = inputManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mStatisticsManager = statisticsManager;
            mButtonTexture = mContentManager.Load<Texture2D>("ButtonNew");
            mButtonSize = new Vector2(100, 40);
            mButtonHoverTexture = mContentManager.Load<Texture2D>("ButtonNewHover");
            BackgroundTexture = mContentManager.Load<Texture2D>("SwordsmanSagaDark");

            // Load your SpriteFont
            mSpriteFont = mContentManager.Load<SpriteFont>("basic_font"); // Replace "YourSpriteFont" with the actual asset name
            InitializeButtons(); // Added this line
            
            ScreenManager = screenManager;
        }

        // Initialization Methods
        private void InitializeButtons()
        {
            int startX = 10;
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10;
            int buttonX = startX + ((int)ButtonType.Exit * ((int)mButtonSize.X + 10));
            Button exitButton = new Button(mButtonHoverTexture, mButtonTexture, new Vector2(buttonX, startY), mButtonSize, mContentManager, mInputManager, mSoundManager, ButtonType.Exit.ToString());
            mButtons.Add(exitButton);
            exitButton.Clicked += () => HandleButtonClick(ButtonType.Exit);
        }

        // Update and Draw Methods
        public void Update(GameTime gameTime, InputState inputState)
        {
            // Check for the "Exit" action
            if (mInputManager.IsActionInputted(inputState, ActionType.Exit))
            {
                // Handle the "Exit" action
                HandleButtonClick(ButtonType.Exit);
            }

            // Update other components
            UpdateButtons(inputState);
            UpdateStatistics();
            AdjustPositions();
        }
        private void AdjustPositions()
        {

            // Update the button positions
            int startX = 10;
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10;

            // Assuming Exit button is always the first button in mButtons
            if (mButtons.Count > 0)
            {
                Button exitButton = mButtons[0];
                int buttonX = startX + ((int)ButtonType.Exit * ((int)mButtonSize.X + 10));
                exitButton.ChangePosition(buttonX, startY);
            }
        }
        private void UpdateStatistics()
        {
            // TODO: Update the statistics values here
        }
        private void UpdateButtons(InputState inputState)
        {
            foreach (var button in mButtons)
            {
                button.Update(inputState);
            }
        }

        private void HandleButtonClick(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.Exit:
                    Exit();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, mGraphicsDeviceManager.PreferredBackBufferWidth, mGraphicsDeviceManager.PreferredBackBufferHeight), Color.White);

            DrawStatistics(spriteBatch);
            DrawButtons(spriteBatch);
            spriteBatch.End();
        }
        public void DrawWithoutButtons(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, mGraphicsDeviceManager.PreferredBackBufferWidth, mGraphicsDeviceManager.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();
        }

        private void DrawStatistics(SpriteBatch spriteBatch)
        {
            // Box dimensions and position
            int boxWidth = 470; 
            int boxHeight = 450; 
            Vector2 boxCenter = new Vector2(mGraphicsDeviceManager.PreferredBackBufferWidth / 2, mGraphicsDeviceManager.PreferredBackBufferHeight / 2);
            Vector2 boxTopLeft = new Vector2(boxCenter.X - boxWidth / 2, boxCenter.Y - boxHeight / 2);

            // Draw the box
            Texture2D boxTexture = mContentManager.Load<Texture2D>("ExtraBackground"); 
            spriteBatch.Draw(boxTexture, new Rectangle((int)boxTopLeft.X, (int)boxTopLeft.Y, boxWidth, boxHeight), Color.White);

            // Starting position for the statistics text inside the box
            Vector2 statisticStartPosition = new Vector2(boxTopLeft.X + 20, boxTopLeft.Y + 40); 

            foreach (KeyValuePair<string, int> statistic in mStatisticsManager.GetStatistics())
            {
                // Draw statistic name
                spriteBatch.DrawString(mSpriteFont, $"{statistic.Key}: {statistic.Value}", statisticStartPosition, Color.White);

                // Increment the Y position for the next statistic
                statisticStartPosition.Y += 30; // Adjust the spacing
            }
        }

        private void DrawButtons(SpriteBatch spriteBatch)
        {
            foreach (var button in mButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        private void Exit()
        {
            ScreenManager.RemoveScreen();
        }
    }
}
