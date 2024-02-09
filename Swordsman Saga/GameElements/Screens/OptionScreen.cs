using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SettingsManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.Options;

namespace Swordsman_Saga.GameElements.Screens
{
    sealed class OptionsScreen : IScreen
    {
        protected Texture2D BackgroundTexture { get; private set; }

        // Properties
        public bool UpdateLower { get; private set; } = false;
        public bool DrawLower { get; private set; } = true;
        
        // Managers
        public ScreenManager ScreenManager { get; }
        private readonly DynamicContentManager mContentManager;
        private readonly InputManager mInputManager;
        private readonly SoundManager mSoundManager;
        private readonly GraphicsDeviceManager mGraphicsDeviceManager;

        // Buttons
        private enum ButtonType
        {
            SaveSettings,
            ToggleFullScreen,
            ChangeKeyBindings,
            TechDemo,
            Exit,
        }

        private void InitializeOptions()
        {
            mOptionFields = new List<IOptionField>();
            //TODO add options to list.
            mOptionFields.Add(new PercentageOptionField(new MusicVolumeOption(mSoundManager, mSoundManager.GetBackgroundMusicVolume()), hoverTexture: mButtonHoverTexture, texture: mOptionFieldTexture, font: mOptionFieldFont, mInputManager, mOptionFieldWidth, mOptionFieldHeight, (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2, 200));
            mOptionFields.Add(new PercentageOptionField(new SoundVolumeOption(mSoundManager, mSoundManager.GetMasterSoundVolume()), hoverTexture: mButtonHoverTexture, texture: mOptionFieldTexture, font: mOptionFieldFont, mInputManager, mOptionFieldWidth, mOptionFieldHeight, (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2, 300));
        }



        // Fields to enter numbers from keyboard into.
        private List<IOptionField> mOptionFields;

        private readonly List<Button> mButtons = new List<Button>();

        private readonly int mOptionFieldWidth = 250;
        private readonly int mOptionFieldHeight = 30;
        private readonly Vector2 mButtonSize;
        private readonly Texture2D mButtonTexture;
        private readonly Texture2D mButtonHoverTexture;
        private readonly Texture2D mOptionFieldTexture;
        private readonly SpriteFont mOptionFieldFont;
        private readonly Vector2 mOptionFieldSize;

        // Constructor
        public OptionsScreen(ScreenManager screenManager, DynamicContentManager contentManager,
            GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager)
        {
            mSoundManager = soundManager;
            mInputManager = inputManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mButtonTexture = mContentManager.Load<Texture2D>("ButtonNew");
            mOptionFieldTexture = mContentManager.Load<Texture2D>("ButtonNew");
            mButtonHoverTexture = mContentManager.Load<Texture2D>("ButtonNewHover");
            mOptionFieldFont = mContentManager.Load<SpriteFont>("basic_font");
            BackgroundTexture = mContentManager.Load<Texture2D>("SwordsmanSaga");

            mButtonSize = new Vector2(250, 30);
            mOptionFieldSize = new Vector2(250, 30);
            InitializeButtons();
            InitializeOptions();
            ScreenManager = screenManager;
        }

        // Initialization Methods
        private void InitializeButtons()
        {
            int buttonCount = Enum.GetValues(typeof(ButtonType)).Length;
            int startY = (mGraphicsDeviceManager.PreferredBackBufferHeight -
                          (buttonCount * (int)mButtonSize.Y + (buttonCount - 1) * 10)) / 2;

            foreach (ButtonType buttonType in Enum.GetValues(typeof(ButtonType)))
            {
                int buttonX = (mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2;
                Button button = null;

                switch (buttonType)
                {
                    case ButtonType.Exit:
                        button = new Button(mButtonHoverTexture, mButtonTexture,
                            new Vector2(buttonX, startY),
                            mButtonSize,
                            mContentManager,
                            mInputManager,
                            mSoundManager,
                            "Exit");
                        button.Clicked += () => Exit();
                        break;
                    case ButtonType.ChangeKeyBindings:
                        button = new Button(mButtonHoverTexture, mButtonTexture,
                            new Vector2(buttonX, startY + 1 * (mButtonSize.Y + 10)),
                            mButtonSize,
                            mContentManager,
                            mInputManager,
                            mSoundManager,
                            "Change key bindings");
                        button.Clicked += ChangeKeybindings;
                        break;
                    case ButtonType.TechDemo:
                        button = new Button(mButtonHoverTexture, mButtonTexture,
                            new Vector2(buttonX, startY + 1 * (mButtonSize.Y + 10)),
                            mButtonSize,
                            mContentManager,
                            mInputManager,
                            mSoundManager,
                            "Tech Demo");
                        button.Clicked += TechDemo;
                        break;
                }

                if (button != null)
                {
                    mButtons.Add(button);
                }
            }
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            AdjustPositions();
            // Check for the "Exit" action
            if (mInputManager.IsActionInputted(inputState, ActionType.Exit))
            {
                // Handle the "Exit" action
                HandleButtonClick(ButtonType.Exit);
            }


            // Update other components
            UpdateButtons(inputState);
            UpdateOptionFields(inputState);
        }

        private void AdjustPositions()
        {
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight / 2 - ((int)mButtonSize.Y * 4) + 8;
            // Loop counter
            int n = 0;

            foreach (var field in mOptionFields)
            {
                field.ChangePosition(
                    (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2,
                    startY + n * (int)mButtonSize.Y * 3 / 2);
                n++;
            }


            foreach (Button button in mButtons)
            {
                button.ChangePosition(
                    (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2,
                    startY + n * (int)mButtonSize.Y * 3 / 2);
                n++;
            }


        }

        private void UpdateButtons(InputState inputState)
        {
            foreach (var button in mButtons)
            {
                button?.Update(inputState);
            }
        }
        
        private void UpdateOptionFields(InputState inputState)
        {
            foreach (var field in mOptionFields)
            {
                field.Update(inputState);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            DrawButtons(spriteBatch);
            DrawOptionFields(spriteBatch);
            spriteBatch.End();
        }
        public void DrawWithoutButtons(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.End();
        }
        private void SaveSettings()
        {
            //TODO save settings, each setting should correspond to an object.
        }

        private void DrawButtons(SpriteBatch spriteBatch)
        {
            foreach (var button in mButtons)
            {
                button?.Draw(spriteBatch);
            }
        }
        
        private void DrawOptionFields(SpriteBatch spriteBatch)
        {
            foreach (var field in mOptionFields)
            {
                field.Draw(spriteBatch);
            }
        }

        // Event Handling
        private void HandleButtonClick(ButtonType buttonType)
        {
            switch (buttonType)
            {
                case ButtonType.Exit:
                    ScreenManager.ResetToFirst();
                    break;
                case ButtonType.SaveSettings:
                    SaveSettings();
                    break;
                case ButtonType.ChangeKeyBindings:
                    // Toggle mute
                    ChangeKeybindings();
                    break;
                case ButtonType.ToggleFullScreen:
                    // Toggle fullscreen
                    ToggleFullscreen();
                    break;
                case ButtonType.TechDemo:
                    // Toggle fullscreen
                    TechDemo();
                    break;
            }
        }


        private void TechDemo()
        {
            // TODO: change to specific TechDemo World when exists
            ScreenManager.AddScreen<WorldScreen>(true, false, true, false, 0);
        }
        private void Exit()
        {
            ScreenManager.RemoveScreen();
        }

        private void ChangeKeybindings()
        {
            ScreenManager.AddScreen<KeyBindingsScreen>(false, false, false, false, -1);
        }

        private void ToggleFullscreen()
        {
            // Toggle fullscreen mode
            mGraphicsDeviceManager.ToggleFullScreen();
        }
    }
}

