using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Swordsman_Saga.Engine.ObjectManagement
{
    interface IResourceBuilding: IBuilding
    {
        Vector2 UpgradeCost { get; }
        void UpgradeBuilding();
        int Level { get; }

    }
}
