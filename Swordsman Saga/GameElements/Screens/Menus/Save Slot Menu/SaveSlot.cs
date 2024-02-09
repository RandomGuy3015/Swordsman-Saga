using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swordsman_Saga.Engine.DataPersistence.Data;

namespace Swordsman_Saga.GameElements.Screens.Menus.Save_Slot_Menu
{
    internal class SaveSlot
    {
        private string mProfileId = "";
        private bool HasData { get; set; }

        private GameData mData;

        public SaveSlot(string profileId)
        {
            mProfileId = profileId;
        }

        public void SetData(GameData data)
        {
            if (data == null)
            {
                HasData = false;
            }
            else
            {
                mData = data;
                HasData = true;
            }
        }

        public string GetProfileId() { return mProfileId; }
    }
}
