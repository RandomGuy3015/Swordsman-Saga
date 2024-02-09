using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Swordsman_Saga.Engine.DataTypes.Grids;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.GameElements.GameObjects.Buildings;
using Swordsman_Saga.GameElements.GameObjects.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Swordsman_Saga.GameElements.Screens.HUDs;
using Newtonsoft.Json.Bson;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;
using static Swordsman_Saga.GameElements.Screens.HUDs.TroopSelectionOverlay;
using static Swordsman_Saga.Engine.ObjectManagement.IUnit;
using System.Collections;
using System.Reflection.Metadata;

namespace Swordsman_Saga.Engine.DataTypes
{
    public enum TaskType
    {
        BuildUnit, // implemented
        BuildBuilding, // implemented
        AttackMapObject, // implemented
        AttackMoveToArea, // implemented
        AttackMoveToUnit,
        AttackEnemyUnit,
        AttackEnemyBuilding,
        AttackArea, // implemented
        Explore,
        MoveTowardsEnemyBase, // implemented
        GetDivisionAction, // implemented
        FormUpWithGroup, // implemented
        RegroupAtHome, // implemented
        Flee, // implemented
        Flank,
    }

    internal class Task
    {
        public string Id { get; set; }
        // If the task is done a new one will be assigned
        public bool IsTaskDone { get; set; }
        public bool IsNewTask { get; set; }

        // The ally assigned to the task
        public IGameObject Self;

        // The target object
        public IGameObject Other;

        // The Task itself
        public TaskType Type;

        // Sometimes one task has to be done before you can continue the old one
        public TaskType NextTask;

        // Where to move to
        public Vector2 Goal;

        // How far away counts as ok
        public int Range;

        // If the unit is stuck. > 2 mean stuck
        public int Stuck;

        // If the unit has an inaccesible goal
        public int CantReachTarget;

        // The army the unit belongs to. -1 is none
        public int Division;

        // Is this a meta-task
        public bool IsMetaTask;




        // #############################   CONSTRUCTORS   ###############################

        // The constructor for standard tasks, that consist of a unit and an action
        public Task(IGameObject gameObject)
        {
            Id = System.Guid.NewGuid().ToString();
            Self = gameObject;
            Other = null;
            FindNewTask();
            IsTaskDone = false;
            Division = -1;
            Stuck = 0;
            CantReachTarget = 0;
            IsMetaTask = false;
        }

        // The constructor for meta tasks, that control many other tasks
        public Task(IGameObject gameObject, int division)
        {
            Id = System.Guid.NewGuid().ToString();

            // This gameObject is the 'leader' of the division, he influences some behaviours
            Self = gameObject;
            Other = null;

            IsNewTask = true;
            IsTaskDone = false;

            Division = division;
            IsMetaTask = true;
            Type = TaskType.FormUpWithGroup;
        }

        public Task()
        {
        }


        // ################################   FIND TASK   ##################################
        public void FindNewTask()
        {
            IsNewTask = true;
            IsTaskDone = false;

            if (Self is Worker)
            {
                Type = TaskType.BuildBuilding;
            }
            else if (Self is Barracks)
            {
                Type = TaskType.BuildUnit;
            }
            else if (Division > -1)
            {
                Type = TaskType.GetDivisionAction;
            }
            else if (Self is Swordsman)
            {
                Type = TaskType.MoveTowardsEnemyBase;
            }
            else if (Self is Archer)
            {
                Type = TaskType.MoveTowardsEnemyBase;
            }
            else if (Self is Knight)
            {
                Type = TaskType.MoveTowardsEnemyBase;
            }
        }

        public void SaveData(ref GameData data, int count)
        {
            data.mTasks.Add(count, Id);
            data.mIsTaskDone.Add(Id, IsTaskDone);
            data.mIsNewTask.Add(Id, IsNewTask);
            data.mSelf.Add(Id, Self.Id);
            if (Other != null)
            {
                data.mOther.Add(Id, Other.Id);
            }
            else
            {
                data.mOther.Add(Id, null);
            }
            data.mType.Add(Id, Type);
            data.mDestination.Add(Id, Goal);
            data.mRange.Add(Id, Range);
            data.mNextTask.Add(Id, NextTask);
            data.mTaskGoal.Add(Id, Goal);
            data.mTaskStuck.Add(Id, Stuck);
            data.mTaskCantReachTarget.Add(Id, CantReachTarget);
            data.mTaskDivision.Add(Id, Division);
            data.mTaskIsMetaTask.Add(Id, IsMetaTask);
        }
        // ######################  ASSIGN OR RE-ASSIGN SPECIFIC TASK   ###################

        public void AssignTask(TaskType task)
        {
            Type = task;
            IsNewTask = true;
        }
    }


    // *******************************************************************************************************************
    // ##############################################    HANDLER - LOOK HERE   ###########################################
    // *******************************************************************************************************************
    internal class AIHandler: IDataPersistence
    {
        // [TO-SERIALIZE]
        // The Townhall to defend
        private TownHall mTownHall;

        // [TO-SERIALIZE]
        // A list of allied objects and their tasks
        private List<Task> mTasks;

        // Don't serialize.
        private List<Task> mNewTasks;

        // [TO-SERIALIZE]
        // The ratio of wood to stone.
        private float mWoodToStoneRatio;

        // [TO-SERIALIZE]
        // The amount of Barracks. Gets updated every time a task finishes.
        private int mBarracksBuilt;

        // [TO-SERIALIZE]
        // The amount of buildings built. Useful to tell which stage of the game we're in.
        private int mBuildingsBuilt;

        // [TO-SERIALIZE]
        // Meta-actions for what divisions of unit should do.
        // key: Division  (0 - 8)
        // value: Action  Task.Type(FormUpWithGroup, Flank, DefendArea, DefendAttack, MoveTowardsEnemyBase, AttackArea)
        private Dictionary<int, Task> mDivisionMapping;

        // [TO-SERIALIZE]
        // The time the game started. Doesn't reset when loading.
        private TimeSpan mGameStartTime;

        // [TO-SERIALIZE]
        // The difficulty selected
        private int mDifficulty;

        // The size of each individual division
        private Dictionary<int, int> mDivisionSizes;

        // This is used to prevent the AI from always building the cheapest option when low on resources.
        private int mAreResourcesReserved;

        // This flag disables the AI.
        public bool IsDebug;

        // The game state. Depends on time elapsed.
        private int mGameState;

        // The worker that the AI starts out with.
        private bool mFoundStarterWorker;

        // The random int creator. All hail.
        private Random mRandom;


        // Helper classes
        readonly private DiamondGrid mGrid;
        readonly private FightManager mFightManager;
        readonly private StatisticsManager mStatisticsManager;
        readonly private ObjectHandler mObjectHandler;
        readonly private DynamicContentManager mContentManager;
        readonly private ResourceHud mResourceHud;
        readonly private BuildingSelectionOverlay mBuildingSelectionOverlay;


        public AIHandler(DiamondGrid grid, FightManager fightManager, StatisticsManager statisticsManager,ObjectHandler objectHandler, DynamicContentManager contentManager, TownHall townHall, ResourceHud resourceHud, BuildingSelectionOverlay buildingSelectionOverlay, int difficulty)
        {
            mGrid = grid;
            mFightManager = fightManager;
            mStatisticsManager = statisticsManager;
            mObjectHandler = objectHandler;
            mContentManager = contentManager;
            mTownHall = townHall;
            mResourceHud = resourceHud;
            mBuildingSelectionOverlay = buildingSelectionOverlay;
            mDifficulty = difficulty;

            mFoundStarterWorker = false;
            IsDebug = false;
            mAreResourcesReserved = -1;
            mWoodToStoneRatio = .5f;
            mBarracksBuilt = 0;
            mBuildingsBuilt = 0;
            mGameState = 0;

            mTasks = new List<Task>();
            mNewTasks = new List<Task>();
            mDivisionMapping = new Dictionary<int, Task>();
            mDivisionSizes = new Dictionary<int, int>();
            mRandom = new ();
        }

        public void Update(GameTime gametime)
        {
            if (IsDebug) { return; }

            if (!mFoundStarterWorker)
            {
                FindStarterWorker();
                Load();
                mGameStartTime = gametime.TotalGameTime;
                
                if (mDifficulty == -1)
                {
                    throw new Exception("Difficulty was not properly loaded!");
                }
            }

            foreach (Task task in mTasks)
            {
                if (task.IsTaskDone)
                {
                    task.FindNewTask();
                    mGameState = (int) (gametime.TotalGameTime.TotalMinutes - mGameStartTime.TotalMinutes);
                }
                FulfillTask(task);
            }

            foreach (Task task in mNewTasks)
            {
                mTasks.Add(task);
            }

            mNewTasks.Clear();

        }

        public void SaveData(ref GameData data)
        {
            data.mWoodToStoneRatio = mWoodToStoneRatio;
            data.mBuildingsBuild = mBuildingsBuilt;
            data.mTimeSinceGameStart = mGameStartTime;
            data.mBarracksBuilt = mBarracksBuilt;
            data.mDivisionSizes = mDivisionSizes;
            int count = 0;
            foreach (var pair in mDivisionMapping)
            {
                pair.Value.SaveData(ref data, count);
                count++;
            }
            data.mDivisionCount = count;
            foreach (Task task in mTasks)
            {
                task.SaveData(ref data, count);
                count++;
            }
        }

        public void LoadData(GameData gameData, TownHall townHall)
        {
            mTownHall = townHall;
            mWoodToStoneRatio = gameData.mWoodToStoneRatio;
            mBuildingsBuilt = gameData.mBuildingsBuild;
            mBarracksBuilt = gameData.mBarracksBuilt;
            mDivisionSizes = gameData.mDivisionSizes;
            mGameStartTime = gameData.mTimeSinceGameStart;
            mDivisionMapping = LoadDivisions(gameData);
            mTasks = LoadTasks(gameData);
            Load();
        }

        private List<Task> LoadTasks(GameData gameData)
        {
            List<Task> tasks = new List<Task>();
            for (int i = gameData.mDivisionCount; i < gameData.mType.Count; i++)
            {
                string id = gameData.mTasks[i];
                Task task = new Task();
                task.Id = id;
                task.Goal = gameData.mDestination[id];
                task.IsNewTask = gameData.mIsNewTask[id];
                task.IsTaskDone = gameData.mIsTaskDone[id];
                task.Range = gameData.mRange[id];
                task.Type = gameData.mType[id];
                task.NextTask = gameData.mNextTask[id];
                task.Goal = gameData.mTaskGoal[id];
                task.Stuck = gameData.mTaskStuck[id];
                task.CantReachTarget = gameData.mTaskCantReachTarget[id];
                task.Division = gameData.mTaskDivision[id];
                task.IsMetaTask = gameData.mTaskIsMetaTask[id];
                /*
                if (gameData.mOther[id] != null)
                {
                    task.Other = DataPersistenceManager.Instance.FindObject(gameData.mOther[id]);
                }*/
                task.Self = DataPersistenceManager.Instance.FindObject(gameData.mSelf[id]);
                tasks.Add(task);
            }
            return tasks;
        }

        private Dictionary<int, Task> LoadDivisions(GameData gameData)
        {
            Dictionary<int, Task> divisions = new Dictionary<int, Task>();
            for (int i = 0; i < gameData.mDivisionCount; i++)
            {
                string id = gameData.mTasks[i];
                Task task = new Task();
                task.Id = id;
                task.Goal = gameData.mDestination[id];
                task.IsNewTask = gameData.mIsNewTask[id];
                task.IsTaskDone = gameData.mIsTaskDone[id];
                task.Range = gameData.mRange[id];
                task.Type = gameData.mType[id];
                task.NextTask = gameData.mNextTask[id];
                task.Goal = gameData.mTaskGoal[id];
                task.Stuck = gameData.mTaskStuck[id];
                task.CantReachTarget = gameData.mTaskCantReachTarget[id];
                task.Division = gameData.mTaskDivision[id];
                task.IsMetaTask = gameData.mTaskIsMetaTask[id];
                task.Self = DataPersistenceManager.Instance.FindObject(gameData.mSelf[id]);
                /*
                if (gameData.mOther[id] != null)
                {
                    task.Other = DataPersistenceManager.Instance.FindObject(gameData.mOther[id]);
                }*/
                divisions.Add(task.Division, task);
            }
            return divisions;
        }

        /// <summary>
        /// Call this method when loading the AI from save. This will update all the variables that aren't consistantly updated.
        /// Only call after the serialized variables are loaded!
        /// </summary>
        public void Load()
        {
            if (mTasks.Count > 0)
            {
                mFoundStarterWorker = true;
            }
        }



        // *******************************************************************************************************************
        // ###############################################   FULFILL TASK   ##################################################
        // *******************************************************************************************************************


        private void FulfillTask(Task task)
        {
            // Our lil buddy died. rip.
            if (((IAttackableObject)task.Self).Health < 0)
            {
                if (task.Division != -1)
                {
                    mDivisionSizes[task.Division]--;
                }
                task.Self = null;
                return;
            }

            // Our guy is still being born, or was deactivated by by the task handler for being dead.
            if (task.Self == null) { return; }

            // He's stuck in a building or tree somewhere
            if (task.CantReachTarget > 2) { return; }

            // Townhall guards
            if (task.Division == 2) { return; }

            switch (task.Type)
            {
                // ####################################   BUILD BUILDING   #########################################

                case TaskType.BuildBuilding:

                    // Building logic
                    Worker worker = task.Self as Worker;
                    if (task.IsNewTask)
                    {
                        if (mResourceHud.AIStoneCount < mBuildingSelectionOverlay.GetBuildingCost(0).X || mResourceHud.AIWoodCount < mBuildingSelectionOverlay.GetBuildingCost(1).Y)
                        {
                            mAreResourcesReserved = 3;
                            break;
                        }


                        Diamond snappedToGridDiamond = mGrid.GetGridDiamondFromPixel(GetNearestResource(worker.Position));
                        int maxTries = 5;
                        while (maxTries > 0 && ((IMovingObject)task.Self).FindLengthOfPath() > Vector2.Distance(worker.Position, new Vector2(snappedToGridDiamond.X, snappedToGridDiamond.Y)) + 200d)
                        {
                            snappedToGridDiamond = mGrid.GetGridDiamondFromPixel(GetNearestResource(worker.Position + new Vector2(mRandom.Next(-200, 200), mRandom.Next(-200, 200))));
                            maxTries--;
                        }
                        task.Goal = new Vector2(snappedToGridDiamond.X, snappedToGridDiamond.Y);
                        task.Range = 100;

                        IBuilding building = null;

                        if (mGrid.GetStaticObjectFromPixel(task.Goal) is Tree && mResourceHud.UseResources(mBuildingSelectionOverlay.GetBuildingCost(1), 1))
                        {
                            building = new LumberCamp(null, (int)task.Goal.X, (int)task.Goal.Y, 1, mContentManager, mFightManager);
                            mWoodToStoneRatio += 0.6f;

                        }
                        else if (mGrid.GetStaticObjectFromPixel(task.Goal) is Stone && mResourceHud.UseResources(mBuildingSelectionOverlay.GetBuildingCost(0), 1))
                        {
                            building = new Quarry(null, (int)task.Goal.X, (int)task.Goal.Y, 1, mContentManager, mFightManager);
                            mWoodToStoneRatio -= 0.6f;

                        }
                        else if (!mGrid.CheckIfGridLocationIsFilledFromPixel(task.Goal) && mResourceHud.UseResources(mBuildingSelectionOverlay.GetBuildingCost(2), 1))
                        {
                            building = new Barracks(null, (int)task.Goal.X, (int)task.Goal.Y, 1, mContentManager, mFightManager);
                            ((Barracks)building).TroopSelection = TroopSelectionOverlay.Instance;
                            SetupBarracks((Barracks)building);
                            mBarracksBuilt++;
                        }
                        else
                        {
                            break;
                        }

                        if (mDifficulty == 0)
                        {
                            mResourceHud.UseResources(new Vector2(100, 100), 1);
                        }
                        else if (mDifficulty == 1)
                        {
                            mResourceHud.UseResources(new Vector2(50, 50), 1);
                        }

                        building.ResourceHud = mResourceHud;
                        mObjectHandler.QueueCreateOverwrite(building);
                        mBuildingsBuilt++;
                        mAreResourcesReserved = -1;

                        mObjectHandler.QueueMove(worker, task.Goal, false);

                        worker.IsMovingToBuild = true;
                        worker.BuildingBeingBuilt = building;

                        task.IsNewTask = false;
                        break;
                    }
                    if (worker.IsBuilding && worker.IsMovingToBuild && Vector2.Distance(worker.BuildingBeingBuilt.Position, worker.Position) > 100)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine($"This message shouldn't pop up. The worker is bugged.");
                        }
                        //task.IsTaskDone = true;
                        //task.AssignTask(TaskType.AttackMapObject);
                        break;
                    }
                    if (worker.IsColliding)
                    {
                        task.Stuck++;
                    }
                    if (task.Stuck > 100)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLine("AI Worker stuck in wall or something.");
                        }
                        //mObjectHandler.QueueMove(worker, worker.BuildingBeingBuilt.Position, false);
                        task.Stuck = 0;
                    }
                    if (((IBuilding)worker.BuildingBeingBuilt).BuildState == 2)
                    {
                        task.IsTaskDone = true;
                        task.Stuck = 0;
                        task.CantReachTarget = 0;

                        if (worker.BuildingBeingBuilt is Barracks)
                        {
                            mNewTasks.Add(new Task(worker.BuildingBeingBuilt));
                        }
                        break;
                    }
                    if (!worker.IsMoving && worker.Goal != worker.Position && !worker.IsBuilding)
                    {
                        task.CantReachTarget++;

                        if (task.CantReachTarget >= 15)
                        {
                            if (Debugger.IsAttached)
                            {
                                Debug.WriteLine("Worker Stuck!");
                            }
                            mObjectHandler.QueueDelete(worker.BuildingBeingBuilt);

                            mBuildingsBuilt--;
                            if (worker.BuildingBeingBuilt is Barracks) { mBarracksBuilt--; }

                            worker.BuildingBeingBuilt = null;
                            worker.IsMovingToBuild = false;

                            mResourceHud.AIStoneCount += 80;
                            mResourceHud.AIWoodCount += 80;

                            task.IsTaskDone = true;
                            task.CantReachTarget = 0;
                        }
                        break;
                    }
                    break;

                // ####################################   BUILD UNIT   #########################################

                case TaskType.BuildUnit:

                    task.IsNewTask = false;

                    Barracks barracks = task.Self as Barracks;

                    if (mAreResourcesReserved == 0) { return; }
                    if (barracks.GetRemainingTrainingTime() < .01f && mRandom.Next(1, 100) > 95)
                    {
                        TroopType toBuild = TroopType.Worker;

                        if (mRandom.Next(1, 100) < SigmoidFromGameState(50f, 1f, -0.3f, 45f))
                        {
                            toBuild = TroopType.Swordsman;
                            if (mRandom.Next(1, 100) < SigmoidFromGameState(80f, .3f, -2f, 10f))
                            {
                                toBuild = TroopType.Archer;
                            }
                            if (mRandom.Next(1, 100) < SigmoidFromGameState(80f, .6f, -6f, 0f))
                            {
                                toBuild = TroopType.Knight;
                            }
                        }

                        for (int i = 0; i < mGameState * .5 + 1; i++)
                        {
                            barracks.AddToQueue(toBuild);
                        }
                    }

                    if (mAreResourcesReserved > 0)
                    {
                        mAreResourcesReserved--;
                    }

                    break;

                // ####################################   ATTACK MAP OBJECT   ########################################

                case TaskType.AttackMapObject:

                    IUnit unit = task.Self as IUnit;
                    if (task.IsNewTask)
                    {
                        int target = mGrid.GetClosestLocThatFulfillsLambda(mGrid.TranslateToGrid(task.Self.Position), FilledBy<Tree>);

                        List<IAttackableObject> attackableObjects = mGrid.GetAttackableObjectFromSlot(target);

                        if (attackableObjects.Count != 0)
                        {
                            mFightManager.SetTargetForAttacker(unit, mGrid.GetAttackableObjectFromSlot(target)[0]);
                        }

                        mObjectHandler.QueueMove(task.Self, mGrid.TranslateFromGrid(target), false);
                        task.IsNewTask = false;
                    }
                    else
                    {
                        if (unit.Action == (int)ActionId.Standing)
                        {
                            task.IsTaskDone = true; 
                        }
                    }
                    break;

                // #####################################   ATTACK AREA   #############################################

                case TaskType.AttackArea:

                    unit = task.Self as IUnit;

                    if (task.IsNewTask)
                    {
                        var goalEnemy = mFightManager.ScanForTarget(unit, false, false);
                        if (goalEnemy == null) { task.IsTaskDone = true; break; }
                        mFightManager.SetTargetForAttacker(unit, goalEnemy);

                        task.IsNewTask = false;
                    }

                    if (unit.Health <= 30) 
                    {
                        task.AssignTask(TaskType.Flee);
                    }

                    if (unit.Action != (int)ActionId.Attacking)
                    {
                        //task.IsTaskDone = true;
                    }
                    break;

                // ################################   MOVE TOWARDS ENEMY BASE   #######################################

                case TaskType.MoveTowardsEnemyBase:

                    unit = task.Self as IUnit;

                    if (task.IsNewTask)
                    {
                        mObjectHandler.QueueMove(task.Self, new Vector2(-50, 300), false);
                        task.IsNewTask = false;
                    }

                    if (unit.Goal == unit.Position)
                    {
                        task.AssignTask(TaskType.AttackArea);
                    }

                    if (task.IsMetaTask)
                    {
                        if (mDivisionSizes[task.Division] < 2 + mGameState)
                        {
                            task.AssignTask(TaskType.RegroupAtHome);
                        }
                    }
                    else
                    {
                        if (task.Type != mDivisionMapping[task.Division].Type)
                        {
                            task.AssignTask(mDivisionMapping[task.Division].Type);
                        }
                    }

                    break;

                // #################################   ATTACK ENEMY BUILDING   ########################################

                case TaskType.AttackEnemyBuilding:

                    unit = task.Self as IUnit;

                    if (task.IsNewTask)
                    {
                        var goalEnemy = mFightManager.ScanForTarget(unit, false, true);
                        if (goalEnemy == null) { task.IsTaskDone = true; break; }
                        mFightManager.SetTargetForAttacker(unit, goalEnemy);

                        task.IsNewTask = false;
                    }

                    if (unit.Health <= 30)
                    {
                        task.AssignTask(TaskType.Flee);
                    }

                    if (unit.Action != (int)ActionId.Attacking)
                    {
                        //task.IsTaskDone = true;
                    }
                    break;

                // #########################################   FLEE   #################################################

                case TaskType.Flee:

                    unit = task.Self as IUnit;

                    if (task.IsNewTask)
                    {
                        mObjectHandler.QueueMove(task.Self, new Vector2(-50, 5050), false);
                        task.IsNewTask = false;
                    }

                    if (unit.Health > unit.MaxHealth - 20)
                    {
                        task.IsTaskDone = true;
                    }

                    break;

                // ###################################   GET DIVISION ACTION   #########################################

                case TaskType.GetDivisionAction:

                    if (task.IsNewTask)
                    {
                        task.AssignTask(mDivisionMapping[task.Division].Type);
                    }
                    break;

                // ####################################   FORM UP WITH GROUP   #########################################

                case TaskType.FormUpWithGroup:

                    if (!task.IsMetaTask)
                    {
                        if (task.IsNewTask)
                        {
                            mObjectHandler.QueueMove(task.Self, mDivisionMapping[task.Division].Goal, false);
                            task.IsNewTask = false;
                        }

                        if (mDivisionMapping[task.Division].Type != task.Type)
                        {
                            task.AssignTask(mDivisionMapping[task.Division].Type);
                        }
                    }
                    else {
                        if (task.IsNewTask)
                        {
                            task.IsNewTask = false;
                        }

                        //Debug.WriteLine($"{mDivisionSizes[task.Division] + 1}  <compared to minimum>  {((mGameState + 1) * 3 - 1)}");
                        if (mDivisionSizes[task.Division] + 1 - (mDifficulty - 2) > mRandom.Next((int)((float)(mGameState + 1) * 1.5f) - 2, 100))
                        {
                            task.AssignTask(TaskType.AttackMoveToArea);
                            task.Goal = new Vector2(-50, 300);

                            if (Debugger.IsAttached)
                            {
                                Debug.WriteLine("Attack moving towards enemy base!");
                            }
                        }
                        else if (mGameState + 10 > mRandom.Next(3, 40))
                        {
                            /*
                            if (Debugger.IsAttached)
                            {
                                Debug.WriteLine("Defending!");
                            }
                            */
                            int target = GetEnemyAttack(task.Self.Position);
                            if (target != -1)
                            {
                                if (mGrid.mGridContent.TryGetValue(target, out List<IGameObject> gridContent))
                                {
                                    foreach (IGameObject gameObject in gridContent)
                                    {
                                        if (gameObject.Team == 0 && gameObject is IUnit)
                                        {
                                            task.AssignTask(TaskType.AttackMoveToUnit);
                                            task.Other = gameObject;
                                            break;
                                        }
                                    }
                                    if (Debugger.IsAttached)
                                    {
                                        //Debug.WriteLine("Could not find enemy in target location.");
                                    }
                                }
                            }
                        }
                    }
                    break;

                // #######################################   REGROUP AT HOME   ###########################################

                case TaskType.RegroupAtHome:

                    if (!task.IsMetaTask)
                    {
                        if (task.IsNewTask)
                        {
                            mObjectHandler.QueueMove(task.Self, mDivisionMapping[task.Division].Goal, false);
                            task.IsNewTask = false;
                        }

                        if (mDivisionMapping[task.Division].Type != task.Type)
                        {
                            task.AssignTask(mDivisionMapping[task.Division].Type);
                        }
                    }
                    else
                    {
                        if (task.IsNewTask)
                        {
                            mObjectHandler.QueueMove(task.Self, mDivisionMapping[task.Division].Goal, false);
                            task.IsNewTask = false;
                            task.Goal = mGrid.TranslateFromGrid(mGrid.GetClosestLocThatFulfillsLambda(mGrid.TranslateToGrid(mTownHall.Position), SpotIsClearing));
                        }

                        if (mDivisionSizes[task.Division] - mGameState * 7 > mRandom.Next(8, 500))
                        {
                            task.AssignTask(TaskType.MoveTowardsEnemyBase);
                        }
                    }
                    break;

                // #######################################   ATTACK MOVE TO AREA   ###########################################

                case TaskType.AttackMoveToArea:

                    if (!task.IsMetaTask)
                    {
                        if (task.IsNewTask)
                        {
                            mFightManager.AddAttackMoveAttacker((IUnit)task.Self, mDivisionMapping[task.Division].Goal);
                            task.IsNewTask = false;
                        }

                        if (mDivisionMapping[task.Division].Type != task.Type)
                        {
                            task.AssignTask(mDivisionMapping[task.Division].Type);
                        }
                    }
                    else
                    {
                        if (task.IsNewTask)
                        {
                            mFightManager.AddAttackMoveAttacker((IUnit)task.Self, mDivisionMapping[task.Division].Goal);
                            task.IsNewTask = false;
                        }

                        if (!((IMovingObject)task.Self).IsMoving && mFightManager.ScanForTarget((IUnit)task.Self, false, false) == null)
                        {
                            if (Debugger.IsAttached)
                            {
                                //Debug.WriteLine("AIHandler: If this pops up more than a few times I made a mistake (Attack move to area)");
                            }
                            task.AssignTask(TaskType.FormUpWithGroup);
                        }
                    }
                    break;

                // #######################################   ATTACK MOVE TO UNIT   ###########################################

                case TaskType.AttackMoveToUnit:

                    if (task.IsNewTask)
                    {
                        mFightManager.AddAttackMoveAttacker((IUnit)task.Self, mDivisionMapping[task.Division].Other.Position);
                        task.IsNewTask = false;
                    }

                    if (DateTimeOffset.Now.ToUnixTimeMilliseconds() % 500 < 30 && ((IUnit)task.Self).Action != (int)ActionId.Attacking)
                    {
                        mFightManager.AddAttackMoveAttacker((IUnit)task.Self, mDivisionMapping[task.Division].Other.Position);
                    }


                    if (!task.IsMetaTask)
                    {

                        if (mDivisionMapping[task.Division].Type != TaskType.AttackMoveToUnit)
                        {
                            task.AssignTask(mDivisionMapping[task.Division].Type);
                        }
                    }
                    else
                    {

                        if (!((IMovingObject)task.Self).IsMoving && mFightManager.ScanForTarget((IUnit)task.Self, false, false) == null)
                        {
                            if (Debugger.IsAttached)
                            {
                                //Debug.WriteLine("AIHandler: If this pops up more than a few times I made a mistake (attack move to unit)");
                            }
                            task.AssignTask(TaskType.AttackMoveToArea);
                            task.Goal = new Vector2(-50, 300);
                        }
                    }
                    break;
            }
        }

        private bool FilledBy<T>(int loc)
        {
            int x = mGrid.GetWidth();

            if (loc / x >= x - 1 || loc % x <= 0 || loc % x >= x - 1) { return false; }

            Func<int, int> lambda = offset => { return mGrid.CheckIfGridLocationIsFilledFromLoc(loc + offset) ? 1 : 0; };
            
            if (lambda(-1-x) + lambda(-x) + lambda(1-x) + lambda(-1) + lambda(1) + lambda(-1+x) + lambda(x) + lambda(1+x) > 4)
            {
                return false;
            }

            return mGrid.GetStaticObjectFromSlot(loc) is T;
        }

        private bool SpotIsFree(int loc)
        {
            int x = mGrid.GetWidth();

            if (loc / x >= x - 1 || loc % x <= 0 || loc % x >= x - 1) { return false; }
            if (loc % 2 == 0 || loc / x % 2 == 0) { return false; }

            Func<int, int> lambda = offset => { return mGrid.CheckIfGridLocationIsFilledFromLoc(loc + offset) ? 1 : 0; };

            if (lambda(-1 - x) + lambda(-x) + lambda(1 - x) + lambda(-1) + lambda(1) + lambda(-1 + x) + lambda(x) + lambda(1 + x) > 3)
            {
                return false;
            }

            return !mGrid.CheckIfGridLocationIsFilledFromLoc(loc);
        }

        private bool SpotIsClearing(int loc)
        {
            int x = mGrid.GetWidth();

            if (loc / x >= x - 10 || loc % x <= 9 || loc % x >= x - 10) { return false; }

            Func<int, int> lambda = offset => { return mGrid.CheckIfGridLocationIsFilledFromLoc(loc + offset) ? 1 : 0; };

            if (lambda(-1 - x) + lambda(-x) + lambda(1 - x) + lambda(-1) + lambda(1) + lambda(-1 + x) + lambda(x) + lambda(1 + x) > 1)
            {
                return false;
            }

            if (mRandom.Next(1, 3) > 1) { return false; }
            return !mGrid.CheckIfGridLocationIsFilledFromLoc(loc);
        }

        private bool SpotHasAttackableEnemy(int loc)
        {

            List<IGameObject> gameObjects;
            if (!mGrid.mGridContent.TryGetValue(loc, out gameObjects)) { return false; }

            foreach (IGameObject gameObject in gameObjects)
            {
                if (gameObject is IUnit unit && unit.Team == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private Vector2 GetNearestResource(Vector2 pos)
        {
            int loc = mGrid.TranslateToGrid(pos);
            Func<int, bool> lambda;

            if (mRandom.Next(1, 100) >= (int)(mWoodToStoneRatio * 100f))
            {
                lambda = FilledBy<Tree>;
            }
            else
            {
                lambda = FilledBy<Stone>;
            }
            if (mRandom.Next(1, 100) > 250 * ((float)(2 * mBarracksBuilt + 1) / (float)(mBuildingsBuilt + 1)))
            {
                lambda = SpotIsFree;
            }
            else if (mBarracksBuilt == 0 && mGameState > 0)
            {
                lambda = SpotIsFree;
            }

            int result = mGrid.GetClosestLocThatFulfillsLambda(loc, lambda);
            return mGrid.TranslateFromGrid(result);
        }


        private void FindStarterWorker()
        {
            foreach (KeyValuePair<string, IGameObject> kvPair in mObjectHandler.Objects)
            {
                if (kvPair.Value is Worker worker && worker.Team == 1)
                {
                    mTasks.Add(new Task(worker));
                    mFoundStarterWorker = true;
                    worker.PreventCollision = true;
                    break;
                }
            }
            List<int> locs = mGrid.GetNeighbors(mGrid.TranslateToGrid(mTownHall.Position));

            foreach (int i in locs)
            {
                if (mGrid.CheckIfGridLocationIsFilledFromLoc(i))
                {
                    mObjectHandler.QueueDelete(mGrid.GetStaticObjectFromSlot(i));
                }
            }
        }

        private int SigmoidFromGameState(float denom, float scale, float xTrans, float yTrans)
        {
            return (int) (denom / (1 + Math.Pow(Math.E, -(scale * mGameState + xTrans))) + yTrans);
        }

        private int GetEnemyAttack(Vector2 pos)
        {
            int target = mGrid.GetFirstLocThatFulFillsLambda(mGrid.TranslateToGrid(pos), SpotHasAttackableEnemy, 50);
            if (target != -1)
            {
                return target;
            }

            return -1;
        }

        private void AssignAndInitDivision(Task task)
        {
            // Could be more complicated but this should work ok
            int result = mRandom.Next(0, mGameState);

            if (mRandom.Next(1, 5) == 5) { result++; }
            if (mRandom.Next(1, 10) == 5) { result+=2; }

            task.Division = result;
            
            if (mDivisionSizes.ContainsKey(result))
            {
                mDivisionSizes[result]++;
            }
            else
            {
                mDivisionSizes.Add(result, 1);
                
                Task metaTask = new(task.Self, result)
                {
                    Goal = mGrid.TranslateFromGrid(mGrid.GetClosestLocThatFulfillsLambda(mGrid.TranslateToGrid(task.Self.Position), SpotIsClearing))
                };
                if (result == 2)
                {
                    metaTask.Goal = mGrid.TranslateFromGrid(mGrid.GetClosestLocThatFulfillsLambda(mGrid.TranslateToGrid(mTownHall.Position), SpotIsFree));
                }
                mDivisionMapping.Add(result, metaTask);
                mNewTasks.Add(metaTask);
            }

            task.AssignTask(mDivisionMapping[result].Type);

        }


        // This code shouldn't be here, but as it was not encapsulated and lies in worldScreen there is no way for me to use it without copying it to below.

        private void SetupBarracks(Barracks barracks)
        {
            barracks.OnTroopSpawn += HandleTroopSpawn;
        }

        public void LoadTownHall(TownHall townHall)
        {
            mTownHall = townHall;
        }

        private void HandleTroopSpawn(TroopType troopType, Vector2 location)
        {
            // Define the radius for random spawning.
            const int spawnRadius = 50;

            // Generate a random offset within the radius.
            Random random = new Random();
            double angle = random.NextDouble() * Math.PI * 2;
            double radius = random.NextDouble() * spawnRadius;
            int offsetX = (int)(radius * Math.Cos(angle));
            int offsetY = (int)(radius * Math.Sin(angle));

            // Apply the offset to the location.
            int spawnX = (int)location.X - 70 + offsetX;
            int spawnY = (int)location.Y + 80 + offsetY;

            IUnit unit = null;

            switch (troopType)
            {
                case TroopType.Swordsman:
                    unit = new Swordsman(null, spawnX, spawnY, 1, mContentManager, mFightManager, mStatisticsManager);
                    break;
                case TroopType.Archer:
                    unit = new Archer(null, spawnX, spawnY, 1, mContentManager, mFightManager, mStatisticsManager);
                    break;
                case TroopType.Knight:
                    unit = new Knight(null, spawnX, spawnY, 1, mContentManager, mFightManager, mStatisticsManager);
                    break;
                case TroopType.Worker:
                    unit = new Worker(null, spawnX, spawnY, 1, mContentManager, mFightManager, mStatisticsManager);
                    mTownHall.mWorkers.Add((Worker)unit);
                    break;
            }

            unit.PreventCollision = true;
            mObjectHandler.QueueCreate(unit);
            Task task = new (unit);
            mNewTasks.Add(task);

            if (unit is not Worker)
            {
                AssignAndInitDivision(task);
            }
        }
    }
}
