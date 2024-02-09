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
    class DifficultyMenu : MenuBaseScreen<DifficultyMenu.ButtonNumeration>
    {
        public enum ButtonNumeration
        {
            Easy,
            Medium,
            Hard,
            Exit
        }

        public DifficultyMenu(ScreenManager screenManager, DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager) : base(screenManager, contentManager, graphicsDeviceManager, soundManager, inputManager)
        {
            ShouldDrawBackground = true;
        }

        protected override void Initialize()
        {
            UpdateLower = false;
            DrawLower = true;
            base.Initialize();
        }

        protected override Action GetButtonClickHandler(ButtonNumeration buttonEnum)
        {
            return buttonEnum switch
            {
                ButtonNumeration.Easy => NewGameEasy,
                ButtonNumeration.Medium => NewGameMedium,
                ButtonNumeration.Hard => NewGameHard,
                ButtonNumeration.Exit => Exit,
                _ => null,
            };
        }

        private void NewGameEasy()
        {
            ScreenManager.AddScreen<WorldScreen>(true, false, false, false, 0);
        }
        private void NewGameMedium()
        {
            ScreenManager.AddScreen<WorldScreen>(true, false, false, false, 1);
        }
        private void NewGameHard()
        {
            ScreenManager.AddScreen<WorldScreen>(true, false, false, false, 2);
        }

        private void Exit()
        {
            ScreenManager.ResetToFirst();
        }
    }
}