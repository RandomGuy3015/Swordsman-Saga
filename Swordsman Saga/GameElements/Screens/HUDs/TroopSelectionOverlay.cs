using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.GameObjects.Buildings;
using static Swordsman_Saga.GameElements.Screens.HUDs.BuildingSelectionOverlay;

namespace Swordsman_Saga.GameElements.Screens.HUDs
{
    public enum TroopType
    {
        Swordsman,
        Archer,
        Knight,
        Worker
    }
    // TODO: should implement several instances, such as 2DObject.
    class TroopSelectionOverlay : IHud
    {
        public static TroopSelectionOverlay Instance { get; private set; }
        private List<RecruitementButton> mButtons;
        private OtherButtons mDemolishButton;
        private List<Vector2> mCostVectors;
        private Texture2D mButtonTexture;
        private Vector2 mButtonSize;
        public bool IsVisiblebtn { get; set; }
        private DynamicContentManager mContentManager;
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private ObjectHandler mObjectHandler;
        private InputManager mInputManager;
        private SoundManager mSoundManager;
        public TroopType SelectedTroopType { get; private set; }
        private Barracks mSelectedBarracks;
        public bool mButtonClickedThisFrame = false;
        private Color mFontColor;
        private SpriteFont mFont;

        public bool ButtonClickedThisFrame => mButtonClickedThisFrame;





        public void SetSelectedBarracks(Barracks barracks)
        {
            mSelectedBarracks = barracks;
        }
        // have to do shit weird to make the ai work with spawning unit. pls don't hardcode only for player next time!!
        public TroopSelectionOverlay(DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, InputManager inputManager, SoundManager soundManager, ObjectHandler objectHandler)
        {
            Instance = this;
            mSoundManager = soundManager;
            mInputManager = inputManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mObjectHandler = objectHandler;
            mButtonTexture = contentManager.Load<Texture2D>("Button"); // Load the texture here
            mButtonSize = new Vector2(150, 60);
            InitializeCostVectors();
            mFontColor = Color.Black;
            mFont = contentManager.Load<SpriteFont>("basic_font");
            Initialize();
        }
        private void InitializeCostVectors()
        {
            mCostVectors = new List<Vector2>();
            // Example costs, adjust as necessary
            mCostVectors.Add(new Vector2(60, 60));  // Cost for Swordsman
            mCostVectors.Add(new Vector2(80, 60)); // Cost for Archer
            mCostVectors.Add(new Vector2(200, 200));// Cost for Knight
            mCostVectors.Add(new Vector2(40, 40));// Cost for Worker
        }
        public Vector2 GetUnitCost(int troopType)
        {
            return mCostVectors[troopType];
        }
        private void Initialize()
        {
            mButtons = new List<RecruitementButton>();
            IsVisiblebtn = false;
            int startX = 10;
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10;
            foreach (TroopType troopType in Enum.GetValues(typeof(TroopType)))
            {
                int buttonX = startX + ((int)troopType * ((int)mButtonSize.X + 10));
                TroopType localTroopType = troopType; // create a local copy
                                                      // Update the Button creation to include cost
                RecruitementButton button = new RecruitementButton(new Vector2(buttonX, startY), mButtonSize, (int)mCostVectors[(int)troopType].X, (int)mCostVectors[(int)troopType].Y, mContentManager, mInputManager, troopType.ToString());
                mButtons.Add(button);
                button.Clicked += () => { AddTroopToQueue(localTroopType); };

                button.Clicked += () => { ButtonClickHandler(localTroopType); };
            }
            startX = mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X - 10;
            startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10;
            mDemolishButton = new OtherButtons(new Vector2(startX, startY), mButtonSize, mContentManager, mInputManager, "Demolish");
            mDemolishButton.Clicked += () => { Demolish(); };
        }
        private void ButtonClickHandler(TroopType troopType)
        {
            GetButtonClickHandler(troopType);

        }
        private void AddTroopToQueue(TroopType troopType)
        {
            mSelectedBarracks?.AddToQueue(troopType);
        }
        public void ToggleVisibility()
        {
            IsVisiblebtn = !IsVisiblebtn;
        }

        private void Demolish()
        {
            mObjectHandler.QueueDelete(mSelectedBarracks);
        }

        public bool IsClickWithinBounds(Vector2 clickPosition)
        {

            // determine the leftmost and rightmost X coordinates of the overlay
            int startX = 10; // starting X position
            int endX = startX + mButtons.Count * ((int)mButtonSize.X + 10) - 10;

            // determine the topmost and bottommost Y coordinates of the overlay
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10; // starting Y position
            int endY = startY + (int)mButtonSize.Y;

            // check if the click position is within the bounds
            return clickPosition.X >= startX && clickPosition.X <= endX &&
                   clickPosition.Y >= startY && clickPosition.Y <= endY;
        }
        private void GetButtonClickHandler(TroopType troopType)
        {
            // handle the button click based on troop type
            switch (troopType)
            {
                case TroopType.Swordsman:
                    SelectedTroopType = TroopType.Swordsman;
                    mSoundManager.PlaySound("SoundAssets/Click_sound", 1, false, false);
                    break;
                case TroopType.Archer:
                    SelectedTroopType = TroopType.Archer;
                    mSoundManager.PlaySound("SoundAssets/Click_sound", 1, false, false);
                    break;
                case TroopType.Knight:
                    SelectedTroopType = TroopType.Knight;
                    mSoundManager.PlaySound("SoundAssets/Click_sound", 1, false, false);
                    break;
            }
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            if (!IsVisiblebtn) { return; }
            foreach (var button in mButtons)
            {
                button.Update(inputState);
            }

            mDemolishButton.Update(inputState);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisiblebtn) { return; }

            if (mSelectedBarracks.BuildState == 2)
            {
                foreach (var button in mButtons)
                {
                    button.Draw(spriteBatch);
                }
            }

            mDemolishButton.Draw(spriteBatch);
            spriteBatch.DrawString(mFont, "Barracks", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(mFont, "Health: " + mSelectedBarracks.Health, new Vector2(10, 30), Color.White);

            // draw the training queue
            if (mSelectedBarracks != null && mSelectedBarracks.BuildState == 2)
            {
                DrawTrainingQueue(spriteBatch);
            }
        }
        private void DrawTrainingQueue(SpriteBatch spriteBatch)
        {
            const int dotSize = 20; // diameter of each dot
            const int dotSpacing = 10; // spacing between dots
            const int queueLength = 5; // total number of dots in the queue

            // calculate the total width of the button layout
            int totalButtonWidth = mButtons.Count * (int)mButtonSize.X + (mButtons.Count - 1) * 10;

            // starting position for the queue (centered above the buttons)
            int queueStartX = 10; // 10 pixels from the left side
            int queueStartY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 45; // position above the buttons

            var queue = mSelectedBarracks.GetTrainingQueue();

            // create a 1x1 texture for dots
            Texture2D dotTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            dotTexture.SetData(new[] { Color.White });

            // draw up to 5 dots
            for (int i = 0; i < queueLength; i++)
            {
                Color color = Color.Gray; // default color for empty slot
                if (queue.Count() > i)
                {
                    color = Color.Green; // color representing a troop in training
                }

                spriteBatch.Draw(
                    texture: dotTexture,
                    destinationRectangle: new Rectangle(queueStartX + (i * (dotSize + dotSpacing)), queueStartY, dotSize, dotSize),
                    color: color
                );
            }
            float remainingTrainingTime = mSelectedBarracks.GetRemainingTrainingTime();

            string timeText = $"{remainingTrainingTime:F2}s"; // format to 2 decimal places
            Vector2 timeTextPosition = new Vector2(queueStartX, queueStartY - 30); // position above the dots
            spriteBatch.DrawString(mFont, timeText, timeTextPosition, Color.White);

        }


        public void UpdatePosition()
        {
            int startX = 10; // starting X position
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10; // starting Y position
            int troopType = 0;
            foreach (var button in mButtons)
            {
                int buttonX = startX + (troopType * ((int)mButtonSize.X + 10));
                button.ChangePosition(buttonX, startY);
                troopType++;
            }
            startX = mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X - 10;
            mDemolishButton.ChangePosition(startX, startY);
        }
    }
}
