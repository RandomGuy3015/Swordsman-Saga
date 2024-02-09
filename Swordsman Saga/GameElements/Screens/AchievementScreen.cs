using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
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
    sealed class AchievementScreen : IScreen
    {
        private Rectangle achievementsArea;
        private int achievementHeight = 100;

        protected Texture2D BackgroundTexture { get; private set; }
        protected Texture2D icon { get; private set; }
        private float scrollPosition = 0;
        private int achievementWidth = 500;
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
        private AchievementsManager mAchievementsManager;

        // Achievements


        // Buttons
        private enum ButtonType { Exit }

        private readonly List<Button> mButtons = new List<Button>();
        private readonly Vector2 mButtonSize;
        private readonly Texture2D mButtonTexture;
        private readonly Texture2D mButtonHoverTexture;
        private readonly Texture2D achievementsBackgroundTexture;
        private readonly Texture2D achievementBackgroundTexture;
        private readonly Texture2D scrollbarTexture;

        // Constructor
        public AchievementScreen(ScreenManager screenManager, DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager, AchievementsManager achievementsManager)
        {
            mSoundManager = soundManager;
            mInputManager = inputManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mAchievementsManager = achievementsManager;
            mButtonTexture = mContentManager.Load<Texture2D>("ButtonNew");
            mButtonSize = new Vector2(100, 40);
            mButtonHoverTexture = mContentManager.Load<Texture2D>("ButtonNewHover");
            BackgroundTexture = mContentManager.Load<Texture2D>("SwordsmanSagaDark");
            icon = mContentManager.Load<Texture2D>("ButtonNew");
            achievementsBackgroundTexture = mContentManager.Load<Texture2D>("Button");
            achievementBackgroundTexture = mContentManager.Load<Texture2D>("ButtonNew");
            scrollbarTexture = mContentManager.Load<Texture2D>("Scrollbar");


            mSpriteFont = mContentManager.Load<SpriteFont>("basic_font");
            InitializeButtons();

            ScreenManager = screenManager;

            int achievementsAreaWidth = 400; 
            int achievementsAreaHeight = 400; 
            int achievementsAreaX = (mGraphicsDeviceManager.PreferredBackBufferWidth - achievementsAreaWidth) / 2;
            int achievementsAreaY = (mGraphicsDeviceManager.PreferredBackBufferHeight - achievementsAreaHeight) / 2;
            achievementsArea = new Rectangle(achievementsAreaX, achievementsAreaY, achievementsAreaWidth, achievementsAreaHeight);
        }


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
            AdjustPositions();

            // Check for the "Exit" action
            if (mInputManager.IsActionInputted(inputState, ActionType.Exit))
            {
                // Handle the "Exit" action
                HandleButtonClick(ButtonType.Exit);
            }
            // Change in scroll wheel value
            float scrollWheelValueDelta = mInputManager.GetMouseScroll();

            // Threshold for scroll wheel sensitivity
            float scrollThreshold = 100;

            if (Math.Abs(scrollWheelValueDelta) > scrollThreshold)
            {
                if (scrollWheelValueDelta > 0) // Scrolling up
                {
                    scrollPosition -= achievementHeight;
                }
                else // Scrolling down
                {
                    scrollPosition += achievementHeight;
                }
            }

            // Calculate the total height of the achievements list
            float totalHeight = mAchievementsManager.GetAchievements().Count * achievementHeight;

            // Clamp the scrollPosition within the bounds
            scrollPosition = MathHelper.Clamp(scrollPosition, 0, Math.Max(0, totalHeight - achievementsArea.Height));

            // Update other components
            UpdateButtons(inputState);
        }
        private void AdjustPositions()
        {
            // Update the achievements area position and size
            int achievementsAreaWidth = 400;
            int achievementsAreaHeight = 400;
            int achievementsAreaX = (mGraphicsDeviceManager.PreferredBackBufferWidth - achievementsAreaWidth) / 2;
            int achievementsAreaY = (mGraphicsDeviceManager.PreferredBackBufferHeight - achievementsAreaHeight) / 2;
            achievementsArea = new Rectangle(achievementsAreaX, achievementsAreaY, achievementsAreaWidth, achievementsAreaHeight);

            // Update the button positions
            int startX = 10;
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10;

            if (mButtons.Count > 0)
            {
                Button exitButton = mButtons[0];
                int buttonX = startX + ((int)ButtonType.Exit * ((int)mButtonSize.X + 10));
                exitButton.ChangePosition(buttonX, startY);
            }
        }
        private void UpdateButtons(InputState inputState)
        {
            foreach (var button in mButtons)
            {
                button.Update(inputState);
            }
        }
        private void DrawButtons(SpriteBatch spriteBatch)
        {
            foreach (var button in mButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        // Event Handling
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
            DrawCenteredBox(spriteBatch, 470, 450, Color.Gray);
            // Draw the background for achievements area
            spriteBatch.Draw(achievementsBackgroundTexture, achievementsArea, Color.White);

            DrawAchievements(spriteBatch);
            DrawButtons(spriteBatch);
            DrawScrollbar(spriteBatch);

            spriteBatch.End();
        }
        public void DrawWithoutButtons(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, mGraphicsDeviceManager.PreferredBackBufferWidth, mGraphicsDeviceManager.PreferredBackBufferHeight), Color.White);

            spriteBatch.End();
        }
        private void DrawAchievements(SpriteBatch spriteBatch)
        {
            // Centering the achievements menu
            float startY = achievementsArea.Y - (scrollPosition % achievementHeight);
            int maxVisibleAchievements = achievementsArea.Height / achievementHeight;
            int totalAchievements = mAchievementsManager.GetAchievements().Count;
            int firstVisibleAchievementIndex = (int)(scrollPosition / achievementHeight);
            int lastVisibleAchievementIndex = Math.Min(firstVisibleAchievementIndex + maxVisibleAchievements, totalAchievements);

            for (int i = firstVisibleAchievementIndex; i < lastVisibleAchievementIndex; i++)
            {
                var achievementEntry = mAchievementsManager.GetAchievements().ElementAt(i);
                Achievement achievement = achievementEntry.Value;
                float achievementX = achievementsArea.X + 15 + (achievementsArea.Width - achievementWidth) / 2 + 25;

                Vector2 startPosition = new Vector2(achievementX, startY);
                Color achievementColor = achievement.IsCompleted ? Color.Gray : Color.White;
                spriteBatch.Draw(achievementBackgroundTexture, new Rectangle((int)startPosition.X, (int)startPosition.Y, achievementWidth - 85, achievementHeight), achievementColor);

                // Ensure achievement is within the visible area
                if (startY + 100 < 0 || startY > mGraphicsDeviceManager.PreferredBackBufferHeight)
                {
                    startY += 100;
                    continue;
                }

                // Draw the achievement icon 
                spriteBatch.Draw(icon, new Rectangle((int)startPosition.X + 20, (int)startPosition.Y + 20, 60, 60), achievementColor);


                Vector2 titlePosition = startPosition + new Vector2(100, 20);
                Vector2 descriptionPosition = startPosition + new Vector2(100, 40);
                Vector2 progressBarPosition = startPosition + new Vector2(100, 60); 
                if (achievement.IsCompleted)
                {
                    string completedText = "(Completed)";
                    Vector2 completedTextSize = mSpriteFont.MeasureString(completedText);
                    Vector2 completedTextPosition = new Vector2(titlePosition.X + mSpriteFont.MeasureString(achievement.Title).X + 5, titlePosition.Y);
                    spriteBatch.DrawString(mSpriteFont, completedText, completedTextPosition, Color.Green);
                }
                // Draw the achievement title and description
                spriteBatch.DrawString(mSpriteFont, achievement.Title, titlePosition, achievement.IsCompleted ? Color.Gray : Color.White);
                spriteBatch.DrawString(mSpriteFont, achievement.Description, descriptionPosition, achievement.IsCompleted ? Color.DarkSlateGray : Color.Gray);

                // Draw the progress bar
                DrawProgressBar(spriteBatch, progressBarPosition, achievement.Progress);

                startY += achievementHeight; // Adjust for next achievement
            }
        }
        private void DrawCenteredBox(SpriteBatch spriteBatch, int boxWidth, int boxHeight, Color boxColor)
        {
            // Calculate the center of the screen
            int screenWidth = mGraphicsDeviceManager.PreferredBackBufferWidth;
            int screenHeight = mGraphicsDeviceManager.PreferredBackBufferHeight;
            Vector2 screenCenter = new Vector2(screenWidth / 2, screenHeight / 2);

            // Calculate the top-left corner of the box so that it's centered
            Vector2 boxPosition = new Vector2(screenCenter.X - boxWidth / 2, screenCenter.Y - boxHeight / 2);

            // Draw the box
            spriteBatch.Draw(mContentManager.Load<Texture2D>("ExtraBackground"), new Rectangle((int)boxPosition.X, (int)boxPosition.Y, boxWidth, boxHeight), boxColor);
        }

        private void DrawProgressBar(SpriteBatch spriteBatch, Vector2 position, float progress)
        {
            // Draw the background of the progress bar
            var progressBarBackground = new Rectangle((int)position.X, (int)position.Y, 200, 20); 
            spriteBatch.Draw(mContentManager.Load<Texture2D>("ButtonNew"), progressBarBackground, Color.Gray);

            // Draw the progress part of the progress bar
            var progressBar = new Rectangle((int)position.X, (int)position.Y, (int)(200 * progress), 20);
            spriteBatch.Draw(mContentManager.Load<Texture2D>("Button"), progressBar, Color.Green);
        }
        private void DrawScrollbar(SpriteBatch spriteBatch)
        {
            int totalAchievements = mAchievementsManager.GetAchievements().Count;
            float totalHeight = totalAchievements * achievementHeight;

            // If all content fits within the view, no need for a scrollbar
            if (totalHeight <= achievementsArea.Height) return;

            float scrollbarHeight = Math.Max(20, (achievementsArea.Height / totalHeight) * achievementsArea.Height);
            float scrollbarPosition = (scrollPosition / (totalHeight - achievementsArea.Height)) * (achievementsArea.Height - scrollbarHeight) + achievementsArea.Y;

            // Clamp the scrollbar position to ensure it stays within the achievements area
            scrollbarPosition = MathHelper.Clamp(scrollbarPosition, achievementsArea.Y + 5, achievementsArea.Y + achievementsArea.Height - scrollbarHeight - 5);

            // Border dimensions
            int borderWidth = 4; // Width of the border
            int scrollbarWidth = 10; // Width of the scrollbar
            Rectangle borderRectangle = new Rectangle(achievementsArea.Right + 12 - borderWidth, achievementsArea.Y, scrollbarWidth + borderWidth * 2, achievementsArea.Height);

            // Draw the border
            Texture2D pixelTexture = mContentManager.Load<Texture2D>("Scroll"); // Use a single-pixel texture
            spriteBatch.Draw(pixelTexture, borderRectangle, Color.White);

            // Draw the scrollbar
            spriteBatch.Draw(scrollbarTexture, new Rectangle(achievementsArea.Right + 12, (int)scrollbarPosition, scrollbarWidth, (int)scrollbarHeight), Color.White);
        }





        private void Exit()
        {
            ScreenManager.RemoveScreen();
        }
    }
}
