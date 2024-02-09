using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataPersistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Swordsman_Saga.Engine.ObjectManagement;

interface IRanged: IUnit, IGameObject
{
    
    bool IUnit.IsInAttackRange(Vector2 targetLocation)
    {
        if ((Position - targetLocation).Length() < AttackRange) {return true;}
        return false;
    }

    void IUnit.DoAttack()
    {
        FightManager.ShootArrow(this);
    }
    
    // necessarry to cancel the animation, cause else they would finish the shooting an arrow animation but shoot no arrow,
    // when target dies while aiming
    void StopAttacking()
    {
        Action = (int)ActionId.Standing;
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
