using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DataTypes.Grids;


namespace Swordsman_Saga.Engine.ObjectManagement
{
    interface ICollidableObject: IGameObject
    {
        bool CheckForCollision(DiamondGrid grid, IGameObject gameObject)
        {
            List<ICollidableObject> nearObjects = new List<ICollidableObject>();
            int gridLocation = grid.TranslateToGrid(new Vector2(gameObject.HitboxRectangle.X + gameObject.HitboxOffset.X, gameObject.HitboxRectangle.Y + gameObject.HitboxOffset.Y));

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (grid.mGridContent.ContainsKey(gridLocation))
                    {
                        foreach (var nearObject in grid.mGridContent[gridLocation])
                        {
                            if (nearObject is ICollidableObject collidableObject)
                            {
                                nearObjects.Add(collidableObject);
                            }
                        }
                    }
                }
            }

            if (gameObject is ICollidableObject gameCollidableObject)
            {
                nearObjects.Remove(gameCollidableObject);
            }
            foreach (ICollidableObject collidableObject in nearObjects)
            {
                if (gameObject.HitboxRectangle.Intersects(collidableObject.HitboxRectangle) && gameObject != collidableObject)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
