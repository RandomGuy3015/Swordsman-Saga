using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Swordsman_Saga.Engine.DataTypes.Grids;
using Swordsman_Saga.Engine.ObjectManagement;

namespace Swordsman_Saga.Engine.DataTypes
{   /// <summary>
    /// Diamond Grid that fits over isometric Map.
    /// </summary>
    class DiamondGrid
    {
        private readonly List<List<GridLocation>> mSlots = new(); // pathfinding Grid
        public bool ShowGridLines { get; set; }
        public bool ShowPathfindingGrid { get; set; }
        public bool ShowCollisionGrid { get; set; }

        private Vector2 mSlotCount;
        private Vector2 mStartPos;
        private readonly int mSlotSize;
        Matrix mTransformMatrix;
        Matrix mInvertedMatrix;

        public readonly Dictionary<int, List<IGameObject>> mGridContent = new(); // collision Grid.

        // ##########################   CONSTRUCTORS   #############################
        public DiamondGrid(Vector2 slotCount, int slotSize, Vector2 startPos)
        {
            mSlotSize = slotSize;
            mSlotCount = slotCount;
            mStartPos = startPos * slotSize + new Vector2(-0.5f * slotSize, 0.5f * slotSize);
            ShowGridLines = false;
            ShowPathfindingGrid = false;
            ShowCollisionGrid = false;
            mTransformMatrix = new Matrix(0.5f, 0.5f, 0, 0, -0.5f, 0.5f, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            mInvertedMatrix = Matrix.Invert(mTransformMatrix);
            InitGrid();
        }


        // ##########################   UPDATE GRID   ###############################

        public void UpdateGrid(IGameObject gameObject)
        {
            if (gameObject == null) throw new ArgumentNullException("Why is the grid being updated with a non-existent object?");

            int key = TranslateToGrid(gameObject.Position);
            if (mGridContent.ContainsKey(key))
            {
                foreach (IGameObject gridObject in mGridContent[key])
                {
                    if (gameObject == gridObject)
                    {
                        return;
                    }
                }
                mGridContent[key].Add(gameObject);
                RemoveFromNeighbors(gameObject, key);
            }
            else
            {
                mGridContent[key] = new List<IGameObject>() { gameObject };
                RemoveFromNeighbors(gameObject, key);
            }
        }

        // ##########################   TRANSLATORS   ###############################


        // Returns Slot corresponding to Screen Position
        public int TranslateToGrid(Vector2 pos)
        {
            pos.Y *= 2f;
            Vector2 squarePos = Vector2.Transform(pos, mTransformMatrix);
            squarePos -= mStartPos;
            Vector2 gridPos = new(squarePos.X / mSlotSize, squarePos.Y / mSlotSize);
            Vector2 slot = new((float)Math.Round(gridPos.X), (float)Math.Round(gridPos.Y));

                //throw new ArgumentOutOfRangeException("GridException", $"Position {pos} {slot} {(int)(slot.X + slot.Y * mSlotCount.X)} is not inside of grid!.");
            slot.X = Math.Clamp(slot.X, 0, mSlotCount.X);
            slot.Y = Math.Clamp(slot.Y, 0, mSlotCount.Y);

            return (int)(slot.X + slot.Y * mSlotCount.X);
        }

        // Returns the Screen position of the slot's middle point
        public Vector2 TranslateFromGrid(int slot)
        {
            float x = slot % (int) mSlotCount.X;
            float y = (float) (slot / (int) mSlotCount.X);

            Vector2 squarePos = new Vector2(x * mSlotSize, y * mSlotSize) + mStartPos;

            Vector2 isoPos = Vector2.Transform(squarePos, mInvertedMatrix);
            isoPos.Y *= 0.5f;

            Vector2 finalPos = isoPos;
            //if (slot == 0.5 * mSlotCount.X * mSlotCount.Y + 0.5 * mSlotCount.X) throw new IndexOutOfRangeException("hi");
            return finalPos;
        }

        // What are these methods? Why not use the in-built ones like every other class?
        // Returns Slot corresponding to Screen Position in Vector Form
        public Vector2 TranslateToGridV(Vector2 pos)
        {
            pos.Y *= 2f;
            Vector2 squarePos = Vector2.Transform(pos, mTransformMatrix);
            squarePos -= mStartPos;
            Vector2 gridPos = new(squarePos.X / mSlotSize, squarePos.Y / mSlotSize);
            Vector2 slot = new((float)Math.Round(gridPos.X), (float)Math.Round(gridPos.Y));

            return slot;
        }

        // Returns the Screen position of the slot's middle point, takes Vector
        public Vector2 TranslateFromGridV(Vector2 slot)
        {
            Vector2 squarePos = new Vector2(slot.X * mSlotSize, slot.Y * mSlotSize) + mStartPos;

            Vector2 isoPos = Vector2.Transform(squarePos, mInvertedMatrix);
            isoPos.Y *= 0.5f;

            Vector2 finalPos = isoPos;
            return finalPos;
        }

        // ###############################   TYPE HELPERS   ###############################

        public Vector2 GetVector2(int slot)
        {
            return new Vector2(slot % (int) mSlotCount.X, (int)(slot / mSlotCount.X));
        }

        /*
        public Vector2 GetTranslatedVector(Vector2 pos)
        {
            Vector2 squarePos = Vector2.Transform(pos, mTransformMatrix);
            squarePos.Y *= 0.5f;
            return squarePos;
        }

        public Vector2 GetUntranslatedTranslatedVector(Vector2 pos)
        {
            pos.Y *= 2f;
            Vector2 squarePos = Vector2.Transform(pos, mInvertedMatrix);
            return squarePos;
        }
        */

        public int GetInt(Vector2 pos) // is the returned int the slot for the pathfinding grid?
        {
            return (int) pos.X + (int) mSlotCount.X * (int) pos.Y;
        }

        private GridLocation GetGridLocationFromSlot(int pos)
        {
            pos = Math.Clamp(pos, 0, (int)mSlotCount.X * (int)mSlotCount.Y - 1);
            return mSlots[pos / (int)mSlotCount.X][pos % (int)mSlotCount.X];
        }
        
        
        // for the FightManager, its necessary cause the exact location from the mouse (pixel) is needed when selecting fight target manually
        // but it would be more clean to just take the getAttackableObject from slot here and do the target selection in the FightManager
        public IAttackableObject GetAttackableObjectFromPixel(Vector2 pixel)
        {
            // loop all objects in the Grid
            List<IGameObject> objects = new List<IGameObject>();
            int key = TranslateToGrid(pixel);
            if (mGridContent.ContainsKey(key))
            {
                objects = mGridContent[key];
                // if units are in the grid, check if their HitboxRectangle contains the pixel
                foreach (var gameObject in objects)
                {
                    IUnit unit = gameObject as IUnit;
                    if (unit != null)
                    {
                        // TODO: maybe change to Texture Rectangle?
                        if (unit.HitboxRectangle.Contains(pixel))
                        {
                            return unit;
                        }
                    }
                }
                // if not check for static object (returns null if not existent
                return GetStaticObjectFromPixel(pixel);
            }
            // return null if key is not existent in the dictionary
            else
            {
                return null;
            }
        }
        public List<IAttackableObject> GetAttackableObjectFromSlot(int key)
        {
            List<IAttackableObject> attackableObjects = new List<IAttackableObject>();
            if (mGridContent.ContainsKey(key))
            {
                foreach (var gameObject in mGridContent[key])
                {
                    var attackableObject = gameObject as IAttackableObject;
                    if(attackableObject is not null) attackableObjects.Add(attackableObject);
                }
                return attackableObjects;
            }
            // return null if key is not existent in the dictionary
            else
            {
                return null;
            }
        }

        // ###############################   HELPER FUNCTIONS   ###############################


        public List<int> GetNeighbors(int key)
        {
            return new List<int>() { key - (int)mSlotCount.X - 1, key - (int)mSlotCount.X, key - (int)mSlotCount.X + 1, key - 1, key + 1, key + (int)mSlotCount.X - 1, key + (int)mSlotCount.X, key + (int)mSlotCount.X + 1 };
        }
        public List<Vector2> GetNeighbors(Vector2 pos)
        {
            // Q: This shouldn't / probably doesn't work. Adding one to the location vector gets the pixel neighbor, not the slot neighbor.
            // A: It does work

            List<Vector2> resList = new();
            var neighbors = new List<Vector2>
            {
                new(pos.X, pos.Y - 1),
                new(pos.X - 1, pos.Y),
                new(pos.X + 1, pos.Y),
                new(pos.X, pos.Y + 1)

            };
            foreach (Vector2 neighbor in neighbors)
            {
                if(0 <= neighbor.X && neighbor.X <  mSlotCount.X && 0 <= neighbor.Y && neighbor.Y < mSlotCount.Y) if (IsPathable(neighbor)) resList.Add(neighbor);
            }
            if ((resList.Contains(neighbors[0]) || resList.Contains(neighbors[1])) && pos is {X: > 0, Y: > 0}) resList.Add(new Vector2(pos.X - 1, pos.Y - 1));
            if ((resList.Contains(neighbors[0]) || resList.Contains(neighbors[2])) && pos.X < mSlotCount.X - 1 && pos.Y > 0) resList.Add(new Vector2(pos.X + 1, pos.Y - 1));
            if ((resList.Contains(neighbors[3]) || resList.Contains(neighbors[1])) && pos.X > 0 && pos.Y < mSlotCount.Y - 1) resList.Add(new Vector2(pos.X - 1, pos.Y + 1));
            if ((resList.Contains(neighbors[3]) || resList.Contains(neighbors[2])) && pos.X < mSlotCount.X - 1 && pos.Y < mSlotCount.Y - 1) resList.Add(new Vector2(pos.X + 1, pos.Y + 1));
            return resList;
        }

        public List<int> GetNeighborsInRange(int key, int range)
        {
            List<int> neighbors = new();
            for (int y = -range; y < range; y++)
            {
                if (key / mSlotCount.X + y < 0 || key / mSlotCount.X + y > mSlotCount.Y - 1)
                {
                    continue;
                }

                for (int x = -range; x < range; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    if (key % mSlotCount.X + x < 0 || key % mSlotCount.X + x > mSlotCount.X - 1)
                    {
                        continue;
                    }

                    neighbors.Add(key + x + y * (int)mSlotCount.X);
                }
            }

            return neighbors;
        }

        public int GetFirstLocThatFulFillsLambda(int key, Func<int, bool> lambda, int range)
        {
            List<int> neighbors = GetNeighborsInRange(key, range);

            foreach (int slot in neighbors) {

                if (lambda(slot))
                {
                    return slot;
                }
            }

            return -1;
        }

        public int GetClosestLocThatFulfillsLambda(int key, Func<int, bool> lambda)
        {
            int closest = 0;
            int range = 0;
            double closestDist = double.MaxValue;

            void CheckAndUpdate(int x, int y)
            {
                int loc = key + x + y * (int)mSlotCount.X;

                if (key / mSlotCount.X + y < 0 || key / mSlotCount.X + y > mSlotCount.Y - 1)
                {
                    return;
                }

                if (x == 0 && y == 0)
                {
                    return;
                }

                if (key % mSlotCount.X + x < 0 || key % mSlotCount.X + x > mSlotCount.X - 1)
                {
                    return;
                }

                if (lambda(loc))
                {
                    double dist = Vector2.Distance(TranslateFromGrid(closest), TranslateFromGrid(loc));

                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = loc;
                        //Debug.WriteLine($"{loc} is {dist} far away.");
                    }
                }
            }

            while (closest == 0)
            {
                range++;

                for (int x = -range; x < range; x++)
                {
                    CheckAndUpdate(x, -range);
                    CheckAndUpdate(x, range);
                }
                for (int y = -range; y < range; y++)
                {
                    CheckAndUpdate(-range, y);
                    CheckAndUpdate(range, y);
                }
            }

            if (range > 100) { throw new Exception("Could not find target within a 100 square radius."); }
            if (closest == 0) { throw new Exception("How?"); }
            return closest;
        }

        // Returns the outer Points of the Slot translated to screen Positions in the order:
        // 0---1---2          2
        // |       |       1     4
        // 3   x   4    0     x     7
        // |       |       3     6
        // 5---6---7          5
        // but in Tilted form, due to Ascii limitations that's the best i can show
        public List<Vector2> GetOuterNavPoints(Vector2 slot)
        {
            Vector2 slotScreenPos = TranslateFromGrid(GetInt(slot));
            Vector2 isoHalfSlotHeight = Vector2.Transform(new Vector2(0f, mSlotSize * 0.5f), mInvertedMatrix);
            isoHalfSlotHeight.Y *= 0.5f;
            Vector2 isoHalfSlotWidth = Vector2.Transform(new Vector2(mSlotSize * 0.5f, 0f), mInvertedMatrix);
            isoHalfSlotWidth.Y *= 0.5f;


            List<Vector2> outerNavPoints = new List<Vector2>
            {
                slotScreenPos - isoHalfSlotWidth - isoHalfSlotHeight,
                slotScreenPos - isoHalfSlotHeight,
                slotScreenPos + isoHalfSlotWidth - isoHalfSlotHeight,
                slotScreenPos - isoHalfSlotWidth,
                slotScreenPos + isoHalfSlotWidth,
                slotScreenPos - isoHalfSlotWidth + isoHalfSlotHeight,
                slotScreenPos + isoHalfSlotHeight,
                slotScreenPos + isoHalfSlotWidth + isoHalfSlotHeight,
            };
            return outerNavPoints;
        }
        private void RemoveFromNeighbors(IGameObject gameObject, int key)
        {
            foreach (int newKey in GetNeighbors(key))
            {
                RemoveFromSlot(gameObject, newKey);
            }
        }
        private void RemoveFromSlot(IGameObject gameObject, int key)
        {
            if (mGridContent.ContainsKey(key))
            {
                mGridContent[key].Remove(gameObject);
                if (mGridContent[key].Count == 0)
                {
                    mGridContent.Remove(key);
                }
            }
        }
        public bool CheckIfGridLocationIsFilledFromPixel(Vector2 pixel)
        {
            GridLocation gridLocation = GetGridLocationFromSlot(TranslateToGrid(pixel));
            if (gridLocation == null) { throw new ArgumentOutOfRangeException("GridLocation is outside of Grid!."); }
            return gridLocation.IsFilled;
        }

        public bool CheckIfGridLocationIsFilledFromLoc(int loc)
        {
            GridLocation gridLocation = GetGridLocationFromSlot(loc);
            if (gridLocation == null) { throw new ArgumentOutOfRangeException("GridLocation is outside of Grid!."); }
            return gridLocation.IsFilled;
        }


        public void SetGridLocationToStaticObjectFromPixel(IStaticObject staticObject, Vector2 pixel)
        {
            GridLocation gridLocation = GetGridLocationFromSlot(TranslateToGrid(pixel));
            // throws an error if gridLocation is already filled. First check if spot is free using CheckIfGridLocationIsFilled()!
            if (gridLocation.IsFilled) { throw new ArgumentOutOfRangeException("Attempted to fill an already filled gridLocation!.");}
            gridLocation.SetToFilled(staticObject, true);
        }

        public IStaticObject GetStaticObjectFromPixel(Vector2 pixel)
        {
            return GetGridLocationFromSlot(TranslateToGrid(pixel)).StaticObject;
        }

        public IStaticObject GetStaticObjectFromSlot(int key)
        {
            return GetGridLocationFromSlot(key).StaticObject;
        }

        public void DeleteStaticObjectFromPixel(Vector2 pixel, bool supressError)
        {
            GridLocation gridLocation = GetGridLocationFromSlot(TranslateToGrid(pixel));
            
            if (!gridLocation.IsFilled)
            {
                if (!supressError)
                {
                    throw new ArgumentNullException(gridLocation.ToString() + " is not filled, yet clearing was attempted.");
                }
                else
                {
                    return;
                }
            }
            gridLocation.Clear(false);
        }

        public void DeleteFromCollisionGrid(IGameObject gameObject, bool supressError)
        {
            if (gameObject == null)
            {
                if (!supressError)
                {
                    throw new ArgumentNullException(gameObject.ToString() + " does not exist, yet deletion was still attempted");
                }
                else
                {
                    return;
                }
            }
            int key = TranslateToGrid(gameObject.Position);
            if (mGridContent.ContainsKey(key))
            {
                mGridContent[key].Remove(gameObject);
                if (mGridContent[key].Count == 0)
                {
                    mGridContent.Remove(key);
                }
            }
            else
            {
                if (!supressError)
                {
                    throw new ArgumentOutOfRangeException(gameObject.ToString() + " does not exist in tile equal to it's position. Maybe you forgot to updateGrid()?");
                }
            }
        }

        public Diamond GetGridDiamondFromPixel(Vector2 pixel)
        {
            int slot = TranslateToGrid(pixel);
            Vector2 snappedToTargetPixel = TranslateFromGrid(slot);
            return new Diamond(snappedToTargetPixel, mSlotSize);
        }

        public List<int> GetSlotPositionsInArea(Vector2 pos1, Vector2 pos2)
        {
            Vector2 topLeft = new(Math.Min(pos1.X, pos2.X), Math.Min(pos1.Y, pos2.Y));
            Vector2 bottomRight = new(Math.Max(pos1.X, pos2.X) + 1, Math.Max(pos1.Y, pos2.Y) + 1);

            List<Vector2> subPoints = new();
            HashSet<int> slots = new();

            for (int y = (int) topLeft.Y; y < bottomRight.Y + mSlotSize / 2 - 1; y += mSlotSize / 2)
            {
                for (int x = (int) topLeft.X; x < bottomRight.X + mSlotSize - 1; x += mSlotSize)
                {
                    subPoints.Add(new Vector2(x, y));
                }
            }
            foreach (Vector2 subPoint in subPoints)
            {
                slots.Add((TranslateToGrid(subPoint)));
            }

            return slots.ToList();
        }

        public int GetWidth()
        {
            return (int)mSlotCount.X;
        }

        //   ########################   GRID HELPERS   ###########################

        public bool IsPathable(Vector2 pos)
        {
            return !mSlots[(int) pos.Y][(int) pos.X].IsFilled;
        }

        public void DrawGrid(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < mSlotCount.X * mSlotCount.Y; x++)
            {
                Diamond diamond = new(TranslateFromGrid(x), mSlotSize);
                diamond.Draw(spriteBatch, Color.White);
            }
        }

        public void DrawGridLocation(SpriteBatch spriteBatch, int pos, Color color)
        {
            Diamond diamond = new(TranslateFromGrid(pos), mSlotSize);
            diamond.Draw(spriteBatch, color);
        }

        public void DrawPathfindingGrid(SpriteBatch spriteBatch, Color color)
        {
            for (int y = 0; y < mSlotCount.Y; y++)
            {
                for (int x = 0; x < mSlotCount.X; x++)
                {
                    if (mSlots[y][x].IsFilled) 
                    {
                        DrawGridLocation(spriteBatch, x + y * (int) mSlotCount.X, color);
                    }
                }
            }
        }

        public void ResizeGrid(Vector2 size)
        {
            throw new NotImplementedException();
        }

        private void InitGrid()
        {
            for (int y = 0; y < mSlotCount.X; y++)
            {
                mSlots.Add(new List<GridLocation>());

                for (int x = 0; x < mSlotCount.X; x++)
                {
                    mSlots[y].Add(new GridLocation());
                }
            }
        }
    }
}
