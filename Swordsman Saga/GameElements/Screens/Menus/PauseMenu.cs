using System;
using Microsoft.Xna.Framework;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.Menus.Save_Slot_Menu;

namespace Swordsman_Saga.GameElements.Screens.Menus
{
    class PauseMenu : MenuBaseScreen<PauseMenu.ButtonNumeration>
    {
        public enum ButtonNumeration
        {
            Continue,
            SaveGame,
            LoadGame,
            Options,
            Exit
        }

        public PauseMenu(ScreenManager screenManager, DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager) : base(screenManager, contentManager, graphicsDeviceManager, soundManager, inputManager)
        {
            ShouldDrawBackground = false;
        }

        protected override void Initialize()
        {
            UpdateLower = false;
            DrawLower = true;
            base.Initialize();
        }

        protected override Action GetButtonClickHandler(ButtonNumeration buttonEnum)
        {
            switch (buttonEnum)
            {
                case ButtonNumeration.Continue:
                    return Continue;
                case ButtonNumeration.Options:
                    return Options;
                case ButtonNumeration.SaveGame:
                    return SaveGame;
                case ButtonNumeration.LoadGame:
                    return LoadGame;
                case ButtonNumeration.Exit:
                    return Exit;
                default:
                    return null;
            }
        }

        private void Continue()
        {
            ScreenManager.RemoveScreen();
        }

        private void Options()
        {
            ScreenManager.AddScreen<OptionsScreen>(false, false, false, false, -1);
        }

        private void Exit()
        {
            ScreenManager.ResetToFirst();
        }

        private void SaveGame()
        {
            ScreenManager.AddScreen<SaveSlotsMenu>(false, true, false, false, -1);
        }

        private void LoadGame()
        {
            ScreenManager.AddScreen<SaveSlotsMenu>(false, false, false, false, -1);
        }
    }
}