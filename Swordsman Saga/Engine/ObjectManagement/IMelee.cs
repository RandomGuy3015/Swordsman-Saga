using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;

namespace Swordsman_Saga.Engine.ObjectManagement;

interface IMelee : IUnit
{
    bool IUnit.IsInAttackRange(Vector2 targetLocation)
    {
        Vector2 distance = Position - targetLocation;
        // punish high y values harder to make this infinity-sign shaped attack Range for Melee units
        // cause Attack Animation is only to left or to right
        if (Math.Abs(distance.X) + Math.Abs((distance.Y) * 3) < AttackRange) {return true;}
        return false;
    }

    void IUnit.DoAttack()
    {
        FightManager.DoDamage(this);
    }
    void IDataPersistence.SaveData(ref GameData data)
    {
        if (data.mHealth.ContainsKey(Id))
        {
            data.mHealth.Remove(Id);
        }
        if (data.mObjectPosition.ContainsKey(Id))
        {
            data.mObjectPosition.Remove(Id);
        }

        if (data.mGameObjects.ContainsKey(Id))
        {
            data.mGameObjects.Remove(Id);
        }
        if (data.mUnitPlayer.ContainsKey(Id))
        {
            data.mUnitPlayer.Remove(Id);
        }

        data.mUnitPlayer.Add(Id, Team);
        data.mGameObjects.Add(Id, TypeToString());
        data.mHealth.Add(Id, Health);
        data.mObjectPosition.Add(Id, Position);
    }
}