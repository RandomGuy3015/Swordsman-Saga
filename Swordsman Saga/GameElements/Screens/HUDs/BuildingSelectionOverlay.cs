using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameLogic;
using static Swordsman_Saga.GameElements.Screens.HUDs.BuildingSelectionOverlay;

namespace Swordsman_Saga.GameElements.Screens.HUDs
{
    class BuildingSelectionOverlay: IHud
    {
        private List<RecruitementButton> mButtons;
        private Vector2 mButtonSize;
        private List<Vector2> mCostVectors;
        public bool IsVisiblebtn { get; set; }
        private DynamicContentManager mContentManager;
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private InputManager mInputManager;
        private SoundManager mSoundManager;

        public BuildingType SelectedBuildingType { get; private set; }
        public int SelectedBuildingSize { get; private set; }
        public bool BuildingHasBeenSelected { get; set; }



        public enum BuildingType
        {
            Quarry,
            Camp,
            Barracks,
        }
        
        public BuildingSelectionOverlay(DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, InputManager inputManager, SoundManager soundManager)
        {
            mSoundManager = soundManager;
            mInputManager = inputManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mButtonSize = new Vector2(150, 60);
            Initialize();
        }

        public Vector2 GetBuildingCost(int buildingType)
        {
                return mCostVectors[buildingType];
        }

        private void Initialize()
        {
            // TODO: 
            // dont know where to have these values, so I have them here before we implement them somewhere else
            mCostVectors = new List<Vector2>();
            mCostVectors.Add(new Vector2(120, 80));
            mCostVectors.Add(new Vector2(80, 120));
            mCostVectors.Add(new Vector2(320, 320));
            // end
            
            mButtons = new List<RecruitementButton>();
            IsVisiblebtn = false;
            int startX = 10; // starting X position
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10; // starting Y position

            foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
            {
                int buttonX = startX + ((int)buildingType * ((int)mButtonSize.X + 10));
                BuildingType localBuildingType = buildingType; // create a local copy
                RecruitementButton button = new RecruitementButton( new Vector2(buttonX, startY), mButtonSize, (int)mCostVectors[(int)buildingType].X, (int)mCostVectors[(int)buildingType].Y, mContentManager, mInputManager, buildingType.ToString());
                mButtons.Add(button);
                button.Clicked += () => { GetButtonClickHandler(localBuildingType); }; // use the local copy
            }
        }

        public void ToggleVisibility()
        {
            IsVisiblebtn = !IsVisiblebtn;
        }

        private void GetButtonClickHandler(BuildingType buildingType)
        {
            // handle the button click based on troop type
            switch (buildingType)
            {
                case BuildingType.Quarry:
                    SelectedBuildingType = BuildingType.Quarry;
                    SelectedBuildingSize = 1;
                    mSoundManager.PlaySound("SoundAssets/Click_sound", 1, false, false);
                    break;
                case BuildingType.Camp:
                    SelectedBuildingType = BuildingType.Camp;
                    SelectedBuildingSize = 1;
                    mSoundManager.PlaySound("SoundAssets/Click_sound", 1, false, false);
                    break;
                case BuildingType.Barracks:
                    SelectedBuildingType = BuildingType.Barracks;
                    SelectedBuildingSize = 1;
                    mSoundManager.PlaySound("SoundAssets/Click_sound", 1, false, false);
                    break;
            }
            BuildingHasBeenSelected = true;

        }

        public int GetBuildingType()
        {
            if (SelectedBuildingType == BuildingType.Quarry)
            {
                return 0;
            }
            else if (SelectedBuildingType == BuildingType.Camp)
            {
                return 1;
            }
            else if (SelectedBuildingType == BuildingType.Barracks)
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            if (!IsVisiblebtn) { return; }
            foreach (var button in mButtons)
            {
                button.Update(inputState);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisiblebtn) { return; }
            foreach (var button in mButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        public void UpdatePosition()
        {
            int startX = 10; // starting X position
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight - (int)mButtonSize.Y - 10; // starting Y position
            int buildingType = 0;
            foreach (var button in mButtons)
            {
                int buttonX = startX + (buildingType * ((int)mButtonSize.X + 10));
                button.ChangePosition(buttonX, startY);
                buildingType++;
            }
        }
    }
}
