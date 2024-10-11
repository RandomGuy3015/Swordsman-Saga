using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.Engine.PathfinderManagement;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataPersistence;
using System.Runtime.Remoting;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.GameElements.GameObjects;
using Swordsman_Saga.GameElements.Screens.HUDs;

namespace Swordsman_Saga.Engine.DataTypes
{
    // #######################################   INTERNAL HELPER CLASS   ##########################################
    internal class Move
    {
        private HashSet<ICollidableObject> _alreadyCollidedWith = new HashSet<ICollidableObject>();
        public IGameObject Self { get; set; }
        public string ID { get; private set; }
        public Vector2 Target { get; private set; }
        public int MoveType { get; private set; }

        public Move(IGameObject gameObject)
        {
            Self = gameObject;
            ID = gameObject.Id;
            MoveType = 0;
        }
        public HashSet<ICollidableObject> AlreadyCollidedWith
        {
            get { return _alreadyCollidedWith; }
        }
        public Move(IGameObject gameObject, string Id)
        {
            Self = gameObject;
            ID = Id;
            MoveType = 5;
        }
        public Move(string Id, IGameObject gameObject)
        {
            Self = gameObject;
            MoveType = 1;
            ID = Id;
        }
        public Move(IGameObject gameObject, Vector2 target, bool init)
        {
            if (init)
            {
                MoveType = 2;
            }
            else
            {
                MoveType = 6;
            }

            Self = gameObject;
            ID = gameObject.Id;
            Target = target;
        }
        public Move(IGameObject gameObject, bool select)
        {
            if (select) { MoveType = 3; }
            else { MoveType = 4; }

            ID = gameObject.Id;
            Self = gameObject;
        }

        public Move()
        {

        }

        public void SaveData(ref GameData gameData, int count)
        {
            Self.SaveData(ref gameData);
            if (gameData.mMoveGameObjects.ContainsKey(count))
            {
                gameData.mMoveGameObjects.Remove(count);
            }
            if (gameData.mMoveTarget.ContainsKey(count))
            {
                gameData.mMoveTarget.Remove(count);
            }
            if (gameData.mMoveMoveType.ContainsKey(count))
            {
                gameData.mMoveMoveType.Remove(count);
            }
            gameData.mMoveGameObjects.Add(count, ID);
            gameData.mMoveTarget.Add(count,  Target);
            gameData.mMoveMoveType.Add(count, MoveType);

        }

        public void LoadData(GameData gameData, int count)
        {
            ID = gameData.mMoveGameObjects[count];
            Target = gameData.mMoveTarget[count];
            MoveType = gameData.mMoveMoveType[count];
        }
    }

    // ***************************************************************************************************************
    // ####################################   OBJECT HANDLER - LOOK HERE   ###########################################
    // ***************************************************************************************************************
    class ObjectHandler
    {
        //Save/Load
        public void SaveData(ref GameData gameData)
        {
            int moves = mMoves.Count;
            int nextMoves = mNextMoves.Count;
            gameData.mMovesCount = moves;
            gameData.mNextMovesCount = nextMoves;
            for (int i = 0; i < (moves + nextMoves); i++)
            {
                if (i < moves)
                {
                    Move move = mMoves.Dequeue();
                    move.SaveData(ref gameData, i);
                }
                else
                {
                    Move move = mNextMoves.Dequeue();
                    move.SaveData(ref gameData, i);
                }
            }
        }

        public void LoadData(GameData gameData)
        {
            for (int i = 0; i < gameData.mMovesCount; i++)
            {
                Move move = new();
                move.LoadData(gameData, i);
                DataPersistenceManager.Instance.SetSelfMove(move);
                mNextMoves.Enqueue(move);
            }

            for (int i = mMoves.Count; i < gameData.mNextMovesCount; i++)
            {
                Move move = new();
                move.LoadData(gameData, i);
                DataPersistenceManager.Instance.SetSelfMove(move);
                mNextMoves.Enqueue(move);
            }
        }

        // Our old mGameObjects list - now upgraded to a hash map!
        public Dictionary<string, IGameObject> Objects;

        // The list of selected GameObjects. For efficiency reasons, it is not always updated: check if Null!
        readonly public Dictionary<string, ISelectableObject> SelectedObjects;

        // The Queue of moves to work through.
        readonly private Queue<Move> mMoves;

        // The Queue of moves to work through next turn.
        readonly private Queue<Move> mNextMoves;

        // The max time used per frame, in miliseconds.
        readonly ulong mBatchLength;

        // Our random machine.
        readonly private Random mRandom;

        private readonly SoundManager mSoundManager;
        private readonly DiamondGrid mGrid;
        private readonly AStarPathfinder mPathfinder;
        public LayerHandler mLayerHandler;
        private Rectangle staticCollisionRect;
        private readonly ResourceHud mResourceHud;


        public ObjectHandler(SoundManager soundManager, DiamondGrid grid, AStarPathfinder pathfinder, ResourceHud resourceHud)
        {
            Objects = new Dictionary<string, IGameObject>();
            mMoves = new Queue<Move>(3000);
            mNextMoves = new Queue<Move>(500);
            SelectedObjects = new Dictionary<string, ISelectableObject>();
            mRandom = new ();


            mBatchLength = 1;

            mPathfinder = pathfinder;
            mSoundManager = soundManager;
            mResourceHud = resourceHud;
            mGrid = grid;
            mLayerHandler = new LayerHandler();
        }


        /// <summary>
        /// The standard way to move a unit. Will throw error if of wrong type unless 'supressError' is true.
        /// </summary>
        public void QueueMove(IGameObject gameObject, Vector2 target, bool supressError)
        {
            if (gameObject is not IMovingObject)
            {
                if (supressError) { return; }
                throw new ArgumentException(gameObject.Id + " (which is a " + gameObject.TypeToString() + " ) is not an IMovingObject");
            }

            Move move = new(gameObject, target, true);

            mMoves.Enqueue(move);
        }

        /// <summary>
        /// Use this to delete an object.
        /// </summary>
        public void QueueDelete(IGameObject gameObject)
        {
            if (gameObject == null) { throw new ArgumentNullException("Cannot delete an already deleted object!"); }
            Move move = new(gameObject.Id, gameObject);
            mMoves.Enqueue(move);
        }

        /// <summary>
        /// Use this to delete all objects and refresh the world.
        /// </summary>
        public void QueueDeleteAll()
        {
            foreach (KeyValuePair<string, IGameObject> kvPair in Objects)
            {
                QueueDelete(kvPair.Value);
            }
        }

        /// <summary>
        /// Use this to create an object.
        /// </summary>
        public void QueueCreate(IGameObject gameObject)
        {
            Move move = new(gameObject);
            mMoves.Enqueue(move);
        }

        /// <summary>
        /// Use this to create an object and delete any static objects below it.
        /// </summary>
        public void QueueCreateOverwrite(IGameObject gameObject)
        {
            Move move = new(gameObject, gameObject.Id);
            mMoves.Enqueue(move);
        }

        /// <summary>
        /// Use this to select objects. Will throw error if of wrong type unless 'supressError' is true.
        /// </summary>
        public void QueueSelect(IGameObject gameObject, bool supressError)
        {
            if (gameObject is not ISelectableObject)
            {
                if (supressError) { return; }
                throw new ArgumentException(gameObject.Id + " (which is a " + gameObject.TypeToString() + " ) is not an ISelectableObject");
            }
            Move move = new(gameObject, true);
            mMoves.Enqueue(move);
        }

        /// <summary>
        /// Use this to deselect objects. Will throw error if of wrong type unless 'supressError' is true.
        /// </summary>
        public void QueueDeselect(IGameObject gameObject, bool supressError)
        {
            if (gameObject is not ISelectableObject)
            {
                if (supressError) { return; }
                throw new ArgumentException(gameObject.Id + " (which is a " + gameObject.TypeToString() + " ) is not an ISelectableObject");
            }
            Move move = new(gameObject, false);
            mMoves.Enqueue(move);
        }

        /// <summary>
        ///  Deselect all selected objects.
        /// </summary>
        public void QueueDeselectAll()
        {
            foreach (KeyValuePair<string, ISelectableObject> kvPair in SelectedObjects)
            {
                QueueDeselect(kvPair.Value, false);
            }
        }

        /// <summary>
        ///  Get the object in the move last added to the Queue
        /// </summary>
        public IGameObject Top()
        {
            return mMoves.Peek().Self;
        }

        // ##################################################   UPDATE   #######################################################

        // this updates as many gameObjects as it can without lagging the game per tick.


        public void Update(GameTime gameTime)
        {
            // Add moves queued from previous update to current move list

            while (mNextMoves.Count > 0)
            {
                mMoves.Enqueue(mNextMoves.Dequeue());
            }

            // Debug
            //if (mMoves.Count > 2000) { Debug.WriteLine("Objects are running slow! Either the map is loading, or you doing something stupid."); }

            // Update all updateable objects
            foreach (KeyValuePair<string, IGameObject> kvPair in Objects)
            {
                if (kvPair.Value is IUpdateableObject updateableObject)
                {
                    updateableObject.Update(gameTime);
                }
                if (kvPair.Value is IMovingObject movingObject && !movingObject.IsMoving)
                {
                    movingObject.JustStartedMoving = false;
                    movingObject.AlreadyCollidedWith.Clear();
                }

            }

            // Work through moves until the allotted time has passed
            // This was bugged the whole time because gameTime is shit...
            long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 30;


            //Debug.WriteLine($"Queue before draw: {mMoves.Count}");

            while (DateTimeOffset.Now.ToUnixTimeMilliseconds() < endTime)
            {
                if (mMoves.Count == 0)
                {
                    break;
                }
                Move move = mMoves.Dequeue();

                switch (move.MoveType)
                {
                    case 0: // Create
                        move.Self.SetSoundManager(mSoundManager);
                        if (move.Self is IMovingObject self)
                        {
                            self.IsColliding = true;
                            self.Destination = self.Position;
                            //mMoves.Enqueue(new Move(self, self.Destination, true));
                        }
                        else if (move.Self is IStaticObject self2)
                        {
                            if (!mGrid.CheckIfGridLocationIsFilledFromPixel(self2.Position))
                            {
                                mGrid.SetGridLocationToStaticObjectFromPixel(self2, self2.Position);
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException("QueueCreate was called on object " + self2.ToString() + " yet the gridLocation is already filled. Use QueueCreateOverwrite or call CheckIfGridLocationIsFilledFromPixel() before hand.");
                            }
                        }
                        Objects.Add(move.Self.Id, move.Self);
                        mGrid.UpdateGrid(move.Self);
                        //mLayerHandler.mSortedDictionary.Add((int)move.Self.Position.Y * 11000 * 10000 + (int)move.Self.Position.X * 10000 + move.ID.GetHashCode(), move.Self);
                        break;

                    case 1: // Delete
                        if (move.Self == null) { throw new ArgumentNullException("Cannot delete an already deleted object"); }
                        mGrid.UpdateGrid(move.Self);
                        if (move.Self is IStaticObject self3)
                        {
                            mGrid.DeleteStaticObjectFromPixel(self3.Position, true);
                        }
                        mGrid.DeleteFromCollisionGrid(move.Self, false);
                        Objects.Remove(move.Self.Id);
                        SelectedObjects.Remove(move.Self.Id);
                        //mLayerHandler.mSortedDictionary.Remove((int)move.Self.Position.Y * 11000 * 10000 + (int)move.Self.Position.X * 10000 + move.ID.GetHashCode());
                        break;

                    case 2: // Initial Move
                        //mLayerHandler.mSortedDictionary.Remove((int)move.Self.Position.Y * 11000 * 10000 + (int)move.Self.Position.X * 10000 + move.ID.GetHashCode());
                        if (move.Self == null) { throw new ArgumentNullException("Cannot move a deleted object"); }

                        IMovingObject movingObject = move.Self as IMovingObject;
                        // Debug.WriteLine("IsMovingInQueue: " + !movingObject.IsMovingInQueue);
                        if (!movingObject.IsMovingInQueue)
                        {
                            movingObject.IsMovingInQueue = true;
                            movingObject.PathingTo = move.Target;
                            movingObject.Path = mPathfinder.FindPath(move.Self.Position, move.Target);
                            if (movingObject.Path.Count == 0)
                            {
                                // Path has zero nodes. Like none.
                                movingObject.Goal = movingObject.Position;
                            }
                            else
                            {
                                movingObject.Goal = movingObject.Path[0];
                            }

                            mMoves.Enqueue(new Move(move.Self, move.Target, false));
                        }
                        else
                        {
                            movingObject.PathingTo = move.Target;
                            movingObject.Path = mPathfinder.FindPath(move.Self.Position, move.Target);
                            if (movingObject.Path.Count > 0)
                            {
                                movingObject.Destination = movingObject.Path[0];

                            }
                        }
                        //mLayerHandler.mSortedDictionary.Add((int)move.Self.Position.Y * 11000 * 10000 + (int)move.Self.Position.X * 10000 + move.ID.GetHashCode(), move.Self);
                        break;

                    case 3: // Select
                        if (!SelectedObjects.ContainsKey(move.ID))
                        {
                            SelectedObjects.Add(move.ID, (ISelectableObject)move.Self);
                            ((ISelectableObject)move.Self).IsSelected = true;
                        }
                        break;

                    case 4: // Deselect
                        SelectedObjects.Remove(move.ID);
                        ((ISelectableObject)move.Self).IsSelected = false;
                        break;

                    case 5: // Overwrite
                        if (move.Self is IMovingObject) { throw new ArgumentException("Are you attempting to overwrite a staticObject with a movingObject? They can coexist in the collision grid. If you have a good reason that is needed then delete this Exception."); }
                        if (mGrid.CheckIfGridLocationIsFilledFromPixel(move.Self.Position))
                        {
                            Objects.Remove(mGrid.GetStaticObjectFromPixel(move.Self.Position).Id);
                            mGrid.DeleteStaticObjectFromPixel(move.Self.Position, false);
                            mGrid.DeleteFromCollisionGrid(move.Self, false);
                        }

                        move.Self.SetSoundManager(mSoundManager);
                        if (move.Self is IMovingObject self4)
                        {
                            self4.IsColliding = true;
                            self4.Destination = self4.Position;
                            mMoves.Enqueue(new Move(self4, self4.Destination, true));
                        }
                        else if (move.Self is IStaticObject self5)
                        {
                            if (!mGrid.CheckIfGridLocationIsFilledFromPixel(self5.Position))
                            {
                                mGrid.SetGridLocationToStaticObjectFromPixel(self5, self5.Position);
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException("QueueCreate was called on object " + self5.ToString() + " yet the gridLocation is already filled. Use QueueCreateOverwrite or call CheckIfGridLocationIsFilledFromPixel() before hand.");
                            }
                        }
                        Objects.Add(move.Self.Id, move.Self);
                        mGrid.UpdateGrid(move.Self);
                        //mLayerHandler.mSortedDictionary.Add((int)move.Self.Position.Y * 11000 * 10000 + (int)move.Self.Position.X * 10000 + move.ID.GetHashCode(), move.Self);
                        break;

                    case 6: // Move
                        //mLayerHandler.mSortedDictionary.Remove((int)move.Self.Position.Y * 11000 * 10000 + (int)move.Self.Position.X * 10000 + move.ID.GetHashCode());
                        if (((IUnit)move.Self).Health <= 0)
                        {
                            QueueDelete(move.Self);
                        }
                        if (move.Self == null) { throw new ArgumentNullException("Cannot move a deleted object"); }
                        ((IMovingObject)move.Self).Move(gameTime);

                        ResolveDynamicCollision((IMovingObject)move.Self);
                        Math.Clamp(move.Self.Position.X, -5200f, 5200f);
                        Math.Clamp(move.Self.Position.Y, 0f, 5200f);

                        if (((IMovingObject)move.Self).IsMoving || ((IMovingObject)move.Self).IsColliding)
                        {
                            mNextMoves.Enqueue(move);
                        }
                        else
                        {
                            ((IMovingObject)move.Self).IsMovingInQueue = false;
                        }
                        mGrid.UpdateGrid(move.Self);
                        //mLayerHandler.mSortedDictionary.Add((int)move.Self.Position.Y * 11000 * 10000 + (int)move.Self.Position.X * 10000 + move.ID.GetHashCode(), move.Self);
                        break;

                };
            }
            //Debug.WriteLine($"Queue after draw: {mMoves.Count}");

            RemoveDeadGameObjects();

        }

        // Catch and delete all 'dead' objects
        private void RemoveDeadGameObjects()
        {
            foreach (KeyValuePair<string, IGameObject> kvPair in Objects)
            {
                if (kvPair.Value is IUnit unit && unit.IsDead)
                {
                    QueueDelete(kvPair.Value);
                }
                if (kvPair.Value is IStaticObject staticObject && staticObject.Health == 0)
                {
                    QueueDelete(kvPair.Value);
                    if (staticObject is Tree)
                    {
                        mResourceHud.WoodCount += 20;
                    }
                    if (staticObject is Stone)
                    {
                        mResourceHud.StoneCount += 20;
                    }
                }
                if (kvPair.Value is Arrow arrow && arrow.mRemove)
                {
                    QueueDelete(kvPair.Value);
                }
            }
        }


        // ######################################################   DRAW   ########################################################

        /*
        public void DrawAllObjects(SpriteBatch spriteBatch, bool showHitboxes, bool showTextureRectangles)
        {
            foreach (KeyValuePair<long, IGameObject> kvPair in mLayerHandler.mSortedDictionary)
            {
                if (kvPair.Value is IDrawableObject drawableObject)
                {
                    drawableObject.Draw(spriteBatch, mGrid, showHitboxes, showTextureRectangles);
                }
                // Draw the Calculated Path
                if (kvPair.Value is IMovingObject movingObject && movingObject.IsMoving)
                {
                    if (movingObject.Path.Count > 0)
                    {
                        mPathfinder.DrawPath(spriteBatch, movingObject.Path);
                    }
                    else
                    {
                        List<Vector2> path = new List<Vector2>();
                        path.Add(movingObject.Destination);
                        mPathfinder.DrawPath(spriteBatch, path);
                    }
                }
            }

        }*/
        public void DrawAllObjects(SpriteBatch spriteBatch, bool showHitBoxes, bool showTextureRectangles)
        {
            mLayerHandler.UpdateQueue(Objects);
            while (mLayerHandler.mPriorityQueue.Count > 0)
            {
                IGameObject gameObject = mLayerHandler.mPriorityQueue.Dequeue();
                if (gameObject is IDrawableObject drawableObject)
                {
                    drawableObject.Draw(spriteBatch, mGrid, showHitBoxes, showTextureRectangles);
                }
                // Draw the Calculated Path
                if (gameObject is IUnit { IsMoving: true, IsSelected: true } unit)
                {
                    if (unit.Path.Count > 0)
                    {
                        List<Vector2> temp = new()
                        {
                            unit.Position,
                            unit.Destination
                        };
                        temp.AddRange(unit.Path);
                        AStarPathfinder.DrawPath(spriteBatch, temp);
                    }
                    else
                    {
                        List<Vector2> path = new()
                        {
                            unit.Position,
                            unit.Destination
                        };
                        AStarPathfinder.DrawPath(spriteBatch, path);
                    }
                }
            }

        }
        // ######################################################   COLLISION   ########################################################


        // INFO: 
        // Collision used to work with separate static and dynamic collision. Now that all collidable objects are in the collision grid, that is not needed anymore.
        // Only dynamic collision is needed.

        // How collision currently works: See below (also loops through each gameObject every collision - needs to work with grid!!)

        // How collision should work:
        //   1. Loops through only the objects in the same area (mGrid.mGridContent of Grid.TranslateToGrid + mGrid.mGridContent of mGrid.GetNeighbors -> all the objects in the same area)
        //   2. Check if colliding (HitBoxRectangle.Intersects)
        //   3. Case collidingObject is MovingObject:
        //          IsMoving:
        //              Push the object you are colliding with to the side (Currently the objects you collide with don't move!). This allows units to reach their destination.
        //          !IsMoving:
        //              Stand around and get pushed if another units wants to go by.
        //              If collides with something else:
        //                  Pushes that unit away

        //      Case collidingObject is StaticObject:
        //          Either: Remembers where it got pushed into the staticObject (DO NOT UPDATE EVERY FRAME IF YOU DO THIS! I WILL THROW A BRICK THROUGH YOUR WINDOW!) and gets pushed back to that point
        //          Or: Gets pushed towards the closest point not in the hit-box of the static Object (SAME HERE: IF YOU DO THAT ITERATIVELY AND NOT PROPERLY MATHEMATICALLY I'LL FUCK YOU UP)

        // We need (!!!) proper collision or our game will never handle >100 units.
        // TODO: Implement it!
        private static bool IsOverlapSignificant(IMovingObject rect1, ICollidableObject rect2)
        {
            Rectangle intersection = Rectangle.Intersect(rect1.HitboxRectangle, rect2.HitboxRectangle);
            int overlapThreshold = rect2 is IStaticObject ? 30 : 31;
            return intersection.Width >= overlapThreshold && intersection.Height >= overlapThreshold;
        }
        private void ResolveDynamicCollision(IMovingObject self)
        {

            int gridKey = mGrid.TranslateToGrid(self.Position);
            var objectsInSameCell = mGrid.mGridContent.ContainsKey(gridKey) ? mGrid.mGridContent[gridKey] : Enumerable.Empty<IGameObject>();
            var objectsInNeighborCells = mGrid.GetNeighbors(gridKey)
                .Where(neighborKey => mGrid.mGridContent.ContainsKey(neighborKey))
                .SelectMany(neighborKey => mGrid.mGridContent[neighborKey]);
            var gridObjects = objectsInSameCell.Concat(objectsInNeighborCells);

            foreach (var obj in gridObjects)
            {
                if (obj is ICollidableObject collidableObject && collidableObject != self &&
                    IsOverlapSignificant(self, collidableObject))
                {
                    if (!self.IsMoving && collidableObject is IMovingObject movingObject1 && self.HitboxRectangle.Intersects(movingObject1.HitboxRectangle))
                    {
                        if (!movingObject1.IsMoving)
                        {
                            self.PreventCollision = true;
                            movingObject1.PreventCollision = true;
                            Vector2 newTarget = movingObject1.Position;
                            movingObject1.Goal = movingObject1.Position + GetPushDirection(self, movingObject1) * 50f;

                            //QueueMove(movingObject1, movingObject1.Goal, false);

                        }
                    }

                    if (self.IsMoving && collidableObject is IMovingObject movingObject)
                    {


                        if (!movingObject.IsMoving)
                        {
                            Vector2 newTarget = movingObject.Position;
                            movingObject.Destination = movingObject.Position + GetPushDirection(self, movingObject) * 50f;

                            movingObject.IsMoving = false;
                            movingObject.JustStartedMoving = true;
                            QueueMove(movingObject, movingObject.Destination, false);

                        }
                        else
                        {
                            if (IsOverlapSignificant(self, collidableObject))
                            //if (self.HitboxRectangle.Intersects(movingObject.HitboxRectangle))
                            {
                                Vector2 directionToOther = movingObject.Position - self.Position;
                                directionToOther.Normalize();
                                Vector2 newPositionSelf = movingObject.Position += directionToOther * 3f;
                                Vector2 newPositionOther = self.Position -= directionToOther * 3f;

                                Vector2 newDestinationSelf = movingObject.Destination += directionToOther * 3f;
                                Vector2 newDestinationother = self.Destination -= directionToOther * 3f;

                                if (self.Path.Count() > 1)
                                {
                                    //QueueMove(self, self.Path.Last(), false);
                                }
                                else if (self.Path.Count() == 1 && !self.PreventCollision)
                                {
                                    self.StopMoving();

                                }
                                if (movingObject.Path.Count() > 1)
                                {
                                    //QueueMove(movingObject, movingObject.Path.Last(), false);
                                }
                                else if (movingObject.Path.Count() == 1 && !movingObject.PreventCollision)
                                {
                                    movingObject.StopMoving();
                                }
                                //movingObject.Path.Insert(0, newPositionOther);

                            }
                        }
                        }
                    else if (collidableObject is IStaticObject staticObject)
                    {
                        self.LastCollisionTime = DateTime.Now;
                        if (!self.IsMoving)
                        {
                            self.CollisionCount = 0;
                        }
                        if (self.CollisionCount % 10 == 0)
                        {
                            MoveBackward(self, staticObject);
                        }
                        Vector2 centerSelf = new Vector2(self.HitboxRectangle.Center.X, self.HitboxRectangle.Center.Y);
                        Vector2 centerStatic = new Vector2(staticObject.HitboxRectangle.Center.X, staticObject.HitboxRectangle.Center.Y);
                        Vector2 direction = centerSelf - centerStatic;
                        direction.Normalize();

                        float displacementMagnitude = 10.0f;
                        Vector2 newPosition = self.Position + direction * displacementMagnitude;
                        self.Position = newPosition;
                        self.CollisionCount++;

                    }
                    self.IsColliding = true;
                    return;
                }
            }
            self.IsColliding = false;
        }

        private void MoveBackward(IMovingObject self, IStaticObject staticObject)
        {

            Vector2 centerSelf = new Vector2(self.HitboxRectangle.Center.X, self.HitboxRectangle.Center.Y);
            Vector2 centerStatic = new Vector2(staticObject.HitboxRectangle.Center.X, staticObject.HitboxRectangle.Center.Y);

            Vector2 collisionDirection = centerSelf - centerStatic;
            if (collisionDirection != Vector2.Zero)
            {
                collisionDirection.Normalize();
            }
            float backwardDistance = 15.0f; 
            Vector2 newPosition = self.Position + collisionDirection * backwardDistance;
            if (self.CollisionCount < 30 && self.Path.Count() == 1)
            {
                self.Destination = newPosition + collisionDirection * 10f;
            }
            if (self.CollisionCount < 30) {
                self.Position = newPosition + collisionDirection * 10f;
            }
            else if (self.CollisionCount > 10 && (DateTime.Now - self.LastCollisionTime).TotalSeconds < 1)
            {
                    self.Destination = self.Position;
                    self.StopMoving();

                    self.CollisionCount = 0;

            }
            else
            {
                self.Position = self.Destination;

                self.CollisionCount = 0;
            }

        }
        private Vector2 GetPushDirection(IMovingObject self, IMovingObject other)
        {
            // Calculate the direction from 'self' to 'other'
            Vector2 directionToOther = other.Position - self.Position;
            directionToOther.Normalize();

            // Calculate 'self's movement direction
            Vector2 movementDirection = self.Destination - self.Position;
            if (movementDirection != Vector2.Zero)
            {
                movementDirection.Normalize();
            }

            // Determine the side of approach
            float sideApproachDotProduct = Vector2.Dot(new Vector2(directionToOther.Y, -directionToOther.X), movementDirection);

            if (sideApproachDotProduct > 0)
            {
                // 'self' is approaching from left of 'other', push 'other' to the right
                return new Vector2(-movementDirection.Y, movementDirection.X);
            }
            else
            {
                // 'self' is approaching from right of 'other', push 'other' to the left
                return new Vector2(movementDirection.Y, -movementDirection.X);
            }
        }


    }
}
