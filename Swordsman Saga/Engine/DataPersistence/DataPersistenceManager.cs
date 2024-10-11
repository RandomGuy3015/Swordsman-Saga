using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.GameElements.GameObjects.Buildings;
using Swordsman_Saga.GameElements.Screens.HUDs;
using Task = Swordsman_Saga.Engine.DataTypes.Task;


namespace Swordsman_Saga.Engine.DataPersistence
{
    class DataPersistenceManager
    {
        private string mFileName = "data.game";
        public GameData mGameData;
        private StatisticsData mStatisticsData;
        private AchievementsData mAchievementsData;
        private KeyBindingData mKeyBindingData;
        private SoundData mSoundData;
        private List<IDataPersistence> mDataPersistenceObjects;
        private Camera mCamera;
        private List<IGameObject> mGameObjects;
        private AIHandler mAIHandler;
        private ObjectHandler mObjectHandler;
        private StatisticsManager mStatisticsManager;
        private AchievementsManager mAchievementsManager;
        private SoundManager mSoundManager;
        private TownHall mEnemyTownHall;
        private KeybindingManager mKeybindmanager;
        private FileDataHandler mFileDataHandler;
        private string mSelectedProfileId = "";
        public static DataPersistenceManager Instance { get; private set; }
        public FightManager mFightManager;
        public DataPersistenceManager(KeybindingManager keybindmanager, FightManager fightManager, StatisticsManager statisticsManager, AchievementsManager achievementsManager, SoundManager soundManager)
        {
            mFightManager = fightManager;
            mStatisticsManager = statisticsManager;
            mAchievementsManager = achievementsManager;
            mSoundManager = soundManager;
            mKeybindmanager = keybindmanager;
            Instance = this;
            mDataPersistenceObjects = new List<IDataPersistence>();
            mFileDataHandler = new FileDataHandler(Path.Combine(Directory.GetCurrentDirectory(), "persistentData"), mFileName);
            mFileDataHandler.Save(mGameData, mSelectedProfileId);
        }

        public void LoadMainMenu()
        {
            mStatisticsData = mFileDataHandler.Load<StatisticsData>();
            if (mStatisticsData == null)
            {
                mStatisticsData = new StatisticsData();
            }

            mAchievementsData = mFileDataHandler.Load<AchievementsData>();
            if (mAchievementsData == null)
            {
                mAchievementsData = new AchievementsData();
            }
            mKeyBindingData = mFileDataHandler.Load<KeyBindingData>();
            if (mKeyBindingData == null)
            {
                mKeyBindingData = new KeyBindingData();
            }
            mSoundData = mFileDataHandler.Load<SoundData>();
            if (mSoundData == null)
            {
                mSoundData = new SoundData();
            }
            mSoundManager.Load(mSoundData);
            mKeybindmanager.LoadData(mKeyBindingData);
            mStatisticsManager.Load(mStatisticsData);
            mAchievementsManager.Load(mAchievementsData);
        }

        public void SaveMainMenu()
        {
            mStatisticsManager.Save(ref mStatisticsData);
            mAchievementsManager.Save(ref mAchievementsData);
            mKeybindmanager.SaveData(ref mKeyBindingData);
            mSoundManager.Save(ref mSoundData);
            mFileDataHandler.Save<SoundData>(mSoundData);
            mFileDataHandler.Save<StatisticsData>(mStatisticsData);
            mFileDataHandler.Save<AchievementsData>(mAchievementsData);
            mFileDataHandler.Save<KeyBindingData>(mKeyBindingData);
        }
        public void Update(Dictionary<string, IGameObject> gameObjects, Camera camera, AIHandler aiHandler, FightManager fightManager, ObjectHandler objectHandler, SoundManager soundManager)
        {
            mDataPersistenceObjects = FindAllDataPersistenceObjects(gameObjects);
            mCamera = camera;
            mFightManager = fightManager;
            mAIHandler = aiHandler;
            mObjectHandler = objectHandler;
            mSoundManager = soundManager;
        }
        public void NewGame()
        {
            mGameData = new GameData();
        }

        public void SaveGame()
        {
            mGameData = new GameData();
            mGameData.mNewGame = false;
            foreach (var dataPersistenceObject in mDataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(ref mGameData);
            }
            mCamera.SaveData(ref mGameData);
            mAIHandler.SaveData(ref mGameData);
            mObjectHandler.SaveData(ref mGameData);
            ResourceHud.Instance.SaveData(ref mGameData);
            if (mSelectedProfileId == "-1")
            {
                mSelectedProfileId = "1";
            }
            mFileDataHandler.Save(mGameData, mSelectedProfileId);
        }

        public List<IGameObject> LoadGame(bool loadAi)
        {
            mGameData = mFileDataHandler.Load(mSelectedProfileId);
            mGameObjects = new List <IGameObject>();
            if (mGameData == null)
            {
                NewGame();
            }
            else
            {
                foreach (var pair in mGameData.mGameObjects)
                {
                    switch (pair.Value)
                    {
                        case "Swordsman":
                            Swordsman swordsman = new Swordsman(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mUnitPlayer[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager, mStatisticsManager);
                            swordsman.Health = mGameData.mHealth[pair.Key];
                            swordsman.IsMoving = mGameData.mIsMoving[pair.Key];
                            swordsman.IsColliding = mGameData.mIsColliding[pair.Key];
                            swordsman.IsDead = mGameData.mIsDead[pair.Key];
                            swordsman.JustStartedMoving = mGameData.mJustStartedMoving[pair.Key];
                            swordsman.Destination = mGameData.mUnitDestination[pair.Key];
                            swordsman.Path = mGameData.mPaths[pair.Key];
                            swordsman.Goal = mGameData.mGoals[pair.Key];
                            swordsman.PathingTo = mGameData.mPathingTo[pair.Key];
                            swordsman.CurrentFrame = mGameData.mCurrentFrame[pair.Key];
                            swordsman.TimePassedSinceLastFrame = mGameData.mTimePastSinceLastFrame[pair.Key];
                            swordsman.Action = mGameData.mAction[pair.Key];
                            swordsman.Attacked = mGameData.mAttacked[pair.Key];
                            swordsman.FlipTexture = mGameData.mFlipTexture[pair.Key];
                            swordsman.IsMovingInQueue = mGameData.mIsMovingInQueue[pair.Key];
                            swordsman.Sound = mSoundManager;
                            mGameObjects.Add(swordsman);
                            break;
                        case "Archer":
                            Archer archer = new Archer(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mUnitPlayer[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager, mStatisticsManager);
                            archer.Health = mGameData.mHealth[pair.Key];
                            archer.IsMoving = mGameData.mIsMoving[pair.Key];
                            archer.IsColliding = mGameData.mIsColliding[pair.Key];
                            archer.IsDead = mGameData.mIsDead[pair.Key];
                            archer.JustStartedMoving = mGameData.mJustStartedMoving[pair.Key];
                            archer.Destination = mGameData.mUnitDestination[pair.Key];
                            archer.Path = mGameData.mPaths[pair.Key];
                            archer.Goal = mGameData.mGoals[pair.Key];
                            archer.PathingTo = mGameData.mPathingTo[pair.Key];
                            archer.CurrentFrame = mGameData.mCurrentFrame[pair.Key];
                            archer.TimePassedSinceLastFrame = mGameData.mTimePastSinceLastFrame[pair.Key];
                            archer.Action = mGameData.mAction[pair.Key];
                            archer.Attacked = mGameData.mAttacked[pair.Key];
                            archer.FlipTexture = mGameData.mFlipTexture[pair.Key];
                            archer.Sound = mSoundManager;
                            mGameObjects.Add(archer);
                            break;
                        case "Knight":
                            Knight knight = new Knight(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mUnitPlayer[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager, mStatisticsManager);
                            knight.Health = mGameData.mHealth[pair.Key];
                            knight.IsMoving = mGameData.mIsMoving[pair.Key];
                            knight.IsColliding = mGameData.mIsColliding[pair.Key];
                            knight.IsDead = mGameData.mIsDead[pair.Key];
                            knight.JustStartedMoving = mGameData.mJustStartedMoving[pair.Key];
                            knight.Destination = mGameData.mUnitDestination[pair.Key];
                            knight.Path = mGameData.mPaths[pair.Key];
                            knight.Goal = mGameData.mGoals[pair.Key];
                            knight.PathingTo = mGameData.mPathingTo[pair.Key];
                            knight.CurrentFrame = mGameData.mCurrentFrame[pair.Key];
                            knight.TimePassedSinceLastFrame = mGameData.mTimePastSinceLastFrame[pair.Key];
                            knight.Action = mGameData.mAction[pair.Key];
                            knight.Attacked = mGameData.mAttacked[pair.Key];
                            knight.FlipTexture = mGameData.mFlipTexture[pair.Key];
                            knight.Sound = mSoundManager;
                            mGameObjects.Add(knight);
                            break;
                        case "Worker":
                            Worker worker = new Worker(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mUnitPlayer[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager, mStatisticsManager);
                            worker.Health = mGameData.mHealth[pair.Key];
                            worker.IsMoving = mGameData.mIsMoving[pair.Key];
                            worker.IsColliding = mGameData.mIsColliding[pair.Key];
                            worker.IsDead = mGameData.mIsDead[pair.Key];
                            worker.Destination = mGameData.mUnitDestination[pair.Key];
                            worker.JustStartedMoving = mGameData.mJustStartedMoving[pair.Key];
                            worker.Path = mGameData.mPaths[pair.Key];
                            worker.Goal = mGameData.mGoals[pair.Key];
                            worker.PathingTo = mGameData.mPathingTo[pair.Key];
                            worker.CurrentFrame = mGameData.mCurrentFrame[pair.Key];
                            worker.TimePassedSinceLastFrame = mGameData.mTimePastSinceLastFrame[pair.Key];
                            worker.Action = mGameData.mAction[pair.Key];
                            worker.Attacked = mGameData.mAttacked[pair.Key];
                            worker.FlipTexture = mGameData.mFlipTexture[pair.Key];
                            worker.IsBuilding = mGameData.mIsBuilding[pair.Key];
                            worker.IsMovingToBuild = mGameData.mIsMovingToBuild[pair.Key];
                            worker.Sound = mSoundManager;
                            if (mGameData.mBuildingBeingBuilt[pair.Key] != null)
                            {
                                worker.mSearchForObject = true;
                            }
                            mGameObjects.Add(worker);
                            break;
                        case "Tree":
                            Tree tree = new Tree(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mMapRectangles[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager);
                            tree.Health = mGameData.mHealth[pair.Key];
                            tree.Sound = mSoundManager;
                            mGameObjects.Add(tree);
                            break;
                        case "Stone":
                            Stone stone = new Stone(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mMapRectangles[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager);
                            stone.Health = mGameData.mHealth[pair.Key];
                            stone.Sound = mSoundManager;
                            mGameObjects.Add(stone);
                            break;
                        case "TownHall":
                            TownHall townHall = new TownHall(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mUnitPlayer[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager);
                            townHall.Health = mGameData.mHealth[pair.Key];
                            townHall.Sound = mSoundManager;
                            mGameObjects.Add(townHall);
                            if (townHall.Team == 1)
                            {
                                mEnemyTownHall = townHall;
                            }
                            break;
                        case "Quarry":
                            Quarry quarry = new Quarry(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mUnitPlayer[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager, mStatisticsManager);
                            quarry.CompletionTimer = mGameData.mCompletionTimers[pair.Key];
                            quarry.BuildState = mGameData.mBuildStates[pair.Key];
                            quarry.StoneGeneration = mGameData.mStoneGeneration[pair.Key];
                            quarry.Level = mGameData.mLevel[pair.Key];
                            quarry.UpgradeCost = mGameData.mUpgradeCost[pair.Key];
                            quarry.ResourceHud = ResourceHud.Instance;
                            quarry.Sound = mSoundManager;
                            mGameObjects.Add(quarry);
                            break;
                        case "LumberCamp":
                            LumberCamp lumberCamp = new LumberCamp(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mUnitPlayer[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager, mStatisticsManager);
                            lumberCamp.CompletionTimer = mGameData.mCompletionTimers[pair.Key];
                            lumberCamp.BuildState = mGameData.mBuildStates[pair.Key];
                            lumberCamp.WoodGeneration = mGameData.mWoodGeneration[pair.Key];
                            lumberCamp.Level = mGameData.mLevel[pair.Key];
                            lumberCamp.UpgradeCost = mGameData.mUpgradeCost[pair.Key];
                            lumberCamp.ResourceHud = ResourceHud.Instance;
                            lumberCamp.Sound = mSoundManager;
                            mGameObjects.Add(lumberCamp);
                            break;
                        case "Barracks":
                            Barracks barracks = new Barracks(pair.Key,
                                (int)mGameData.mObjectPosition[pair.Key].X,
                                (int)mGameData.mObjectPosition[pair.Key].Y,
                                mGameData.mUnitPlayer[pair.Key],
                                DynamicContentManager.Instance,
                                mFightManager);
                            barracks.CompletionTimer = mGameData.mCompletionTimers[pair.Key];
                            barracks.BuildState = mGameData.mBuildStates[pair.Key];
                            barracks.TroopSelection = TroopSelectionOverlay.Instance;
                            barracks.mTrainingQueue = mGameData.mTrainingQueues[pair.Key];
                            barracks.mCurrentTrainingTime = mGameData.mCurrentTrainingTimes[pair.Key];
                            barracks.ResourceHud = ResourceHud.Instance;
                            barracks.Sound = mSoundManager;
                            mGameObjects.Add(barracks);
                            break;
                    }
                }
            }

            foreach (var gameObject in mGameObjects)
            {
                if (gameObject is Worker worker)
                {
                    if (worker.mSearchForObject)
                    {
                        SetBuildingWorker(worker);
                    }
                }
            }
            mCamera.LoadData(mGameData);
            mObjectHandler.LoadData(mGameData);
            // Musste das reinmachen weil das versucht die ganze Zeit beim neuen Spiel die Townhall der KI mit null zu ueberschreieben.
            if (loadAi)
            {
                if (mEnemyTownHall != null)
                {
                    mAIHandler.LoadData(mGameData, mEnemyTownHall);
                }
            }
            ResourceHud.Instance.LoadData(mGameData);
            return mGameObjects;
        }

        public void SetBuildingWorker(Worker worker)
        {
            foreach (var gameObject in mGameObjects)
            {
                if (gameObject.Id == mGameData.mBuildingBeingBuilt[worker.Id])
                {
                    worker.BuildingBeingBuilt = (IBuilding)gameObject;
                    Debug.WriteLine("Team: "+worker.Team);
                    Debug.WriteLine(Object.ReferenceEquals(worker.BuildingBeingBuilt, gameObject));
                }
            }
        }
        public void SetSelfMove(Move move)
        {
            foreach (var gameObject in mGameObjects)
            {
                if (gameObject.Id == move.ID)
                {
                    move.Self = gameObject;
                }
            }
        }
        public void SetSelfTask(Task task)
        {
            foreach (var gameObject in mGameObjects)
            {
                if (gameObject.Id == mGameData.mSelf[task.Id])
                {
                    task.Self = gameObject;
                }
            }
        }

        public void SetOtherTask(Task task)
        {
            foreach (var gameObject in mGameObjects)
            {
                if (gameObject.Id == mGameData.mOther[task.Id])
                {
                    task.Other = gameObject;
                }
            }
        }
        private List<IDataPersistence> FindAllDataPersistenceObjects(Dictionary<string, IGameObject> gameObjects)
        {
            List<IDataPersistence> dataPersistenceObjects = new List<IDataPersistence>();
            foreach (var gameObject in gameObjects)
            {
                if (gameObject.Value is IDataPersistence dataPersistenceObject)
                {
                    dataPersistenceObjects.Add(dataPersistenceObject);
                }
            }
            return dataPersistenceObjects;
        }

        public Dictionary<string, GameData> GetAllProfilesGameData()
        {
            return mFileDataHandler.LoadAllProfiles();
        }

        public void SetSelectedProfileId(string profileId)
        {
            mSelectedProfileId = profileId;
        }
    }
}
