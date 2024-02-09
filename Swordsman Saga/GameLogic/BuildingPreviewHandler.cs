using Swordsman_Saga.Engine.DataTypes.Grids;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.GameElements.Screens.HUDs;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.ObjectManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Swordsman_Saga.GameElements.GameObjects.Units;
using System.Diagnostics;

namespace Swordsman_Saga.GameLogic
{
    class BuildingPreviewHandler
    {
        private readonly InputManager mInputManager;
        private readonly DiamondGrid mGrid;
        private readonly BuildingSelectionOverlay mBuildingSelectionOverlay;
        private readonly ResourceHud mResourceHud;
        private int mBuildingSize;
        private int mJustSpawned;
        private int mState;
        private Color mColor;
        private int mBuildingType;

        public bool BuildingHasBeenPlaced { get; set; }

        public Diamond SnappedToGridDiamond { get; private set; }
        public BuildingPreviewHandler(InputManager inputManager, DiamondGrid grid, BuildingSelectionOverlay buildingSelectionOverlay, ResourceHud resourceHud)
        {
            mInputManager = inputManager;
            mGrid = grid;
            mBuildingSelectionOverlay = buildingSelectionOverlay;
            mResourceHud = resourceHud;
            BuildingHasBeenPlaced = false;
            mJustSpawned = 10;
        }

        public void Update(GameTime gameTime, Vector2 worldMousePosition, InputState inputState)
        {
            if (mBuildingSelectionOverlay.BuildingHasBeenSelected)
            {
                mBuildingType = mBuildingSelectionOverlay.GetBuildingType();
                mBuildingSize = mBuildingSelectionOverlay.SelectedBuildingSize;
                SnappedToGridDiamond = mGrid.GetGridDiamondFromPixel(worldMousePosition);

                bool filled = mGrid.CheckIfGridLocationIsFilledFromPixel(new Vector2(SnappedToGridDiamond.X, SnappedToGridDiamond.Y));
                IStaticObject filledBy = filled ? mGrid.GetStaticObjectFromPixel(new Vector2(SnappedToGridDiamond.X, SnappedToGridDiamond.Y)) : null;

                mState = DetermineState(filled, filledBy, mBuildingType);

                if (mJustSpawned > 0)
                {
                    mJustSpawned--;
                }
                else if (!mResourceHud.PollResources(mBuildingSelectionOverlay.GetBuildingCost(mBuildingType), 0))
                {
                    mState = 1;
                }
                else if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonHeld) && mState == 0)
                {
                    mState = 4;
                }
                else if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonReleased) && (mState == 4 || mState == 0))
                {
                    BuildingHasBeenPlaced = true;
                    mBuildingSelectionOverlay.BuildingHasBeenSelected = false;
                    mJustSpawned = 10;
                }
                mColor = DetermineColor(mState);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (mBuildingSelectionOverlay.BuildingHasBeenSelected)
            {
                SnappedToGridDiamond.Draw(spriteBatch, mColor);
            }
        }


        private static int DetermineState(bool filled, IStaticObject filledBy, int buildingType)
        {
            if (filled)
            {
                if (buildingType == 0 && filledBy is Stone || buildingType == 1 && filledBy is Tree)
                {
                    return 0;
                }
                else if (buildingType == 0 || buildingType == 1 || buildingType == 2)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            else
            {
                return (buildingType == 0 || buildingType == 1) ? 2 : 0;
            }
        }

        private static Color DetermineColor(int state)
        {
            return state switch
            {
                0 => Color.LawnGreen,
                1 => Color.Yellow,
                2 => Color.Red,
                3 => Color.HotPink,
                4 => Color.DarkGreen,
                _ => Color.HotPink, // default color
            }; ;
        }
    }
}
