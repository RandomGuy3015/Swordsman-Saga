using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;    
using Newtonsoft.Json;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DataTypes.Grids;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.PathfinderManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.GameObjects;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.GameElements.GameObjects.Buildings;
using Swordsman_Saga.GameElements.Screens.HUDs;
using Swordsman_Saga.GameElements.Screens.Menus;
using Swordsman_Saga.GameLogic;
using TiledCS;
using Microsoft.Xna.Framework.Content;
using Swordsman_Saga.Engine.FightManagement;

namespace Swordsman_Saga.GameElements.Screens
{
    sealed class WorldScreen : IScreen
    {
        private TimeSpan gameDuration;

        public bool UpdateLower { get; private set; } = false;
        public bool DrawLower { get; private set; } = true;
        // Managers
        public ScreenManager ScreenManager { get; }
        private DynamicContentManager mContentManager;
        private InputManager mInputManager;
        private SoundManager mSoundManager;
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private GraphicsDevice mGraphicsDevice;
        private AStarPathfinder mPathfinder;
        private KeybindingManager mKeybindingManager;
        private FightManager mFightManager;
        private ObjectHandler mObjectHandler;
        private LayerHandler mLayerHandler;
        private AIHandler mAIHandler;

        private AchievementsManager mAchievementsManager;

        private StatisticsManager mStatisticsManager;
        private Map mWorldMap;
        private DiamondGrid mGrid;

        private List<ISelectableObject> mSelectedObjects;
        private Vector2 mSelectionStart;
        private bool mDrawSelectionRectangle;
        private Rectangle mSelectionRectangle;
        private TroopSelectionOverlay mTroopSelectionOverlay;
        private ResourceBuildingOverlay mResourceBuildingOverlay;
        private BuildingSelectionOverlay mBuildingSelectionOverlay;
        private TownhallOverlay mTownhallOverlay;
        private BuildingPreviewHandler mBuildingPreviewHandler;
        private ResourceHud mResourceHud;
        private FpsCounter mFpsCounter;
        private Camera mCamera;
        private TownHall mFriendlyTownHall;
        private TownHall mEnemyTownHall;

        // Debug stuff
        private bool mShowHitbox = false;
        private bool mShowTextureRectangle = false;
        private bool mShowFogOfWar = false;
        private Vector2 mMouseLocation;

        private readonly int mMapSize = 84; // Frag nicht wieso genau 168, keine ahnung. Hat jemand in Tiled so festgelegt
        private readonly int mSeed = 8923; // Das kann alles sein

        private bool mIsSelecting = false;
        private bool mIsSelectingFightTarget = false;
        private float mCameraZoom = 1f;
        private RenderTarget2D mRenderTarget;
        private Effect mFogOfWar;

        private bool mDebugKeys = false;
        private IBuilding mSelectedBuilding = null;


        public WorldScreen(ScreenManager screenManager, DynamicContentManager contentManager, InputManager inputManager, SoundManager soundManager, 
            GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, KeybindingManager keybindingManager, 
            AchievementsManager achievementsManager, StatisticsManager statisticsManager, FightManager fightManager, bool newGame, bool techdemo, int difficulty)
        {
            gameDuration = TimeSpan.Zero;
            ScreenManager = screenManager;
            mContentManager = contentManager;
            mInputManager = inputManager;
            mSoundManager = soundManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mGraphicsDevice = graphicsDevice;
            mSoundManager = soundManager;
            mKeybindingManager = keybindingManager;
            mAchievementsManager = achievementsManager;
            mStatisticsManager = statisticsManager;
            mFightManager = fightManager;
            Random rnd = new ();
            mSeed = rnd.Next(0000, 9999);
            if (techdemo)
            {
                mDebugKeys = true;
            }
            Initialize(newGame, difficulty);

        }

        private void Initialize(bool newGame, int difficulty)
        {
            mGrid = new DiamondGrid(new Vector2(mMapSize, mMapSize), 64, new Vector2(-mMapSize + 1, 0));
            mPathfinder = new AStarPathfinder(mGrid);
            mResourceHud = new ResourceHud(mContentManager, mGraphicsDeviceManager, mDebugKeys);
            mObjectHandler = new ObjectHandler(mSoundManager, mGrid, mPathfinder, mResourceHud);
            mTroopSelectionOverlay = new TroopSelectionOverlay(mContentManager, mGraphicsDeviceManager, mInputManager, mSoundManager, mObjectHandler);
            mResourceBuildingOverlay = new ResourceBuildingOverlay(mContentManager,
                mGraphicsDeviceManager,
                mInputManager,
                mSoundManager,
                mObjectHandler);
            mTownhallOverlay = new TownhallOverlay(mContentManager,
                mGraphicsDeviceManager,
                mInputManager,
                mSoundManager,
                mObjectHandler);
            mBuildingSelectionOverlay = new BuildingSelectionOverlay(mContentManager, mGraphicsDeviceManager, mInputManager, mSoundManager, mResourceHud);
            mBuildingPreviewHandler = new BuildingPreviewHandler(mInputManager, mGrid, mBuildingSelectionOverlay, mResourceHud);
            mFpsCounter = new FpsCounter(ScreenManager, mContentManager);
            mCamera = new Camera(mGraphicsDeviceManager, mInputManager);
            mWorldMap = new Map(ScreenManager, mContentManager, mInputManager, mSoundManager, mGraphicsDeviceManager);
            mRenderTarget = new RenderTarget2D(
                mGraphicsDevice,
                (int) mWorldMap.Size.X * 100,
                (int) mWorldMap.Size.Y * 100,
                false,
                mGraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            //mFogOfWar = mContentManager.Load<Effect>("FogOfWar");
            mAIHandler = new AIHandler(mGrid, mFightManager, mStatisticsManager,mObjectHandler, mContentManager, mEnemyTownHall, mResourceHud, mBuildingSelectionOverlay, difficulty);
            DataPersistenceManager.Instance.Update(mObjectHandler.Objects, mCamera, mAIHandler, mFightManager, mObjectHandler, mSoundManager);
            DataPersistenceManager.Instance.LoadGame(false);
            if (newGame || DataPersistenceManager.Instance.mGameData.mNewGame)
            {
                DataPersistenceManager.Instance.SetSelectedProfileId("-1");

                GenerateMap(25, 14, mSeed); // 35% mit holz gefuellt, 18% mit stein. kann man beliebig aendern
                //GenerateMap(0, 0, mSeed); // 35% mit holz gefuellt, 18% mit stein. kann man beliebig aendern
            }
            Diamond snappedToGridDiamond = mGrid.GetGridDiamondFromPixel(new Vector2(-50, 300));
            mFriendlyTownHall = new TownHall(null, snappedToGridDiamond.X, snappedToGridDiamond.Y, 0, mContentManager, mFightManager);

            snappedToGridDiamond = mGrid.GetGridDiamondFromPixel(new Vector2(-50, 5050));
            mEnemyTownHall = new TownHall(null, snappedToGridDiamond.X, snappedToGridDiamond.Y, 1, mContentManager, mFightManager);

            DataPersistenceManager.Instance.Update(mObjectHandler.Objects, mCamera, mAIHandler, mFightManager, mObjectHandler, mSoundManager);

            List<Worker> friendlyWorkers = new List<Worker>();
            List<Worker> enemyWorkers = new List<Worker>();

            foreach (IGameObject gameObject in DataPersistenceManager.Instance.LoadGame(true)){
                mObjectHandler.QueueCreate(gameObject);
                if (gameObject is TownHall townHall)
                {
                    switch (townHall.Team)
                    {
                        case 0:
                            mFriendlyTownHall = townHall;
                            break;
                        case 1:
                            mEnemyTownHall = townHall;
                            break;
                    }
                }

                if (gameObject is Worker worker)
                {
                    switch (worker.Team)
                    {
                        case 0:
                            friendlyWorkers.Add(worker);
                            break;
                        case 1:
                            enemyWorkers.Add(worker);
                            break;
                    }
                }

                if (gameObject is Barracks barracks)
                {
                    if (barracks.Team == 1)
                    {
                        mAIHandler.SetupBarracks(barracks);
                    }
                    else
                    {
                        SetupBarracks(barracks);
                    }
                }

            }

            mFriendlyTownHall.mWorkers = friendlyWorkers;
            mEnemyTownHall.mWorkers = enemyWorkers;

            if (newGame || DataPersistenceManager.Instance.mGameData.mNewGame)
            {
                mObjectHandler.QueueCreateOverwrite(mFriendlyTownHall);
                mObjectHandler.QueueCreateOverwrite(mEnemyTownHall);
                
            }

            // Quick fix for this chicken-and-egg problem
            mAIHandler.LoadTownHall(mEnemyTownHall);


            mFightManager.Initialize(mPathfinder, mGrid, mObjectHandler, this);
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            gameDuration += gameTime.ElapsedGameTime;

            // retrieve inputs from InputManager
            Vector2 screenMousePosition = inputState.mMousePosition;
            Vector2 worldMousePosition = mCamera.ScreenToWorld(screenMousePosition, mCameraZoom);

            mMouseLocation = worldMousePosition;
            
            InputActions(inputState, worldMousePosition);
            GameActions(inputState, worldMousePosition);
            mCamera.Update(inputState, mGraphicsDeviceManager);
            mObjectHandler.Update(gameTime);
            mSelectionRectangle = mInputManager.GetSelectionRectangle(worldMousePosition, mSelectionStart);
            mResourceHud.UpdatePosition();
            mResourceBuildingOverlay.Update(gameTime, inputState);
            mResourceBuildingOverlay.UpdatePosition();
            mTroopSelectionOverlay.Update(gameTime, inputState);
            mTroopSelectionOverlay.UpdatePosition();
            mBuildingSelectionOverlay.Update(gameTime, inputState);
            mBuildingSelectionOverlay.UpdatePosition();
            mBuildingPreviewHandler.Update(gameTime, worldMousePosition, inputState);
            mAIHandler.Update(gameTime);
            mResourceHud.Update();
            mFpsCounter.Update(gameTime, inputState);
            mStatisticsManager.UpdateGameTime(gameTime);
            mFightManager.Update(gameTime);

            RestoreWorkers(mFriendlyTownHall);
            RestoreWorkers(mEnemyTownHall);

            if (mFriendlyTownHall.Health <= 0)
            {
                Lose();
            } else if (mEnemyTownHall.Health <= 0)
            {
                Win();
            }
        }


        private void InputActions(InputState inputState, Vector2 worldMousePosition) 
        {
            // ###########################################################################################################
            // ##                                          INPUT  ACTIONS                                               ##
            // ###########################################################################################################



            // ###################  LEFT CLICK - SELECT  ################################################


            if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonClick))
            {
                if (mTroopSelectionOverlay.IsVisiblebtn && mTroopSelectionOverlay.IsClickWithinBounds(inputState.mMousePosition))
                {
                    return;
                }
                mSelectionStart = worldMousePosition; // set the starting position
                mIsSelecting = true;
            }
            else if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonHeld) && mIsSelecting)
            {
                mDrawSelectionRectangle = true;
                foreach (KeyValuePair<string, IGameObject> kvPair in mObjectHandler.Objects)
                {
                    if (!mBuildingSelectionOverlay.BuildingHasBeenSelected && kvPair.Value is ISelectableObject selectableObject)
                    {
                        mObjectHandler.QueueDeselect(selectableObject, false);
                    }
                }
            }
            else if (mInputManager.IsActionInputted(inputState, ActionType.MouseLeftButtonReleased) && mIsSelecting)
            {

                SelectObjectsInArea(mSelectionStart, worldMousePosition); // use world coordinates for selection
                mDrawSelectionRectangle = false;
                mIsSelecting = false;

                bool workerIsSelected = false;
                bool unitIsSelected = false;
                int buildingsCount = 0; // count of selected barracks
                int resourceBuildingsCount = 0;

                foreach (KeyValuePair<string, ISelectableObject> kvPair in mObjectHandler.SelectedObjects)  // Hier werden die richtigen Overlays sichtbar
                {
                    if (kvPair.Value is IUnit unit && unit is not Worker)
                    {
                        unitIsSelected = true;
                        mSelectedBuilding = null;
                    }
                    if (kvPair.Value is Worker worker)
                    {
                        workerIsSelected = true;
                        mSelectedBuilding = null;
                    }
                    if (kvPair.Value is IBuilding building)
                    {
                        buildingsCount++;
                        mSelectedBuilding = building;
                    }
                }
                if (unitIsSelected)
                {
                    mBuildingSelectionOverlay.IsVisiblebtn = false;
                    mTroopSelectionOverlay.IsVisiblebtn = false;
                    mResourceBuildingOverlay.IsVisiblebtn = false;

                    List<ISelectableObject> toRemove = new ();
                    foreach (KeyValuePair<string, ISelectableObject> kvPair in mObjectHandler.SelectedObjects)  // Hier werden die richtigen Overlays sichtbar
                    {
                        if (!(kvPair.Value is IUnit unit && unit is not Worker))
                        {
                            kvPair.Value.IsSelected = false;
                            toRemove.Add(kvPair.Value);
                        }
                    }
                    foreach (ISelectableObject selectableObject in toRemove)
                    {
                        mObjectHandler.SelectedObjects.Remove(selectableObject.Id);
                    }
                }
                else if (workerIsSelected)  // Falls beide Selected sind, haben die Worker prio
                {
                    mBuildingSelectionOverlay.IsVisiblebtn = true;
                    mTroopSelectionOverlay.IsVisiblebtn = false;
                    mResourceBuildingOverlay.IsVisiblebtn = false;

                    foreach (KeyValuePair<string, ISelectableObject> kvPair in mObjectHandler.SelectedObjects)  // Hier werden die richtigen Overlays sichtbar
                    {
                        if (kvPair.Value is IBuilding building)
                        {
                            building.IsSelected = false;
                        }
                    }
                }
                else if (buildingsCount == 1)
                {
                    if (mSelectedBuilding is Barracks barracks)
                    {
                        mTroopSelectionOverlay.IsVisiblebtn = true;
                        mBuildingSelectionOverlay.IsVisiblebtn = false;
                        mResourceBuildingOverlay.IsVisiblebtn = false;
                        mTroopSelectionOverlay.SetSelectedBarracks(barracks);
                    }
                    else if (mSelectedBuilding is IResourceBuilding resourceBuilding)
                    {
                        if (mSelectedBuilding.BuildState == 2)
                        {
                            mResourceBuildingOverlay.IsVisiblebtn = true;
                        }
                        mBuildingSelectionOverlay.IsVisiblebtn = false;
                        mTroopSelectionOverlay.IsVisiblebtn = false;
                        mResourceBuildingOverlay.SetSelectedBuilding(resourceBuilding);
                    }
                    else
                    {
                        mTownhallOverlay.IsVisiblebtn = true;
                        mTroopSelectionOverlay.IsVisiblebtn = false;
                        mResourceBuildingOverlay.IsVisiblebtn = false;
                        mBuildingSelectionOverlay.IsVisiblebtn = false;
                        mTownhallOverlay.SetSelectedBuilding(mSelectedBuilding);
                    }
                }
                else // Falls keins selected ist
                {
                    mTroopSelectionOverlay.IsVisiblebtn = false;
                    mBuildingSelectionOverlay.IsVisiblebtn = false;
                    mResourceBuildingOverlay.IsVisiblebtn = false;
                    mTownhallOverlay.IsVisiblebtn = false;
                }
            }


            // ############################   RIGHT CLICK   ########################################################
            

            if (mInputManager.IsActionInputted(inputState, ActionType.MouseRightButtonClick))
            {
                if (mBuildingSelectionOverlay.BuildingHasBeenSelected) // This allows the building selection to be cancelled
                {
                    mBuildingSelectionOverlay.BuildingHasBeenSelected = false;
                }
            }

            // ###########################   KEY ACTIONS (NON-DEBUG)   ##############################################


            // Move with pathfinder / Attack 

            if (mInputManager.IsActionInputted(inputState, ActionType.MouseRightButtonClick))
            {

                Vector2 averagePosition = GetAveragePositionOfSelection();

                Vector2 start = averagePosition;
                Vector2 goal = worldMousePosition;

                var goalEnemy = mGrid.GetAttackableObjectFromPixel(worldMousePosition);
                if (CheckIfAttackable(goalEnemy))
                {
                    foreach (KeyValuePair<string, ISelectableObject> kvPair in mObjectHandler.SelectedObjects)
                    {
                        if (kvPair.Value is IUnit unit)
                        {
                            unit.PreventCollision = true;
                            mFightManager.SetTargetForAttacker(unit, goalEnemy);
                        }
                    }
                }
                // if not clicked on IAttackable object that is neutral or enemy, just move there
                else
                {
                    mFightManager.RemoveAttackers(mObjectHandler.SelectedObjects);

                    foreach (KeyValuePair<string, ISelectableObject> kvPair in mObjectHandler.SelectedObjects)
                    {
                        if (kvPair.Value is IMovingObject movingObject)
                        {
                            movingObject.IsMoving = true;
                            movingObject.PreventCollision = false;
                            mObjectHandler.QueueMove(movingObject, goal, false);
                            
                            if (kvPair.Value is Worker worker)
                            {
                                worker.RightClickActions(mGrid, worldMousePosition);
                            }
                        }
                    }
                }
            }
            
            // AttackMove (F)
            if (mInputManager.IsActionInputted(inputState, ActionType.AttackMove))
            {
                mFightManager.RemoveAttackers(mObjectHandler.SelectedObjects);

                foreach (KeyValuePair<string, ISelectableObject> kvPair in mObjectHandler.SelectedObjects)
                {
                    if (kvPair.Value is IUnit unit)
                    {
                        if (unit is IMovingObject movingObject)
                        {
                            movingObject.IsMoving = true;
                        }
                        unit.PreventCollision = true;
                        mFightManager.AddAttackMoveAttacker(unit, worldMousePosition);
                    }
                }
            }

            float scrollWheelValueDelta = mInputManager.GetMouseScroll();

            // change zoom factor depending on mousewheel changes
            mCameraZoom *= 1 + (0.0005f * scrollWheelValueDelta);

            // limit zoom to certain values
            mCameraZoom = MathHelper.Clamp(mCameraZoom, 0.4f, 1.5f);
            

            // ##############################   DEBUG KEY ACTIONS   ####################################################

            // ----------------------------------   SPAWNING   --------------------------------------

            // Spawn Allied Unit at mouse location (Z)

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugSpawnAlliedSwordsman) && mDebugKeys)
            {
                
                switch (mTroopSelectionOverlay.SelectedTroopType)
                {
                    case TroopType.Swordsman:
                        mObjectHandler.QueueCreate(new Swordsman(null, (int)worldMousePosition.X, (int)worldMousePosition.Y, 0, mContentManager, mFightManager, mStatisticsManager));
                        break;
                    case TroopType.Archer:
                        mObjectHandler.QueueCreate(new Archer(null, (int)worldMousePosition.X - 20, (int)worldMousePosition.Y - 20, 0, mContentManager, mFightManager, mStatisticsManager));
                        break;
                    case TroopType.Knight:
                        mObjectHandler.QueueCreate(new Knight(null, (int)worldMousePosition.X - 20, (int)worldMousePosition.Y - 20, 0, mContentManager, mFightManager, mStatisticsManager));
                        break;
                }
                /*for (int i = 0; i < 1000; i++)
                {
                    mObjectHandler.QueueCreate(new Swordsman(null, (int)worldMousePosition.X, (int)worldMousePosition.Y, 0, mContentManager, mFightManager, mStatisticsManager));
                }*/
            }
            
            // Spawn Enemy Unit at mouse location (F3)

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugSpawnEnemyUnit) && mDebugKeys)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SPAWNING ENEMY UNIT");
                }
                switch (mTroopSelectionOverlay.SelectedTroopType)
                {
                    case TroopType.Swordsman:
                        mObjectHandler.QueueCreate(new Swordsman(null, (int)worldMousePosition.X, (int)worldMousePosition.Y, 1, mContentManager, mFightManager, mStatisticsManager));
                        break;
                    case TroopType.Archer:
                        mObjectHandler.QueueCreate(new Archer(null, (int)worldMousePosition.X - 20, (int)worldMousePosition.Y - 20, 1, mContentManager, mFightManager, mStatisticsManager));
                        break;
                    case TroopType.Knight:
                        mObjectHandler.QueueCreate(new Knight(null, (int)worldMousePosition.X - 20, (int)worldMousePosition.Y - 20, 1, mContentManager, mFightManager, mStatisticsManager));
                        break;
                }
            }
            //Spawn 100 Swordsman (F9) on non Filled Squares
            if (mInputManager.IsActionInputted(inputState, ActionType.DebugSaveMovingObjects) && mDebugKeys)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SPAWNING Friendly UNITs");
                }
                switch (mTroopSelectionOverlay.SelectedTroopType)
                {
                    case TroopType.Swordsman:
                        var count = 0;
                        var row = 0;
                        for (int i = 0; i < 100; i++)
                        {
                            if (count == 10)
                            {
                                row++;
                                count = 0;
                            }

                            var position =
                                mGrid.TranslateFromGrid(mGrid.TranslateToGrid(worldMousePosition) + i + row * 74);
                            if (!mGrid.CheckIfGridLocationIsFilledFromPixel(position))
                            {
                                mObjectHandler.QueueCreate(new Swordsman(null,
                                    (int)position.X,
                                    (int)position.Y,
                                    0,
                                    mContentManager,
                                    mFightManager,
                                    mStatisticsManager));
                            }

                            count++;
                        }
                        
                        break;
                }
            }
            //Spawn 100 Enemy Swordsman (F10)
            if (mInputManager.IsActionInputted(inputState, ActionType.DebugLoadMovingObjects) && mDebugKeys)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("SPAWNING Enemy UNITs");
                }
                switch (mTroopSelectionOverlay.SelectedTroopType)
                {
                    case TroopType.Swordsman:
                        var count = 0;
                        var row = 0;
                        for (int i = 0; i < 100; i++)
                        {
                            if (count == 10)
                            {
                                row++;
                                count = 0;
                            }

                            var position =
                                mGrid.TranslateFromGrid(mGrid.TranslateToGrid(worldMousePosition) + i + row * 74);
                            if (!mGrid.CheckIfGridLocationIsFilledFromPixel(position))
                            {
                                mObjectHandler.QueueCreate(new Swordsman(null,
                                    (int)position.X,
                                    (int)position.Y,
                                    1,
                                    mContentManager,
                                    mFightManager,
                                    mStatisticsManager));
                            }

                            count++;
                        }

                        break;
                }
            }
            // Spawn Worker at mouse location (U)

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugSpawnAlliedWorker) && mDebugKeys)
            {
                mObjectHandler.QueueCreate(new Worker(null, (int)worldMousePosition.X - 20, (int)worldMousePosition.Y - 20, 0, mContentManager, mFightManager, mStatisticsManager));
            }

            if (mInputManager.IsActionInputted(inputState, ActionType.PressKey0) && mDebugKeys)
            {  
                mFriendlyTownHall.Health = 0;
            }
            if (mInputManager.IsActionInputted(inputState, ActionType.PressKey1) && mDebugKeys)
            {
                mEnemyTownHall.Health = 0;
            }

            if (mInputManager.IsActionInputted(inputState, ActionType.PlaceBuilding) && mDebugKeys)
            {
                Diamond snappedToGridDiamond = mBuildingPreviewHandler.SnappedToGridDiamond;
                Vector2 target = new(snappedToGridDiamond.X, snappedToGridDiamond.Y);
                IBuilding newBuilding = null;

                switch (mBuildingSelectionOverlay.SelectedBuildingType)
                {
                    case BuildingSelectionOverlay.BuildingType.Quarry:
                        newBuilding = new Quarry(null, snappedToGridDiamond.X, snappedToGridDiamond.Y, 1, mContentManager, mFightManager, mStatisticsManager);
                        break;
                    case BuildingSelectionOverlay.BuildingType.LumberCamp:
                        newBuilding = new LumberCamp(null, snappedToGridDiamond.X, snappedToGridDiamond.Y, 1, mContentManager, mFightManager, mStatisticsManager);
                        break;
                        // Add cases for other building types
                }

                if (newBuilding != null)
                {
                    mObjectHandler.QueueCreate(newBuilding);

                    UpdateUnitPaths(snappedToGridDiamond);

                    // Close the building overlay
                    mBuildingPreviewHandler.BuildingHasBeenPlaced = false;
                }
            }


            // Spawn three example buildings 

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugSpawnTownHall) && mDebugKeys)
            {
                mObjectHandler.QueueCreate(new TownHall(null, (int)worldMousePosition.X, (int)worldMousePosition.Y, 1, mContentManager, mFightManager));
                mObjectHandler.QueueCreate(new Quarry(null, (int)worldMousePosition.X + 200, (int)worldMousePosition.Y, 1, mContentManager, mFightManager , mStatisticsManager));
                mObjectHandler.QueueCreate(new LumberCamp(null, (int)worldMousePosition.X, (int)worldMousePosition.Y + 200, 1, mContentManager, mFightManager, mStatisticsManager));
            }
            
            if (mInputManager.IsActionInputted(inputState, ActionType.DebugToggleAI) && mDebugKeys)
            {
                mAIHandler.IsDebug = !mAIHandler.IsDebug;
                mSoundManager.PlaySound("SoundAssets/basic_soundeffect", 1, false, false);
            }

            
            // ---------------------------------------------------------------------------------------------
            // ------------------------------------   DELETE   ---------------------------------------------


            // Delete selected Gameobjects (X)

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugRemoveAllSelected) && mDebugKeys)
            {
                foreach (KeyValuePair<string, ISelectableObject> kvPair in mObjectHandler.SelectedObjects)
                {
                    if (kvPair.Value is IUnit unit)
                    {
                        unit.IsDead = true;
                    }
                }
            }
            if (mInputManager.IsActionInputted(inputState, ActionType.Exit))
            {
                mAchievementsManager.Update(mObjectHandler.Objects, mResourceHud,false, gameDuration);
                DataPersistenceManager.Instance.Update(mObjectHandler.Objects, mCamera, mAIHandler, mFightManager, mObjectHandler, mSoundManager);
                mInputManager.Flush();
                mSoundManager.StopAllSounds();   
                ScreenManager.AddScreen<PauseMenu>(false, true, false, false, -1);
            }

            // -----------------------------------------------------------------------------------------------
            // ------------------------------------   OVERLAYS   ---------------------------------------------


            // Debug open Troop Selection Overlay

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugToggleTroopSelectionOverlayVisibility) && !mBuildingSelectionOverlay.IsVisiblebtn)
            {
                mTroopSelectionOverlay.ToggleVisibility();
            }


            // Debug open Building Selection Overlay

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugToggleBuildingSelectionOverlayVisibility) && !mTroopSelectionOverlay.IsVisiblebtn)
            {
                mBuildingSelectionOverlay.ToggleVisibility();
            }


            // Toggle FPS Overlay visibility

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugToggleFpsCounterVisibility) && mDebugKeys)
            {
                mFpsCounter.ToggleVisibility();
            }


            // Toggle Grid visibility

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugToggleGrid) && mDebugKeys)
            {
                mGrid.ShowGridLines = !mGrid.ShowGridLines;
            }

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugTogglePathfindingGrid) && mDebugKeys)
            {
                mGrid.ShowPathfindingGrid = !mGrid.ShowPathfindingGrid;
            }

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugToggleCollisionGrid) && mDebugKeys)
            {
                mGrid.ShowCollisionGrid = !mGrid.ShowCollisionGrid;
            }

            // Toggle Hitbox visibility

            if (mInputManager.IsActionInputted(inputState, ActionType.DebugToggleShowHitbox) && mDebugKeys)
            {
                mShowHitbox = !mShowHitbox;
                mShowTextureRectangle = !mShowTextureRectangle;
            }

            // -----------------------------------------------------------------------------------------------
            // --------------------------------------   MAP   ------------------------------------------------

            // Debug generate sparse map
            if (mInputManager.IsActionInputted(inputState, ActionType.PressKey7) && mDebugKeys)
            {
                int count = mObjectHandler.Objects.Count;
                mGrid = new DiamondGrid(new Vector2(mMapSize, mMapSize), 64, new Vector2(-mMapSize + 1, 0));
                mObjectHandler.QueueDeleteAll();
                GenerateMap(15, 8, 1234 + count);
            }

            // Debug generate filled map
            if (mInputManager.IsActionInputted(inputState, ActionType.PressKey8) && mDebugKeys)
            {
                mGrid = new DiamondGrid(new Vector2(mMapSize, mMapSize), 64, new Vector2(-mMapSize + 1, 0));
                mObjectHandler.QueueDeleteAll(); 
                GenerateMap(85, 16, 9876);
            }

            // Debug generate stone map
            if (mInputManager.IsActionInputted(inputState, ActionType.PressKey9) && mDebugKeys)
            {
                mGrid = new DiamondGrid(new Vector2(mMapSize, mMapSize), 64, new Vector2(-mMapSize + 1, 0));
                mObjectHandler.QueueDeleteAll();
                GenerateMap(20, 43, 1234);
            }
        }


        private void GameActions(InputState inputState, Vector2 worldMousePosition)
        {
            // ###########################################################################################################
            // ##                                           GAME  ACTIONS                                               ##
            // ###########################################################################################################


            // ###########################   BUILDING LOGIC   ####################################

            // If a building is placed, selected workers move towards it

            if (mBuildingPreviewHandler.BuildingHasBeenPlaced)
            {
                // Helper vars
                int buildingType = mBuildingSelectionOverlay.GetBuildingType();
                Diamond snappedToGridDiamond = mBuildingPreviewHandler.SnappedToGridDiamond;
                bool build = false;

                // The grid location is filled in mBuildingPreviewHandler

                // Adds the correct building to mGameObjects

                Vector2 target = new (snappedToGridDiamond.X,snappedToGridDiamond.Y);

                IBuilding building = null;
                
                switch (buildingType)
                {
                    case 0:
                        if (mGrid.GetStaticObjectFromPixel(target) is Stone && (mResourceHud.UseResources(mBuildingSelectionOverlay.GetBuildingCost(0), 0)))
                        {
                            Quarry quarry = new (null, (int)target.X, (int)target.Y, 0, mContentManager, mFightManager, mStatisticsManager);
                            building = quarry;
                            mObjectHandler.QueueCreateOverwrite(quarry);
                            quarry.ResourceHud = mResourceHud;
                            build = true;
                        }
                        break;
                    case 1:
                        if (mGrid.GetStaticObjectFromPixel(target) is Tree && mResourceHud.UseResources(mBuildingSelectionOverlay.GetBuildingCost(1), 0))
                        {
                            LumberCamp lumberCamp = new (null, (int)target.X, (int)target.Y, 0, mContentManager, mFightManager, mStatisticsManager);
                            building = lumberCamp;
                            mObjectHandler.QueueCreateOverwrite(lumberCamp);
                            lumberCamp.ResourceHud = mResourceHud;
                            build = true;
                        }
                        break;
                    case 2:
                        if (mResourceHud.UseResources(mBuildingSelectionOverlay.GetBuildingCost(2), 0))
                        {
                            Barracks barracks = new (null, (int)target.X, (int)target.Y, 0, mContentManager, mFightManager);
                            building = barracks;
                            mObjectHandler.QueueCreateOverwrite(barracks);
                            SetupBarracks(barracks);
                            barracks.ResourceHud = mResourceHud;
                            barracks.TroopSelection = mTroopSelectionOverlay;

                            build = true;
                        }
                        break;
                    default:
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine("Invalid buildingType");
                        }
                        break;
                }

                if (build && building != null)
                {
                    
                    // Assignes the building to grid
                   
                    // Closes the building overlay
                    mBuildingPreviewHandler.BuildingHasBeenPlaced = false;
                    mBuildingSelectionOverlay.BuildingHasBeenSelected = false;
                    
                    // Moves selected workers to build site
                    foreach (KeyValuePair<string, IGameObject> kvPair in mObjectHandler.Objects) {
                        if (kvPair.Value is Worker worker) 
                        { 
                            if (worker.IsSelected)
                            {
                                mObjectHandler.QueueMove((IGameObject)worker, new Vector2(target.X, target.Y), false);
                                worker.IsMovingToBuild = true;
                                worker.BuildingBeingBuilt = building;
                            }
                        }
                    }
                    // Rectangle newBuildingArea = new Rectangle((int)target.X, (int)target.Y, SnappedToGridDiamond.Width, SnappedToGridDiamond.Height);
                    //UpdateUnitPaths(newBuildingArea);
                }
            }
                

            // #########################################   SOUNDS   ####################################################


            //  Not sure what this is supposed to do. Could probably just delete it.

            if (mInputManager.IsActionInputted(inputState, ActionType.PlayTileSound))
            {
                mObjectHandler.QueueCreate(new Worker(null, (int)worldMousePosition.X, (int)worldMousePosition.Y, 1, mContentManager, mFightManager, mStatisticsManager));
            }

            // -------------------------------------------------------------------------------------------
            // ----------------------------------   SAVE - LOAD   ----------------------------------------
        }


        public void DrawWithoutButtons(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch);
        }
        public void Draw(SpriteBatch spriteBatch)
        {

            // ###########################################################################################################
            // ##                                             DRAW                                                      ##
            // ###########################################################################################################



            // for all camera related elements

            // Fog of war shader - doesn't work yet. can't figure out the bugs. pain

            if (mShowFogOfWar)
            {
                
                mGraphicsDevice.SetRenderTarget(null);
                mGraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                mGraphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(blendState: BlendState.AlphaBlend);

                /*foreach (IGameObject gameObject in mGameObjects)
                {
                    spriteBatch.Draw(mContentManager.Load<Texture2D>("2DAssets/circle"), gameObject.Position, null, Color.White, 0f, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0);
                }
                */
                spriteBatch.Draw(mContentManager.Load<Texture2D>("2DAssets/circle"), new Rectangle(0, 0, 10000, 10000), Color.IndianRed);
                spriteBatch.End();

                mGraphicsDevice.SetRenderTarget(null);
            }

            mCamera.mSpeed = 10 / mCameraZoom;
            mCamera.mZoom = mCameraZoom;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: mCamera.Transform);
            mWorldMap.Draw(spriteBatch);

            // Display the grids

            if (mGrid.ShowGridLines)
            {
                mGrid.DrawGrid(spriteBatch);
            }

            if (mGrid.ShowCollisionGrid)
            {
                foreach (var filledCollidableGrid in mGrid.mGridContent)
                {
                    mGrid.DrawGridLocation(spriteBatch, filledCollidableGrid.Key, Color.Turquoise);
                }
            }
            
            if (mGrid.ShowPathfindingGrid)
            {
                mGrid.DrawPathfindingGrid(spriteBatch, Color.OrangeRed);
            }

            // Draws all objects 

            if (mDrawSelectionRectangle)
            {
                spriteBatch.DrawRectangle(mSelectionRectangle, new Color(0, 255, 41, 100));
            }

            mObjectHandler.DrawAllObjects(spriteBatch, mShowHitbox, mShowTextureRectangle);

            mBuildingPreviewHandler.Draw(spriteBatch);
            mGraphicsDevice.Clear(Color.Transparent);
            spriteBatch.End();

            if (mShowFogOfWar)
            {
                spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: mCamera.Transform * Matrix.CreateScale(new Vector3(mCameraZoom, mCameraZoom, 1)));
                spriteBatch.Draw(mRenderTarget, new Rectangle(0, 0, 1000, 1000), Color.White);
                spriteBatch.End();
            }

            // for UI elements
            spriteBatch.Begin();
            if (mDebugKeys)
            {
                mFpsCounter.Draw(spriteBatch);
            }
            mTroopSelectionOverlay.Draw(spriteBatch);
            mResourceBuildingOverlay.Draw(spriteBatch);
            mTownhallOverlay.Draw(spriteBatch);
            mBuildingSelectionOverlay.Draw(spriteBatch);
            mResourceHud.Draw(spriteBatch);
            spriteBatch.End();
        }





        // ######################################   HELPER METHODS   #############################################


        private void SetupBarracks(Barracks barracks)
        {
            barracks.OnTroopSpawn += HandleTroopSpawn;
        }

        private async void HandleTroopSpawn(TroopType troopType, Vector2 location)
        {
            ISelectableObject newUnit = null;

            switch (troopType)
            {
                case TroopType.Swordsman:
                    newUnit = new Swordsman(null, (int)location.X, (int)location.Y, 0, mContentManager, mFightManager, mStatisticsManager);
                    break;
                case TroopType.Archer:
                    newUnit = new Archer(null, (int)location.X, (int)location.Y, 0, mContentManager, mFightManager, mStatisticsManager);
                    break;
                case TroopType.Knight:
                    newUnit = new Knight(null, (int)location.X, (int)location.Y, 0, mContentManager, mFightManager, mStatisticsManager);
                    break;
                case TroopType.Worker:
                    Worker newWorker = new Worker(null, (int)location.X, (int)location.Y, 0, mContentManager, mFightManager, mStatisticsManager);
                    newUnit = newWorker;
                    mFriendlyTownHall.mWorkers.Add(newWorker);
                    break;
            }

            if (newUnit != null)
            {
                mObjectHandler.QueueCreate(newUnit);
                mStatisticsManager.UnitTrained();
                mGrid.UpdateGrid(newUnit);
            }
            await System.Threading.Tasks.Task.Delay(200);

            HandleBarracksSpawnMove(location);

        }

        private void HandleBarracksSpawnMove(Vector2 location)
        {

            List<IGameObject> nearbyObjects = new ();

            /* This seems to just push nearby workers away and isn't really needed
            foreach (int loc in mGrid.GetNeighbors(mGrid.TranslateToGrid(location)))
            {
                if (mGrid.mGridContent.TryGetValue(loc, out List<IGameObject> objs))
                {
                    foreach (var obj in objs)
                    {
                        nearbyObjects.Add(obj);
                    }
                }
            }
            */

            if (mGrid.mGridContent.TryGetValue(mGrid.TranslateToGrid(location), out List<IGameObject> objs2))
            {
                foreach (var obj in objs2)
                {
                    nearbyObjects.Add(obj);
                }
            }


            int stepsDown = 70;

            foreach (var obj in nearbyObjects)
            {

                if (obj is IMovingObject movingObject)
                {
                    Vector2 newPosition = new Vector2(movingObject.Position.X - 70, movingObject.Position.Y + stepsDown);
                    movingObject.IsMoving = true;
                    movingObject.PreventCollision = true;
                    movingObject.Destination = newPosition;
                    mObjectHandler.QueueMove(movingObject, newPosition, false);

                }
            }
        }


        private bool IsPathBlockedByNewBuilding(List<Vector2> path, Diamond buildingDiamond)
        {
            foreach (Vector2 point in path)
            {
                if (buildingDiamond.Contains(point))
                {
                    return true;
                }
            }
            return false;
        }



        private void UpdateUnitPaths(Diamond newBuildingDiamond)
        {
            foreach (KeyValuePair<string, IGameObject> kvPair in mObjectHandler.Objects)
            {
                if (kvPair.Value is IMovingObject movingObject)
                {
                    if (IsPathBlockedByNewBuilding(movingObject.Path, newBuildingDiamond))
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine("path blocked, recalculating...");
                        }

                        // clear the existing path
                        movingObject.Path = null;
                        Vector2 start = new Vector2(kvPair.Value.HitboxRectangle.X, kvPair.Value.HitboxRectangle.Y);
                        Vector2 goal = movingObject.Goal;
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine("WorldScreen calls FindPath");
                        }
                        movingObject.Path = mPathfinder.FindPath(start, goal);
                    }
                }
            }
        }

        
        private void SelectObjectsInArea(Vector2 start, Vector2 end)
        {
            //TODO select objects depending on grid.
            //TODO get grid position form pixel position. Beware that the selection can contain multiple grid positions.
            //TODO retrieve the objects from the grid positions and select them.

            //TODO: aggregate all grid positions between start and end.
            mObjectHandler.SelectedObjects.Clear();
            
            List<int> slotPositions = mGrid.GetSlotPositionsInArea(start, end);
            
            foreach (int slotPosition in slotPositions) 
            {
                if (mGrid.mGridContent.TryGetValue(slotPosition, out List<IGameObject> gameObjects)) {

                    foreach (IGameObject gameObject in gameObjects)
                    {
                        if (gameObject is ISelectableObject selectableObject && IsWithinSelection(gameObject, start, end) && selectableObject.Team == 0)
                        {
                            {
                                mObjectHandler.SelectedObjects.TryAdd(selectableObject.Id, selectableObject);
                                selectableObject.IsSelected = true;
                            }
                            //mObjectHandler.QueueSelect(selectableObject, false);
                            //Debug.WriteLine($"Object at {gameObject.HitboxRectangle} is selected.");
                            //Debug.WriteLine($"Object is in Grid Position: {mGrid.TranslateToGrid(gameObject.Position)}");
                        }
                    }
                }
            }
            
        }
        
        private bool IsWithinSelection(IGameObject gameObject, Vector2 selectionMouseStartPosition, Vector2 selectionMouseEndPosition)
        {
            return mInputManager.IsWithinSelection(gameObject.HitboxRectangle,
                selectionMouseStartPosition,
                selectionMouseEndPosition);
        }
        
        private Vector2 GetAveragePositionOfSelection()
        {
            Vector2 averagePosition = new Vector2();
            int selectedObjects = 0;
            foreach (KeyValuePair<string, ISelectableObject> kvPair in mObjectHandler.SelectedObjects)
            {
                averagePosition += new Vector2(kvPair.Value.HitboxRectangle.X, kvPair.Value.HitboxRectangle.Y);
                selectedObjects += 1;
            }
            return averagePosition / selectedObjects;
        }

        private void GenerateMap(int seed)
        {
            InitializeResourcesFromMap(seed);
        }

        private void GenerateMap(int wood, int stone, int seed)
        {
            InitializeResourcesFromMap(seed + 10000 * stone + 1000000 * wood);
        }

        private void InitializeResourcesFromMap(int seed)
        {
            NoiseMap map = new(seed, mMapSize);

            for (int i = 0; i < mMapSize * mMapSize; i++)
            {
                if (map.WoodGrid[i] >= map.WoodCutoff)
                {
                    Vector2 coord = mGrid.TranslateFromGrid(i);
                    if (!mGrid.CheckIfGridLocationIsFilledFromPixel(coord))
                    {
                        mObjectHandler.QueueCreate(new Tree(null, (int)coord.X, (int)coord.Y, mContentManager, mFightManager));
                    }
                }
                if (map.StoneGrid[i] >= map.StoneCutoff)
                {
                    Vector2 coord = mGrid.TranslateFromGrid(i);
                    mObjectHandler.QueueCreateOverwrite(new Stone(null, (int)coord.X, (int)coord.Y, mContentManager, mFightManager));
                }
            }
        }
        public void AddArrow(Vector2 position, Vector2 destination, int damage, int team)
        { 
            mObjectHandler.QueueCreate(new Arrow(position, destination-position, damage, team, mContentManager, mGrid));
        }

        private void Win()
        {
            mAchievementsManager.Update(mObjectHandler.Objects, mResourceHud, true, gameDuration);


            ScreenManager.AddScreen<WinLoseScreen>(false, false, false, true, -1);
        }

        private void Lose()
        {
            mAchievementsManager.Update(mObjectHandler.Objects, mResourceHud, false, gameDuration);


            ScreenManager.AddScreen<WinLoseScreen>(false, false, false, false, -1);
        }
        
        private void RestoreWorkers(TownHall th)
        {
            while (th.WorkersMissing > 0)
            {
                Worker newWorker = new Worker(null,
                    (int)th.Position.X + 50,
                    (int)th.Position.Y + 50,
                    th.Team,
                    mContentManager,
                    mFightManager, mStatisticsManager);
                th.mWorkers.Add(newWorker);
                mObjectHandler.QueueCreate(newWorker);
                th.WorkersMissing--;
            }
        }

        private bool CheckIfAttackable(IAttackableObject goalEnemy)
        {
            if (goalEnemy is null) return false;
            if (goalEnemy is IMapObject) return true;
            //TODO: hier ist leider keine vereinheitlichung zwischen buildings and units (einmal IsAllied, einmal Player)
            var unit = goalEnemy as IUnit;
            if (unit != null && unit.Team == 0) return false;
            var building = goalEnemy as IBuilding;
            if (building != null && building.Team == 0) return false;
            return true;
        }
    }
    
}