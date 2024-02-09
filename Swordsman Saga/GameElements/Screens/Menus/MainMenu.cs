using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SettingsManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.Menus.Save_Slot_Menu;

namespace Swordsman_Saga.GameElements.Screens.Menus
{
    class MainMenu : MenuBaseScreen<MainMenu.ButtonNumeration>
    {
        private readonly Texture2D mBackgroundTexture;
        public enum ButtonNumeration
        {
            NewGame,
            LoadGame,
            AchievementsAndStatistics,
            Settings,
            Exit
        }

        public MainMenu(ScreenManager screenManager, DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager) : base(screenManager, contentManager, graphicsDeviceManager, soundManager, inputManager, GetButtonNames())
        {
            ShouldDrawBackground = true;
        }

        protected override void Initialize(List<string> buttonNames)
        {
            UpdateLower = false;
            DrawLower = true;
            //mSoundManager.PlayBackgroundMusic("SoundAssets/background_music", .25f);
            // TODO: Background Picture
            foreach (var button in Enum.GetValues(typeof(ButtonNumeration)))
            {
                buttonNames.Add(GetButtonName((ButtonNumeration)button));
            }
            base.Initialize(buttonNames);
        }
        private static string GetButtonName(ButtonNumeration button)
        {
            switch (button)
            {
                case ButtonNumeration.LoadGame:
                    return "Load Game";
                case ButtonNumeration.NewGame:
                    return "New Game";
                case ButtonNumeration.AchievementsAndStatistics:
                    return "Achievements & Statistics";
                case ButtonNumeration.Settings:
                    return "Settings";
                case ButtonNumeration.Exit:
                    return "Exit";
                default:
                    return button.ToString();
            }
        }
        private static List<string> GetButtonNames()
        {
            var buttonNames = new List<string>();
            foreach (var button in Enum.GetValues(typeof(ButtonNumeration)))
            {
                buttonNames.Add(GetButtonName((ButtonNumeration)button));
            }
            return buttonNames;
        }
        protected override Action GetButtonClickHandler(ButtonNumeration buttonEnum)
        {
            switch (buttonEnum)
            {
                case ButtonNumeration.LoadGame:
                    return LoadGame;
                case ButtonNumeration.NewGame:
                    return NewGame;
                case ButtonNumeration.Exit:
                    return Exit;
                case ButtonNumeration.AchievementsAndStatistics:
                    return ShowAchievementsAndStatistics;
                case ButtonNumeration.Settings:
                    return Settings;
                default:
                    return null;
            }
        }

        private void LoadGame()
        {
            ScreenManager.AddScreen<SaveSlotsMenu>(false, false, false, false, -1);
        }

        private void NewGame()
        {
            ScreenManager.AddScreen<DifficultyMenu>(false, false, false, false, -1);
        }

        private void TechDemo()
        {
            // TODO: change to specific TechDemo World when exists
            ScreenManager.AddScreen<WorldScreen>(true, false, true, false, 0);
        }

        private void Exit()
        {
            DataPersistenceManager.Instance.SaveMainMenu();
            System.Environment.Exit(0);
        }
        private void ShowAchievementsAndStatistics()
        {
            ScreenManager.AddScreen<AchievementsAndStatisticsScreen>(false, false, false, false, -1);
        }
        private void Achievements()
        {
            ScreenManager.AddScreen<AchievementScreen>(false, false, false, false, -1);
        }
        
        private void Statistics()
        {
            ScreenManager.AddScreen<StatisticsScreen>(false, false, false, false, -1);
        }
        
        private void Settings()
        {
            ScreenManager.AddScreen<OptionsScreen>(false, false, false, false, -1);
        }

    }
}