using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swordsman_Saga.Engine.DataPersistence.Data;

namespace Swordsman_Saga.Engine.DataPersistence
{
    public interface IDataPersistence
    {
        void SaveData(ref GameData data);
    }
}
