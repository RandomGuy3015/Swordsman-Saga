using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.GameElements.GameObjects.Buildings;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.GameElements.Screens.HUDs;

namespace Swordsman_Saga.Engine.DataPersistence.Data
{
    [System.Serializable]
    public class GameData
    {
        public Dictionary<string, string> mGameObjects;
        public Dictionary<string, int> mUnitPlayer;
        public Dictionary<string, int> mHealth;
        public Dictionary<string, Vector2> mObjectPosition;
        public Dictionary<string, int> mCompletionTimers;
        public Dictionary<string, Rectangle> mMapRectangles;
        public Dictionary<string, int> mBuildStates;
        public Dictionary<string, int> mWoodGeneration;
        public Dictionary<string, int> mStoneGeneration;
        public Dictionary<string, int> mLevel;
        public Dictionary<string, Vector2> mUpgradeCost;
        public Dictionary<string, Queue<TroopType>> mTrainingQueues;
        public Dictionary<string, float> mCurrentTrainingTimes;
        public int mWoodCount;
        public int mStoneCount;
        //Units
        public Dictionary<string, bool> mIsMoving;
        public Dictionary<string, bool> mIsColliding;
        public Dictionary<string, bool> mJustStartedMoving;
        public Dictionary<string, List<Vector2>> mPaths;
        public Dictionary<string, Vector2> mGoals;
        public Dictionary<string, Vector2> mUnitDestination;
        public Dictionary<string, Vector2> mPathingTo;
        public Dictionary<string, int> mCurrentFrame;
        public Dictionary<string, float> mTimePastSinceLastFrame;
        public Dictionary<string, int> mAction;
        public Dictionary<string, bool> mIsDead;
        public Dictionary<string, bool> mAttacked;
        public Dictionary<string, bool> mFlipTexture;
        public Dictionary<string, bool> mIsMovingInQueue;
        //Worker
        public Dictionary<string, bool> mIsBuilding;
        public Dictionary<string, bool> mIsMovingToBuild;
        public Dictionary<string, string> mBuildingBeingBuilt;

        //AI
        public float mWoodToStoneRatio;
        public int mBuildingsBuild;
        public int mBarracksBuilt;
        public Dictionary<int, int> mDivisionSizes;
        public int mDivisionCount;
        public TimeSpan mTimeSinceGameStart;
        public int mAIWoodCount;
        public int mAIStoneCount;

        //Task
        public Dictionary<int, string> mTasks;
        public Dictionary<string, bool> mIsTaskDone;
        public Dictionary<string, bool> mIsNewTask;
        public Dictionary<string, string> mSelf;
        public Dictionary<string, string> mOther;
        public Dictionary<string, TaskType> mType;
        public Dictionary<string, Vector2> mDestination;
        public Dictionary<string, int> mRange;
        public Dictionary<string, TaskType> mNextTask;
        public Dictionary<string, Vector2> mTaskGoal;
        public Dictionary<string, int> mTaskStuck;
        public Dictionary<string, int> mTaskCantReachTarget;
        public Dictionary<string, int> mTaskDivision;
        public Dictionary<string, bool> mTaskIsMetaTask;


        //Move
        public Dictionary<int, string> mMoveGameObjects;
        public Dictionary<int, Vector2> mMoveTarget;
        public Dictionary<int, int> mMoveMoveType;

        //ObjectManager
        public int mMovesCount;
        public int mNextMovesCount;

        public bool mNewGame;

        public Vector2 mCameraPosition;
        public Dictionary<ActionType, IEvent> mKeyBindings = new() 
            {
                { ActionType.MouseButton1Click, new MouseEvent(EventType.OnButtonPress, MouseButton.Button1) },
                { ActionType.MouseButton2Click, new MouseEvent(EventType.OnButtonPress, MouseButton.Button2) },
                { ActionType.MouseLeftButtonClick, new MouseEvent(EventType.OnButtonPress, MouseButton.LeftButton) },
                { ActionType.MouseMiddleButtonClick, new MouseEvent(EventType.OnButtonPress, MouseButton.MiddleButton) },
                { ActionType.MouseRightButtonClick, new MouseEvent(EventType.OnButtonPress, MouseButton.RightButton) },
                { ActionType.MouseButton1Held, new MouseEvent(EventType.OnButtonHeld, MouseButton.Button1) },
                { ActionType.MouseButton2Held, new MouseEvent(EventType.OnButtonHeld, MouseButton.Button2) },
                { ActionType.MouseLeftButtonHeld, new MouseEvent(EventType.OnButtonHeld, MouseButton.LeftButton) },
                { ActionType.MouseMiddleButtonHeld, new MouseEvent(EventType.OnButtonHeld, MouseButton.MiddleButton) },
                { ActionType.MouseRightButtonHeld, new MouseEvent(EventType.OnButtonHeld, MouseButton.RightButton) },
                { ActionType.MouseLeftButtonReleased, new MouseEvent(EventType.OnButtonRelease, MouseButton.LeftButton) },
                { ActionType.PressKey0, new KeyEvent(EventType.OnButtonPress, Keys.D0) },
                { ActionType.PressKey1, new KeyEvent(EventType.OnButtonPress, Keys.D1) },
                { ActionType.PressKey2, new KeyEvent(EventType.OnButtonPress, Keys.D2) },
                { ActionType.PressKey3, new KeyEvent(EventType.OnButtonPress, Keys.D3) },
                { ActionType.PressKey4, new KeyEvent(EventType.OnButtonPress, Keys.D4) },
                { ActionType.PressKey5, new KeyEvent(EventType.OnButtonPress, Keys.D5) },
                { ActionType.PressKey6, new KeyEvent(EventType.OnButtonPress, Keys.D6) },
                { ActionType.PressKey7, new KeyEvent(EventType.OnButtonPress, Keys.D7) },
                { ActionType.PressKey8, new KeyEvent(EventType.OnButtonPress, Keys.D8) },
                { ActionType.PressKey9, new KeyEvent(EventType.OnButtonPress, Keys.D9) },
                { ActionType.PressBackSpaceKey, new KeyEvent(EventType.OnButtonPress, Keys.Back) },
                { ActionType.PressEnterKey, new KeyEvent(EventType.OnButtonPress, Keys.Enter) },
                { ActionType.HoldKey0, new KeyEvent(EventType.OnButtonHeld, Keys.D0) },
                { ActionType.HoldKey1, new KeyEvent(EventType.OnButtonHeld, Keys.D1) },
                { ActionType.HoldKey2, new KeyEvent(EventType.OnButtonHeld, Keys.D2) },
                { ActionType.HoldKey3, new KeyEvent(EventType.OnButtonHeld, Keys.D3) },
                { ActionType.HoldKey4, new KeyEvent(EventType.OnButtonHeld, Keys.D4) },
                { ActionType.HoldKey5, new KeyEvent(EventType.OnButtonHeld, Keys.D5) },
                { ActionType.HoldKey6, new KeyEvent(EventType.OnButtonHeld, Keys.D6) },
                { ActionType.HoldKey7, new KeyEvent(EventType.OnButtonHeld, Keys.D7) },
                { ActionType.HoldKey8, new KeyEvent(EventType.OnButtonHeld, Keys.D8) },
                { ActionType.HoldKey9, new KeyEvent(EventType.OnButtonHeld, Keys.D9) },
                { ActionType.MoveCameraDown, new KeyEvent(EventType.OnButtonHeld, Keys.S) },
                { ActionType.MoveCameraLeft, new KeyEvent(EventType.OnButtonHeld, Keys.A) },
                { ActionType.MoveCameraUp, new KeyEvent(EventType.OnButtonHeld, Keys.W) },
                { ActionType.MoveCameraRight, new KeyEvent(EventType.OnButtonHeld, Keys.D) },
                { ActionType.SpeedUpCamera, new KeyEvent(EventType.OnButtonHeld, Keys.LeftShift) },
                { ActionType.Exit, new KeyEvent(EventType.OnButtonPress, Keys.Escape) },
                { ActionType.MainMenu, new KeyEvent(EventType.OnButtonPress, Keys.M) },
                { ActionType.PlaceBuilding, new KeyEvent(EventType.OnButtonPress, Keys.P) },
                { ActionType.ToggleFullscreen, new KeyEvent(EventType.OnButtonRelease, Keys.F11) },
                { ActionType.DebugSpawnAlliedKnight, new KeyEvent(EventType.OnButtonPress, Keys.H) },
                { ActionType.DebugSpawnAlliedRider, new KeyEvent(EventType.OnButtonPress, Keys.J) },
                { ActionType.DebugSpawnAlliedSwordsman, new KeyEvent(EventType.OnButtonPress, Keys.Z) },
                { ActionType.DebugSpawnAlliedWorker, new KeyEvent(EventType.OnButtonPress, Keys.U) },
                { ActionType.DebugSpawnEnemyUnit, new KeyEvent(EventType.OnButtonPress, Keys.F3) },
                { ActionType.EnterFightMode, new KeyEvent(EventType.OnButtonPress, Keys.F8)},
                { ActionType.DebugToggleAI, new KeyEvent(EventType.OnButtonPress, Keys.Q) },
                { ActionType.AttackMove, new KeyEvent(EventType.OnButtonPress, Keys.F) },
                { ActionType.DebugRemoveAllSelected, new KeyEvent(EventType.OnButtonPress, Keys.X) },
                { ActionType.DebugToggleTroopSelectionOverlayVisibility, new KeyEvent(EventType.OnButtonPress, Keys.K) },
                { ActionType.DebugToggleBuildingSelectionOverlayVisibility, new KeyEvent(EventType.OnButtonPress, Keys.B) },
                { ActionType.DebugSpawnTownHall, new KeyEvent(EventType.OnButtonPress, Keys.J) },
                { ActionType.DebugToggleFpsCounterVisibility, new KeyEvent(EventType.OnButtonPress, Keys.F) },
                { ActionType.DebugSaveMovingObjects, new KeyEvent(EventType.OnButtonPress, Keys.F9) },
                { ActionType.DebugLoadMovingObjects, new KeyEvent(EventType.OnButtonPress, Keys.F10) },
                { ActionType.PlayTileSound, new KeyEvent(EventType.OnButtonPress, Keys.F12) },
                { ActionType.DebugToggleGrid, new KeyEvent(EventType.OnButtonPress, Keys.F5) },
                { ActionType.DebugToggleShowHitbox, new KeyEvent(EventType.OnButtonPress, Keys.F4) },
                { ActionType.DebugTogglePathfindingGrid, new KeyEvent(EventType.OnButtonPress, Keys.F6) },
                { ActionType.DebugToggleCollisionGrid, new KeyEvent(EventType.OnButtonPress, Keys.F7) },
                { ActionType.PressKeyA, new KeyEvent(EventType.OnButtonPress, Keys.A) },
                { ActionType.PressKeyB, new KeyEvent(EventType.OnButtonPress, Keys.B) },
                { ActionType.PressKeyC, new KeyEvent(EventType.OnButtonPress, Keys.C) },
                { ActionType.PressKeyD, new KeyEvent(EventType.OnButtonPress, Keys.D) },
                { ActionType.PressKeyE, new KeyEvent(EventType.OnButtonPress, Keys.E) },
                { ActionType.PressKeyF, new KeyEvent(EventType.OnButtonPress, Keys.F) },
                { ActionType.PressKeyG, new KeyEvent(EventType.OnButtonPress, Keys.G) },
                { ActionType.PressKeyH, new KeyEvent(EventType.OnButtonPress, Keys.H) },
                { ActionType.PressKeyI, new KeyEvent(EventType.OnButtonPress, Keys.I) },
                { ActionType.PressKeyJ, new KeyEvent(EventType.OnButtonPress, Keys.J) },
                { ActionType.PressKeyK, new KeyEvent(EventType.OnButtonPress, Keys.K) },
                { ActionType.PressKeyL, new KeyEvent(EventType.OnButtonPress, Keys.L) },
                { ActionType.PressKeyM, new KeyEvent(EventType.OnButtonPress, Keys.M) },
                { ActionType.PressKeyN, new KeyEvent(EventType.OnButtonPress, Keys.N) },
                { ActionType.PressKeyO, new KeyEvent(EventType.OnButtonPress, Keys.O) },
                { ActionType.PressKeyP, new KeyEvent(EventType.OnButtonPress, Keys.P) },
                { ActionType.PressKeyQ, new KeyEvent(EventType.OnButtonPress, Keys.Q) },
                { ActionType.PressKeyR, new KeyEvent(EventType.OnButtonPress, Keys.R) },
                { ActionType.PressKeyS, new KeyEvent(EventType.OnButtonPress, Keys.S) },
                { ActionType.PressKeyT, new KeyEvent(EventType.OnButtonPress, Keys.T) },
                { ActionType.PressKeyU, new KeyEvent(EventType.OnButtonPress, Keys.U) },
                { ActionType.PressKeyV, new KeyEvent(EventType.OnButtonPress, Keys.V) },
                { ActionType.PressKeyW, new KeyEvent(EventType.OnButtonPress, Keys.W) },
                { ActionType.PressKeyX, new KeyEvent(EventType.OnButtonPress, Keys.X) },
                { ActionType.PressKeyY, new KeyEvent(EventType.OnButtonPress, Keys.Y) },
                { ActionType.PressKeyZ, new KeyEvent(EventType.OnButtonPress, Keys.Z) },
                { ActionType.PressKeyBackspace, new KeyEvent(EventType.OnButtonPress, Keys.Back) },
                { ActionType.PressKeyTab, new KeyEvent(EventType.OnButtonPress, Keys.Tab) },
                { ActionType.PressKeyEnter, new KeyEvent(EventType.OnButtonPress, Keys.Enter) },
                { ActionType.PressKeySpace, new KeyEvent(EventType.OnButtonPress, Keys.Space) },
                { ActionType.PressKeyCapsLock, new KeyEvent(EventType.OnButtonPress, Keys.CapsLock) },
                { ActionType.PressKeyEsc, new KeyEvent(EventType.OnButtonPress, Keys.Escape) },
                { ActionType.PressKeyF1, new KeyEvent(EventType.OnButtonPress, Keys.F1) },
                { ActionType.PressKeyF2, new KeyEvent(EventType.OnButtonPress, Keys.F2) },
                { ActionType.PressKeyF3, new KeyEvent(EventType.OnButtonPress, Keys.F3) },
                { ActionType.PressKeyF4, new KeyEvent(EventType.OnButtonPress, Keys.F4) },
                { ActionType.PressKeyF5, new KeyEvent(EventType.OnButtonPress, Keys.F5) },
                { ActionType.PressKeyF6, new KeyEvent(EventType.OnButtonPress, Keys.F6) },
                { ActionType.PressKeyF7, new KeyEvent(EventType.OnButtonPress, Keys.F7) },
                { ActionType.PressKeyF8, new KeyEvent(EventType.OnButtonPress, Keys.F8) },
                { ActionType.PressKeyF9, new KeyEvent(EventType.OnButtonPress, Keys.F9) },
                { ActionType.PressKeyF10, new KeyEvent(EventType.OnButtonPress, Keys.F10) },
                { ActionType.PressKeyF11, new KeyEvent(EventType.OnButtonPress, Keys.F11) },
                { ActionType.PressKeyF12, new KeyEvent(EventType.OnButtonPress, Keys.F12) },
                { ActionType.PressKeyPrintScreen, new KeyEvent(EventType.OnButtonPress, Keys.PrintScreen) },
                { ActionType.PressKeyScrollLock, new KeyEvent(EventType.OnButtonPress, Keys.Scroll) },
                { ActionType.PressKeyPause, new KeyEvent(EventType.OnButtonPress, Keys.Pause) },
                { ActionType.PressKeyInsert, new KeyEvent(EventType.OnButtonPress, Keys.Insert) },
                { ActionType.PressKeyHome, new KeyEvent(EventType.OnButtonPress, Keys.Home) },
                { ActionType.PressKeyPageUp, new KeyEvent(EventType.OnButtonPress, Keys.PageUp) },
                { ActionType.PressKeyDelete, new KeyEvent(EventType.OnButtonPress, Keys.Delete) },
                { ActionType.PressKeyEnd, new KeyEvent(EventType.OnButtonPress, Keys.End) },
                { ActionType.PressKeyPageDown, new KeyEvent(EventType.OnButtonPress, Keys.PageDown) },
                { ActionType.PressKeyArrowUp, new KeyEvent(EventType.OnButtonPress, Keys.Up) },
                { ActionType.PressKeyArrowDown, new KeyEvent(EventType.OnButtonPress, Keys.Down) },
                { ActionType.PressKeyArrowLeft, new KeyEvent(EventType.OnButtonPress, Keys.Left) },
                { ActionType.PressKeyArrowRight, new KeyEvent(EventType.OnButtonPress, Keys.Right) },
                { ActionType.PressKeyNumLock, new KeyEvent(EventType.OnButtonPress, Keys.NumLock) },
                { ActionType.PressKeyNumPad0, new KeyEvent(EventType.OnButtonPress, Keys.NumPad0) },
                { ActionType.PressKeyNumPad1, new KeyEvent(EventType.OnButtonPress, Keys.NumPad1) },
                { ActionType.PressKeyNumPad2, new KeyEvent(EventType.OnButtonPress, Keys.NumPad2) },
                { ActionType.PressKeyNumPad3, new KeyEvent(EventType.OnButtonPress, Keys.NumPad3) },
                { ActionType.PressKeyNumPad4, new KeyEvent(EventType.OnButtonPress, Keys.NumPad4) },
                { ActionType.PressKeyNumPad5, new KeyEvent(EventType.OnButtonPress, Keys.NumPad5) },
                { ActionType.PressKeyNumPad6, new KeyEvent(EventType.OnButtonPress, Keys.NumPad6) },
                { ActionType.PressKeyNumPad7, new KeyEvent(EventType.OnButtonPress, Keys.NumPad7) },
                { ActionType.PressKeyNumPad8, new KeyEvent(EventType.OnButtonPress, Keys.NumPad8) },
                { ActionType.PressKeyNumPad9, new KeyEvent(EventType.OnButtonPress, Keys.NumPad9) },
                { ActionType.PressKeyNumPadAdd, new KeyEvent(EventType.OnButtonPress, Keys.Add) },
                { ActionType.PressKeyNumPadSubtract, new KeyEvent(EventType.OnButtonPress, Keys.Subtract) },
                { ActionType.PressKeyNumPadMultiply, new KeyEvent(EventType.OnButtonPress, Keys.Multiply) },
                { ActionType.PressKeyNumPadDivide, new KeyEvent(EventType.OnButtonPress, Keys.Divide) },
                { ActionType.PressKeyNumPadDecimal, new KeyEvent(EventType.OnButtonPress, Keys.Decimal) },
                { ActionType.PressKeyEnterNumPad, new KeyEvent(EventType.OnButtonPress, Keys.Enter) }
            };
        public GameData()
        {
            mUnitPlayer = new Dictionary<string, int>();
            mHealth = new Dictionary<string, int>();
            mObjectPosition = new Dictionary<string, Vector2>();
            mGameObjects = new Dictionary<string, string>();
            mMapRectangles = new Dictionary<string, Rectangle>();
            mCompletionTimers = new Dictionary<string, int>();
            mBuildStates = new Dictionary<string, int>();
            mWoodGeneration = new Dictionary<string, int>();
            mStoneGeneration = new Dictionary<string, int>();
            mLevel = new Dictionary<string, int>();
            mUpgradeCost = new Dictionary<string, Vector2>();
            mTrainingQueues = new Dictionary<string, Queue<TroopType>>();
            mCurrentTrainingTimes = new Dictionary<string, float>();

            //Units
            mIsMoving = new Dictionary<string, bool>();
            mIsColliding = new Dictionary<string, bool>();
            mIsDead = new Dictionary<string, bool>();
            mJustStartedMoving = new Dictionary<string, bool>();
            mUnitDestination = new Dictionary<string, Vector2>();
            mPaths = new Dictionary<string, List<Vector2>>();
            mGoals = new Dictionary<string, Vector2>();
            mPathingTo = new Dictionary<string, Vector2>();
            mCurrentFrame = new Dictionary<string, int>();
            mTimePastSinceLastFrame = new Dictionary<string, float>();
            mAction = new Dictionary<string, int>();
            mAttacked = new Dictionary<string, bool>();
            mFlipTexture = new Dictionary<string, bool>();
            mIsMovingInQueue = new Dictionary<string, bool>();
            //Worker
            mIsBuilding = new Dictionary<string, bool>();
            mIsMovingToBuild = new Dictionary<string, bool>();
            mBuildingBeingBuilt = new Dictionary<string, string>();
            //Task
            mTasks = new Dictionary<int, string>();
            mIsTaskDone = new Dictionary<string, bool>();
            mIsNewTask = new Dictionary<string, bool>();
            mSelf = new Dictionary<string, string>();
            mOther = new Dictionary<string, string>();
            mType = new Dictionary<string, TaskType>();
            mDestination = new Dictionary<string, Vector2>();
            mRange = new Dictionary<string, int>();
            mNextTask = new Dictionary<string, TaskType>(); 
            mTaskGoal = new Dictionary<string, Vector2>(); 
            mTaskStuck = new Dictionary<string, int>(); 
            mTaskCantReachTarget = new Dictionary<string, int>(); 
            mTaskDivision = new Dictionary<string, int>(); 
            mTaskIsMetaTask = new Dictionary<string, bool>(); 

            //Move
            mMoveGameObjects = new Dictionary<int, string>();
            mMoveTarget = new Dictionary<int, Vector2>();
            mMoveMoveType = new Dictionary<int, int>();

            //AI
            mDivisionSizes = new Dictionary<int, int>();

            mWoodCount = 350;
            mStoneCount = 350;
            mAIStoneCount = 350;
            mAIWoodCount = 350;
            mMovesCount = 0;
            mNextMovesCount = 0;
            mNewGame = true;
            mCameraPosition = new Vector2(100, 500);
        }
    }
}
