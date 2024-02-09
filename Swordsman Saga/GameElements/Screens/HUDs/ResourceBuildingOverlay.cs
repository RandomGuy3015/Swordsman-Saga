using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.GameObjects.Buildings;

namespace Swordsman_Saga.GameElements.Screens.HUDs
{
    class ResourceBuildingOverlay: IHud
    {
        private RecruitementButton mUpgradeButton;
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
        private IResourceBuilding mSelectedBuilding;
        public bool mButtonClickedThisFrame = false;
        private Color mFontColor;
        private SpriteFont mFont;

        public bool ButtonClickedThisFrame => mButtonClickedThisFrame;





        public void SetSelectedBuilding(IResourceBuilding building)
        {
            mSelectedBuilding = building;
        }
        public ResourceBuildingOverlay(DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, InputManager inputManager, SoundManager soundManager, ObjectHandler objectHandler)
        {
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
            mCostVectors.Add(new Vector2(10, 10));  // Cost for Swordsman
            mCostVectors.Add(new Vector2(15, 10)); // Cost for Archer
            mCostVectors.Add(new Vector2(20, 20));// Cost for Knight
            mCostVectors.Add(new Vector2(20, 10));// Cost for Worker
        }
        public Vector2 GetUnitCost(int troopType)
        {
            return mCostVectors[troopType];
        }
        private void Initialize()
        {
            IsVisiblebtn = false;
            int startX = 10;
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10;
            int buttonX = startX + ((int)mButtonSize.X + 10);
            mUpgradeButton = new RecruitementButton(new Vector2(buttonX, startY), mButtonSize, 0, 0, mContentManager, mInputManager, "Upgrade");
            mUpgradeButton.Clicked += () => { UpgradeBuilding(); };
            startX = mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X - 10;
            startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10;
            mDemolishButton = new OtherButtons(new Vector2(startX, startY), mButtonSize, mContentManager, mInputManager, "Demolish");
            mDemolishButton.Clicked += () => { Demolish(); };
        }

        public void ToggleVisibility()
        {
            IsVisiblebtn = !IsVisiblebtn;
        }

        private void Demolish()
        {
            mObjectHandler.QueueDelete(mSelectedBuilding);
        }

        private void UpgradeBuilding()
        {
            mSelectedBuilding.UpgradeBuilding();
        }


        public void Update(GameTime gameTime, InputState inputState)
        {
            if (!IsVisiblebtn) { return; }
            mUpgradeButton.Update(inputState);
            if (mSelectedBuilding != null)
            {
                mUpgradeButton.SetWoodStoneCost(mSelectedBuilding.UpgradeCost);
            }
            mDemolishButton.Update(inputState);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisiblebtn) { return; }

            mUpgradeButton.Draw(spriteBatch);
            mDemolishButton.Draw(spriteBatch);
            spriteBatch.DrawString(mFont, mSelectedBuilding.TypeToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(mFont, "Health: " + mSelectedBuilding.Health, new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(mFont, "Level: " + mSelectedBuilding.Level, new Vector2(10, 50), Color.White);

        }


        public void UpdatePosition()
        {
            int startX = 10; // starting X position
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10; // starting Y position
            mUpgradeButton.ChangePosition(startX, startY);
            startX = mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X - 10;
            mDemolishButton.ChangePosition(startX, startY);
        }
    }
}
