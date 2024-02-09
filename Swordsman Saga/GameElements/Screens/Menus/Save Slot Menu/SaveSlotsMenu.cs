using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SoundManagement;

namespace Swordsman_Saga.GameElements.Screens.Menus.Save_Slot_Menu
{
    class SaveSlotsMenu: MenuBaseScreen<SaveSlotsMenu.ButtonNumeration>
    {
        private List<SaveSlot> mSaveSlots;
        private bool mSaveMenu;

        public enum ButtonNumeration
        {
            SaveSpace1,
            SaveSpace2,
            SaveSpace3,
            SaveSpace4,
            Exit
            
        }
        public SaveSlotsMenu(ScreenManager screenManager, DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager, bool saveMenu) : base(screenManager, contentManager, graphicsDeviceManager, soundManager, inputManager)
        {
            mSaveSlots = new List<SaveSlot>();
            mSaveMenu = saveMenu;
            ShouldDrawBackground = false;

            var saveSlots = Enum.GetValues(typeof(ButtonNumeration)).   Cast<ButtonNumeration>().ToList();
            int id = 0;
            foreach (var slot in saveSlots)
            {
                mSaveSlots.Add(new SaveSlot(id.ToString()));
                id++;
            }
            Dictionary<string, GameData> profilesData = DataPersistenceManager.Instance.GetAllProfilesGameData();
            foreach (var saveSlot in mSaveSlots)
            {
                GameData profileData = null;
                profilesData.TryGetValue(saveSlot.GetProfileId(), out profileData);
                saveSlot.SetData(profileData);
            }
        }
        protected override void Initialize()
        {
            UpdateLower = false;
            DrawLower = true;
            // TODO: Background Picture 
            base.Initialize();
        }

        protected override Action GetButtonClickHandler(ButtonNumeration buttonEnum)
        {
            switch (buttonEnum)
            {
                case ButtonNumeration.SaveSpace1:
                    return SaveLoadGameSlot1;
                case ButtonNumeration.SaveSpace2:
                    return SaveLoadGameSlot2;
                case ButtonNumeration.SaveSpace3:
                    return SaveLoadGameSlot3;
                case ButtonNumeration.SaveSpace4:
                    return SaveLoadGameSlot4;
                case ButtonNumeration.Exit:
                    return Exit;
                default:
                    return null;
            }
        }

        private void SaveLoadGameSlot1()
        {
            DataPersistenceManager.Instance.SetSelectedProfileId("0");
            if (mSaveMenu)
            {
                DataPersistenceManager.Instance.SaveGame();
                ScreenManager.ResetToFirst();
            }
            else
            {
                ScreenManager.ResetToFirst();
                ScreenManager.AddScreen<WorldScreen>(false, false, false, false, -1);
            }
        }

        private void SaveLoadGameSlot2()
        {
            DataPersistenceManager.Instance.SetSelectedProfileId("1");
            if (mSaveMenu)
            {
                DataPersistenceManager.Instance.SaveGame();
                ScreenManager.ResetToFirst();
            }
            else
            {
                ScreenManager.ResetToFirst();
                ScreenManager.AddScreen<WorldScreen>(false, false, false, false, -1);
            }
        }

        private void SaveLoadGameSlot3()
        {
            DataPersistenceManager.Instance.SetSelectedProfileId("2");
            if (mSaveMenu)
            {
                DataPersistenceManager.Instance.SaveGame();
                ScreenManager.ResetToFirst();
            }
            else
            {
                ScreenManager.ResetToFirst();
                ScreenManager.AddScreen<WorldScreen>(false, false, false, false, -1);
            }
        }

        private void SaveLoadGameSlot4()
        {
            DataPersistenceManager.Instance.SetSelectedProfileId("3");
            if (mSaveMenu)
            {
                DataPersistenceManager.Instance.SaveGame();
                ScreenManager.ResetToFirst();
            }
            else
            {
                ScreenManager.ResetToFirst();
                ScreenManager.AddScreen<WorldScreen>(false, false, false, false, -1);
            }
        }

        private void Exit()
        {
            ScreenManager.RemoveScreen();
        }
    }
}
