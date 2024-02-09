using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.SettingsManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.Menus;

namespace Swordsman_Saga.GameLogic
{
     internal sealed class Game1 : Game
    {
        private GraphicsDeviceManager mGraphics;
        private SpriteBatch mSpriteBatch;
        private ScreenManager mScreenManager;
        private DynamicContentManager mContentManager;
        private SoundManager mSoundManager;
        private InputManager mInputManager;
        private SettingsManager mSettingsManager;
        private DataPersistenceManager mDataPersistenceManager;
        private ResizeHandler mResizeHandler;
        private AchievementsManager mAchievementsManager;
        private StatisticsManager mStatisticsManager;
        private KeybindingManager mKeybindingManager;
        private FightManager mFightManager;
        // Save/Load ?
        
        public Game1()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Make the window resizable
            Window.AllowUserResizing = true;
            mGraphics.HardwareModeSwitch = false;
        }

        protected override void Initialize()
        {
            // Manager Initialization
            mContentManager = new DynamicContentManager(Content);
            mSettingsManager = new SettingsManager();
            mKeybindingManager = new KeybindingManager();
            mInputManager = new InputManager(mKeybindingManager);
            mResizeHandler = new ResizeHandler(mGraphics, mInputManager, Window);
            mSoundManager = new SoundManager(mContentManager, mSettingsManager);
            mStatisticsManager = new StatisticsManager();
            mAchievementsManager = new AchievementsManager();
            mFightManager = new FightManager(mInputManager, mSoundManager);
            mDataPersistenceManager = new DataPersistenceManager(mKeybindingManager, mFightManager, mStatisticsManager, mAchievementsManager, mSoundManager);
            // TO IMPLEMENT: mObjectManager = new ObjectManager(mContentManager);
            mScreenManager = new ScreenManager(mSoundManager, mInputManager, mContentManager, mGraphics, GraphicsDevice, mResizeHandler, mKeybindingManager, mAchievementsManager, mStatisticsManager, mFightManager);
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Content laden
            mSoundManager.PlayBackgroundMusic("SoundAssets/background_music", .25f);
            mDataPersistenceManager.LoadMainMenu();
            mScreenManager.AddScreen<MainMenu>(false, false, false, false, -1);
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive) 
            { 
                mScreenManager.Update(gameTime); 
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            mScreenManager.Draw(mSpriteBatch);
            base.Draw(gameTime);
        }
    }
}