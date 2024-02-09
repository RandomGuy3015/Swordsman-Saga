using Microsoft.Xna.Framework;
using Swordsman_Saga.Engine.DataTypes.Grids;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.SoundManagement;
using System.Collections.Generic;

namespace Swordsman_Saga.Engine.ObjectManagement;

public interface IUpdateableObject
{
    void Update(GameTime gameTime);
}