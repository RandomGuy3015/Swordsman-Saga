using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Swordsman_Saga.Engine.ObjectManagement;

namespace Swordsman_Saga.Engine.DataTypes.Grids
{
    class GridLocation
    {
        //possible A* prep
        public bool IsFilled { get; private set; }
        public bool IsPathable { get; private set; }
        public bool IsPassable { get; private set; }
        public IStaticObject StaticObject { get; private set; }
        private float mFScore, mCost, mCurrentDist;

        public GridLocation()
        {
            IsFilled = false;
            IsPathable = true;
            IsPassable = true;
        }

        public void SetToFilled(IStaticObject staticObject, bool impassable)
        {
            IsFilled = true;
            IsPassable = impassable;
            StaticObject = staticObject;
        }


        public void Clear(bool impassable)
        {
            IsFilled = false;
            IsPassable = impassable;
            StaticObject = null;
        }
    }
}
