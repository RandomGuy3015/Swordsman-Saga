using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.Menus;

namespace Swordsman_Saga.Engine.ScreenManagement.MenuManagement
{
    abstract class MenuBaseScreen<TButtonNumeration> : IScreen
    {
        protected Texture2D BackgroundTexture { get; private set; }
        protected bool ShouldDrawBackground { get; set; } = true;

        public ScreenManager ScreenManager { get; }
        protected readonly List<Button> mButtons;

        public bool UpdateLower { get; protected set; }
        public bool DrawLower { get; protected set; }

        protected Texture2D ButtonTexture { get; private set; }
        protected Vector2 ButtonSize { get; private set; }
        protected DynamicContentManager mContentManager;
        protected GraphicsDeviceManager mGraphicsDeviceManager;
        protected SoundManager mSoundManager; // falls Menu BackgroundMusic oder ButtonSelectSound o.ä. hat, kann sonst raus
        protected InputState mInputState;
        protected InputManager mInputManager;

        protected interface IButtonNumeration
        {
        }

        public MenuBaseScreen(ScreenManager screenManager, DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager)
        {
            mInputManager = inputManager;
            ScreenManager = screenManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mSoundManager = soundManager;
            mButtons = new List<Button>();
            Initialize();
        }

        public MenuBaseScreen(ScreenManager screenManager, DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager, List<string> buttonNames)
        {
            mInputManager = inputManager;
            ScreenManager = screenManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mSoundManager = soundManager;
            mButtons = new List<Button>();
            Initialize(buttonNames);
        }

        protected virtual void Initialize(List<string> buttonNames)
        {
            ButtonTexture = mContentManager.Load<Texture2D>("ButtonNew");
            ButtonSize = new Vector2(250, 30);
            Texture2D buttonHoverTexture = mContentManager.Load<Texture2D>("ButtonNewHover");

            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight / 2 - (((int)ButtonSize.Y * (Enum.GetValues(typeof(TButtonNumeration))).Length) / 2);


            foreach (TButtonNumeration buttonEnum in Enum.GetValues(typeof(TButtonNumeration)))
            {
                mButtons.Add(new Button(buttonHoverTexture, ButtonTexture,
                    new Vector2(mGraphicsDeviceManager.PreferredBackBufferWidth / 2 - ButtonSize.X / 2,
                        startY + (int)(object)buttonEnum * (int)ButtonSize.Y * 3 / 2),
                    ButtonSize, mContentManager, mInputManager,mSoundManager, buttonNames.First()));
                buttonNames.RemoveAt(0);
                mButtons[(int)(object)buttonEnum].Clicked += GetButtonClickHandler(buttonEnum);
            }
            BackgroundTexture = mContentManager.Load<Texture2D>("SwordsmanSaga");
        }

        protected virtual void Initialize()
        {
            ButtonTexture = mContentManager.Load<Texture2D>("ButtonNew");
            ButtonSize = new Vector2(250, 30);
            Texture2D buttonHoverTexture = mContentManager.Load<Texture2D>("ButtonNewHover");

            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight / 2 - (((int)ButtonSize.Y * (Enum.GetValues(typeof(TButtonNumeration))).Length) / 2);


            foreach (TButtonNumeration buttonEnum in Enum.GetValues(typeof(TButtonNumeration)))
            {
                mButtons.Add(new Button(buttonHoverTexture, ButtonTexture,
                    new Vector2(mGraphicsDeviceManager.PreferredBackBufferWidth / 2 - ButtonSize.X / 2,
                        startY + (int)(object)buttonEnum * (int)ButtonSize.Y * 3 / 2),
                    ButtonSize, mContentManager, mInputManager, mSoundManager, Enum.GetName(typeof(TButtonNumeration), buttonEnum)));
                mButtons[(int)(object)buttonEnum].Clicked += GetButtonClickHandler(buttonEnum);
            }
            BackgroundTexture = mContentManager.Load<Texture2D>("SwordsmanSaga");

        }

        protected abstract Action GetButtonClickHandler(TButtonNumeration buttonEnum);

        public virtual void Update(GameTime gameTime, InputState inputState)
        {
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight / 2 - ((((int)ButtonSize.Y * 3 / 2) * (Enum.GetValues(typeof(TButtonNumeration))).Length) / 2);
            // Loop counter
            int n = 0;

            foreach (Button button in mButtons)
            {
                button.ChangePosition(
                    mGraphicsDeviceManager.PreferredBackBufferWidth / 2 - ButtonSize.X / 2,
                    startY + n * (int)ButtonSize.Y * 3 / 2);
                button.Update(inputState);
                n++;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if (ShouldDrawBackground && BackgroundTexture != null)
            {
                spriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, mGraphicsDeviceManager.PreferredBackBufferWidth, mGraphicsDeviceManager.PreferredBackBufferHeight), Color.White);
            }

            foreach (Button button in mButtons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
        public virtual void DrawWithoutButtons(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if (ShouldDrawBackground && BackgroundTexture != null)
            {
                spriteBatch.Draw(BackgroundTexture, new Rectangle(0, 0, mGraphicsDeviceManager.PreferredBackBufferWidth, mGraphicsDeviceManager.PreferredBackBufferHeight), Color.White);
            }


            spriteBatch.End();
        }
    }
}