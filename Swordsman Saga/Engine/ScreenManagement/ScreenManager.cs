using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens;
using Swordsman_Saga.GameElements.Screens.HUDs;
using Swordsman_Saga.GameElements.Screens.Menus;
using Swordsman_Saga.GameElements.Screens.Menus.Save_Slot_Menu;

namespace Swordsman_Saga.Engine.ScreenManagement
{
    class ScreenManager
    {
        private SoundManager mSoundManager;
        private InputManager mInputManager;
        private DynamicContentManager mDynamicContentManager;
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private GraphicsDevice mGraphicsDevice;
        private List<IScreen> mScreenStack = new List<IScreen>();
        private ResizeHandler mResizeHandler;
        private KeybindingManager mKeybindingManager;
        private AchievementsManager mAchievementsManager;
        private StatisticsManager mStatisticsManager;
        private FightManager mFightManager;
        private LayerHandler mLayerHandler;

        public ScreenManager(SoundManager soundManager, InputManager inputManager,
            DynamicContentManager dynamicContentManager, GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, ResizeHandler resizeHandler, KeybindingManager keybindingManager, 
            AchievementsManager achievementsManager, StatisticsManager statisticsManager, FightManager fightManager)
        {
            mSoundManager = soundManager;
            mInputManager = inputManager;
            mDynamicContentManager = dynamicContentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mGraphicsDevice = graphicsDevice;
            mResizeHandler = resizeHandler;
            mKeybindingManager = keybindingManager;
            mAchievementsManager = achievementsManager;
            mStatisticsManager = statisticsManager;
            mFightManager = fightManager;
        }

        public void AddScreen<T>(bool newgame, bool savemenu, bool techdemo, bool win, int difficulty) where T : IScreen
        {
            IScreen screen = CreateScreen<T>(newgame, savemenu, techdemo, win, difficulty);
            mScreenStack.Add(screen);
            /*if (screen is WorldScreen)
            {
                if (newgame)
                {
                    DataPersistenceManager.Instance.NewGame();
                }
                else
                {
                    DataPersistenceManager.Instance.LoadGame(false);
                }
            }*/
        }

        private IScreen CreateScreen<T>(bool newgame, bool savemenu, bool techdemo, bool win, int difficulty)
        {
            // TODO: change to just creating the construction parameters via switch and calling T-Constructor in the end
            // that way not every class needs their own case. and no specific constructor needs to get called
            // One case for Classes belonging to the same interface that need the same parameters
            mInputManager.Flush();
            switch (typeof(T).Name)
            {
                case "MainMenu":
                    return new MainMenu(this, mDynamicContentManager, mGraphicsDeviceManager, mSoundManager, mInputManager);
                case "PauseMenu":
                    return new PauseMenu(this, mDynamicContentManager, mGraphicsDeviceManager, mSoundManager, mInputManager);
                case "DifficultyMenu":
                    return new DifficultyMenu(this, mDynamicContentManager, mGraphicsDeviceManager, mSoundManager, mInputManager);
                case "WorldScreen":
                    return new WorldScreen(this, mDynamicContentManager, mInputManager, mSoundManager, mGraphicsDeviceManager, mGraphicsDevice, mKeybindingManager, 
                        mAchievementsManager, mStatisticsManager, mFightManager, newgame, techdemo, difficulty);
                case "AchievementScreen":
                    return new AchievementScreen(this, mDynamicContentManager, mGraphicsDeviceManager, mSoundManager, mInputManager, mAchievementsManager);
                case "StatisticsScreen":
                    return new StatisticsScreen(this, mDynamicContentManager, mGraphicsDeviceManager, mSoundManager, mInputManager, mStatisticsManager);
                case "OptionsScreen":
                    return new OptionsScreen(this, mDynamicContentManager, mGraphicsDeviceManager, mSoundManager, mInputManager);
                case "AchievementsAndStatisticsScreen":
                    return new AchievementsAndStatisticsScreen(this, mDynamicContentManager, mGraphicsDeviceManager, mSoundManager, mInputManager);
                case "SaveSlotsMenu":
                    return new SaveSlotsMenu(this,
                        mDynamicContentManager,
                        mGraphicsDeviceManager,
                        mSoundManager,
                        mInputManager,
                        savemenu);
                case "KeyBindingsScreen":
                    return new KeyBindingsScreen(this, mDynamicContentManager, mGraphicsDeviceManager, mSoundManager, mInputManager, mKeybindingManager);
                case "WinLoseScreen":
                    return new WinLoseScreen(this,
                        mDynamicContentManager,
                        mGraphicsDeviceManager,
                        mSoundManager,
                        mInputManager,
                        win);
                default:
                    throw new InvalidOperationException("{typeof(T).Name} is missing in CreateScreen-Method in the ScreenManager");
            }
        }

        public void RemoveScreen()
        {
            mScreenStack.RemoveAt(mScreenStack.Count - 1);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            List<IScreen> drawStack = new List<IScreen>();
            var activeScreenIndex = mScreenStack.Count - 1;

            // Start with the active screen
            var activeScreen = mScreenStack[activeScreenIndex];
            drawStack.Add(activeScreen);

            // Add lower screens if DrawLower is true
            while (activeScreenIndex > 0)
            {
                var lowerScreen = mScreenStack[activeScreenIndex - 1];
                if (!lowerScreen.DrawLower)
                {
                    break; // Stop if the next lower screen should not be drawn
                }
                drawStack.Add(lowerScreen);
                activeScreenIndex -= 1;
            }
            drawStack.Reverse();

            // Draw screens
            foreach (var screen in drawStack)
            {
                if (screen == activeScreen)
                {
                    screen.Draw(spriteBatch); // Draw the active screen normally
                }
                else
                {
                    screen.DrawWithoutButtons(spriteBatch); // Draw lower screens without buttons
                }
            }
        }




        public void Update(GameTime gameTime)
        {
            var inputState = mInputManager.Update();
            mResizeHandler.Update(inputState);
            var activeScreenIndex = mScreenStack.Count - 1;
            var activeScreen = mScreenStack[activeScreenIndex];
            activeScreen.Update(gameTime, inputState);
            while (activeScreen.UpdateLower is true)
            {
                activeScreenIndex -= 1;
                activeScreen = mScreenStack[activeScreenIndex];
                activeScreen.Update(gameTime,inputState);
            }
        }

        public void ResetToFirst()
        {
            mScreenStack.RemoveRange(1, mScreenStack.Count - 1);
        }
        
        private void ScreenInit() {}
    }
}
