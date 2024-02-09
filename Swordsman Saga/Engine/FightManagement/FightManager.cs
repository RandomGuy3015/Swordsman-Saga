using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DataTypes.Grids;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.PathfinderManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.GameObjects;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.GameElements.Screens;

namespace Swordsman_Saga.Engine.FightManagement;

class FightManager
{
    private InputManager mInputManager;
    private AStarPathfinder mPathfinder;
    private SoundManager mSoundManager;
    private DiamondGrid mGrid;
    private GameTime mGameTime;
    private WorldScreen mWorldScreen;
    private ObjectHandler mObjectHandler;

    // Dictionary that pairs the unit that is attacking with the unit that is being attacked.
    private Dictionary<IUnit, IAttackableObject> mAttackerTargetMapping;
    // Dictionary that pairs units doing attack move with their actual grid location and their goal grid location
    private Dictionary<IUnit, Tuple<int, int>> mAttackerDestinationMapping;

    private int counterForPathfinder = 200;
    private int counterForScanning = 500;
    
    public FightManager(InputManager inputManager, SoundManager soundManager)
    {
        mInputManager = inputManager;
        mSoundManager = soundManager;
        //mGameTime = gameTime;
    }

    public void Initialize(AStarPathfinder pathfinder, DiamondGrid grid, ObjectHandler objectHandler, WorldScreen worldScreen)
    {
        mWorldScreen = worldScreen;
        mPathfinder = pathfinder;
        mGrid = grid;
        mObjectHandler = objectHandler;
        mAttackerTargetMapping = new Dictionary<IUnit, IAttackableObject>();
        mAttackerDestinationMapping = new Dictionary<IUnit, Tuple<int, int>>();
    }
    
    public void SetTargetForAttacker(IUnit attacker, IAttackableObject target)
     {
        return;
         if (!mAttackerTargetMapping.ContainsKey(attacker))
         {
             mAttackerTargetMapping.Add(attacker, target);
         }
         else
         {
             mAttackerTargetMapping[attacker] = target;
         }
     }

    public void Update(GameTime gameTime)
    {
        counterForPathfinder -= gameTime.ElapsedGameTime.Milliseconds;
        counterForScanning -= gameTime.ElapsedGameTime.Milliseconds;

        //long endTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 4;

        // for all Attackers, check if Target is in attack range
        foreach (var pair in mAttackerTargetMapping)
        {
            return;
            //if (DateTimeOffset.Now.ToUnixTimeMilliseconds() > endTime) { break; }
            if (!pair.Key.IsAttacking())
            {
                if (pair.Key.IsInAttackRange(pair.Value.Position))
                {
                    // only start new Attack if last Attack is finished
                    if (/*!pair.Key.IsAttacking() &&*/ !pair.Key.IsDying())
                    {
                        //mSoundManager.PlaySound("SoundAssets/Sword_sound", 1, false, false);
                        pair.Key.StartAttack(pair.Value.Position - pair.Key.Position);
                    }
                }
                else if (counterForPathfinder <= 0)
                {
                    // TODO: Pathfinder now calculates new Path every Update Cycle -> Find Solution
                    // zb if pair.key-"destination of actual path" != pair.Value.Position

                    // move towards target
                    counterForPathfinder = 200;
                    if (pair.Key.Path.Count > 0)
                    {
                        var diamond1 = mGrid.GetGridDiamondFromPixel(pair.Key.PathingTo);
                        var diamond2 = mGrid.GetGridDiamondFromPixel(pair.Value.Position);
                        if (diamond1.X != diamond2.X || diamond1.Y != diamond2.Y)
                        {
                            mObjectHandler.QueueMove(pair.Key, pair.Value.Position, false);
                        }
                    }
                    else
                    {
                        var diamond1 = mGrid.GetGridDiamondFromPixel(pair.Key.PathingTo);
                        var diamond2 = mGrid.GetGridDiamondFromPixel(pair.Value.Position);
                        if (diamond1.X != diamond2.X || diamond1.Y != diamond2.Y)
                        {
                            mObjectHandler.QueueMove(pair.Key, pair.Value.Position, false);
                        }
                    }
                }
            }
        }
        // for all units doing attack move
        foreach (var pair in mAttackerDestinationMapping)
        {
            return;
            IUnit unit = pair.Key;
            // skip if unit has a target
            if (mAttackerTargetMapping.ContainsKey(unit))
            {
                // skip
            }
            else
            {
                // unit has entered a new grid?
                int actualGridPosition = mGrid.TranslateToGrid(unit.Position);
                if (actualGridPosition != pair.Value.Item1)
                {
                    // scan for target and update position values in dicitonary
                    mAttackerDestinationMapping[unit] = new Tuple<int, int>(actualGridPosition, pair.Value.Item2);
                    var target = ScanForTarget(unit, false, false);
                    if (target is not null) SetTargetForAttacker(unit, target);
                    
                    //destination reached?
                    if (actualGridPosition == pair.Value.Item2)
                    {
                        RemoveAttackMoveAttacker(unit);
                    }
                }
            }
        }
        return;
        if (counterForScanning <= 0)
        {
            counterForScanning = 500;
            // all units that are doing nothing scan for auto attacking target 
            foreach (var gameObject in mObjectHandler.Objects)
            {
                if (gameObject.Value is IUnit unit && unit.Action == 0)
                {
                    IAttackableObject target = ScanForTarget(unit, false, false);
                    if (target is not null) mAttackerTargetMapping[unit] = target;
                }
            }
        }

    }

    public void RemoveAttackers(Dictionary<string, ISelectableObject> attackers)
    {
        return;
        foreach (var attacker in attackers)
        {
            var unit = attacker.Value as IUnit;
            if (unit != null) {mAttackerTargetMapping.Remove(unit);}
        }
    }

    public void RemoveAttacker(IUnit attacker)
    {
        return;
        if (mAttackerTargetMapping.ContainsKey(attacker))
        {
            mAttackerTargetMapping.Remove(attacker);
        }
    }

    public void RemoveAttackMoveAttacker(IUnit unit)
    {
        return;
        if (mAttackerDestinationMapping.ContainsKey(unit))
        {
            mAttackerDestinationMapping.Remove(unit);
        }
    }

    public void AddAttackMoveAttacker(IUnit unit, Vector2 destination)
    {
        return;
            if (!mAttackerDestinationMapping.ContainsKey(unit))
            {
                mObjectHandler.QueueMove(unit, destination, false);
                mAttackerDestinationMapping.Add(unit, new Tuple<int, int>(mGrid.TranslateToGrid(unit.Position) ,mGrid.TranslateToGrid(destination)));
            }
            else
            {
                mObjectHandler.QueueMove(unit, destination, false);
                mAttackerDestinationMapping[unit] = new Tuple<int, int>(mGrid.TranslateToGrid(unit.Position) ,mGrid.TranslateToGrid(destination));
            }
    }

    public void AddAttackMoveAttackers(List<IUnit> units, Vector2 destination)
    {
        foreach (var unit in units)
        {
            if (!mAttackerDestinationMapping.ContainsKey(unit))
            {
                mObjectHandler.QueueMove(unit, destination, false);
                mAttackerDestinationMapping.Add(unit, new Tuple<int, int>(mGrid.TranslateToGrid(unit.Position) ,mGrid.TranslateToGrid(destination)));
            }
            else
            {
                mObjectHandler.QueueMove(unit, destination, false);
                mAttackerDestinationMapping[unit] = new Tuple<int, int>(mGrid.TranslateToGrid(unit.Position) ,mGrid.TranslateToGrid(destination));
            }
            
        }
    }

    public void DoDamage(IUnit attacker)
    {
        return;
        if (mAttackerTargetMapping.ContainsKey(attacker)) 
        {
            // this allows workers to do more damage to trees and stones
            float damageMultiplier = 1f;

            if (mAttackerTargetMapping[attacker] is IMapObject)
            {
                if (attacker is Worker)
                {
                    damageMultiplier = 12.5f;
                }
                else if (attacker is Archer)
                {
                    damageMultiplier = .75f;
                }
            }

            mAttackerTargetMapping[attacker].GetDamage((int)((float)attacker.Damage * damageMultiplier));
            
            if(attacker is IMelee) mSoundManager.PlaySound("SoundAssets/Sword_sound", 1, false, false);
        }
    }

    public void RemoveTarget(IAttackableObject target)
    {
        return;
        var keysToRemove = mAttackerTargetMapping.Where(kvp => kvp.Value == target)
            .Select(kvp => kvp.Key)
            .ToList();
        foreach (var attacker in keysToRemove)
        {
            mAttackerTargetMapping.Remove(attacker);
            var newTarget = ScanForTarget(attacker, false, false);
            if (newTarget is not null) mAttackerTargetMapping[attacker] = newTarget;
        }
    }
    
    public IAttackableObject ScanForTarget(IUnit attacker, bool isMapObjectOk, bool onlyTargetBuildings)
    {
        return null;
        int rangeInGridSquares = (attacker is IMelee) ? 1 : 2;
        
        var targetSquares = new HashSet<int>{mGrid.TranslateToGrid(attacker.Position)};
        var squaresToAdd = new HashSet<int>();
        for (int i = 0; i < rangeInGridSquares; ++i)
        {
            foreach (var square in targetSquares)
            {
                foreach (var neighbor in mGrid.GetNeighbors(square))
                {
                    squaresToAdd.Add(neighbor);
                }
            }
            targetSquares.UnionWith(squaresToAdd);
            squaresToAdd.Clear();
        }
        
        float minDistance = float.MaxValue;
        IUnit targetUnit = null;
        // first look for a unit in neighboring grid squares
        foreach (var square in targetSquares)
        {
            // search all units for the closest unit
            var attackableObjects = mGrid.GetAttackableObjectFromSlot(square);
            if (attackableObjects != null)
            {
                foreach (var attackableObject in attackableObjects)
                {
                    var unit = attackableObject as IUnit;
                    if (unit is not null && unit.Team != attacker.Team && !unit.IsDead)
                    {
                        if (targetUnit is null) targetUnit = unit;
                        else if ((unit.Position - attacker.Position).Length() <
                                 (minDistance))
                        {
                            targetUnit = unit;
                            minDistance = (targetUnit.Position - attacker.Position).Length();
                        }
                    }
                }
            }
        }
        if (targetUnit is not null && !onlyTargetBuildings) return targetUnit;
        // then look for enemy buildings
        foreach (var square in targetSquares)
        {
            var building = mGrid.GetStaticObjectFromSlot(square) as IBuilding;
            if (building is not null && building.Team != attacker.Team)
            {
                return building;
            }
        }
        if (isMapObjectOk)
        {
            foreach (var square in targetSquares)
            {
                var staticObject = mGrid.GetStaticObjectFromSlot(square);
                if (staticObject is not null && staticObject.Team != attacker.Team)
                {
                    if (Debugger.IsAttached)
                    {
                        Console.WriteLine("Attack Nature");
                    }
                    return staticObject;
                }
            }
        }
        return null;
    }
    
    public void ShootArrow(IRanged attacker)
    {
        mSoundManager.PlaySound("SoundAssets/Arrow_sound", 1, false, false);
        if (mAttackerTargetMapping.ContainsKey(attacker))
        {
            mWorldScreen.AddArrow(attacker.Position, mAttackerTargetMapping[attacker].Position, attacker.Damage, attacker.Team);
        }
        else
        {
            attacker.StopAttacking();
        }
    }
}
