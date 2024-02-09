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
    sealed class KeyBindingsScreen : IScreen
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
        private readonly KeybindingManager mKeybindingManager;

        // Buttons
        private enum ButtonType
        {
            Exit,
        }

        private void InitializeOptions()
        {
            mKeybindFields = new List<IKeybindField>();
            //TODO add options to list.
            mKeybindFields.Add(new KeybindField(mKeybindingManager, new KeyBind(ActionType.MoveCameraUp, mKeybindingManager.GetKeyBinding(ActionType.MoveCameraUp)), hoverTexture: mButtonHoverTexture, texture: mKeybindFieldTexture, font: mKeybindFieldFont, mInputManager, mKeybindFieldWidth, mKeybindFieldHeight, (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2, 200));
            mKeybindFields.Add(new KeybindField(mKeybindingManager, new KeyBind(ActionType.MoveCameraDown, mKeybindingManager.GetKeyBinding(ActionType.MoveCameraDown)), hoverTexture: mButtonHoverTexture, texture: mKeybindFieldTexture, font: mKeybindFieldFont, mInputManager, mKeybindFieldWidth, mKeybindFieldHeight, (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2, 300));
            mKeybindFields.Add(new KeybindField(mKeybindingManager, new KeyBind(ActionType.MoveCameraLeft, mKeybindingManager.GetKeyBinding(ActionType.MoveCameraLeft)), hoverTexture: mButtonHoverTexture, texture: mKeybindFieldTexture, font: mKeybindFieldFont, mInputManager, mKeybindFieldWidth, mKeybindFieldHeight, (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2, 400));
            mKeybindFields.Add(new KeybindField(mKeybindingManager, new KeyBind(ActionType.MoveCameraRight, mKeybindingManager.GetKeyBinding(ActionType.MoveCameraRight)), hoverTexture: mButtonHoverTexture, texture: mKeybindFieldTexture, font: mKeybindFieldFont, mInputManager, mKeybindFieldWidth, mKeybindFieldHeight, (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2, 500));
            mKeybindFields.Add(new KeybindField(mKeybindingManager, new KeyBind(ActionType.AttackMove, mKeybindingManager.GetKeyBinding(ActionType.AttackMove)), hoverTexture: mButtonHoverTexture, texture: mKeybindFieldTexture, font: mKeybindFieldFont, mInputManager, mKeybindFieldWidth, mKeybindFieldHeight, (int)(mGraphicsDeviceManager.PreferredBackBufferWidth - (int)mButtonSize.X) / 2, 600));

        }



        // Fields to enter numbers from keyboard into.
        private List<IKeybindField> mKeybindFields;

        private readonly List<Button> mButtons = new List<Button>();

        private readonly int mKeybindFieldWidth = 250;
        private readonly int mKeybindFieldHeight = 30;
        private readonly Vector2 mButtonSize;
        private readonly Texture2D mButtonTexture;
        private readonly Texture2D mKeybindFieldTexture;
        private readonly SpriteFont mKeybindFieldFont;
        private readonly Vector2 mOptionFieldSize;
        private readonly Texture2D mButtonHoverTexture;


        // Constructor
        public KeyBindingsScreen(ScreenManager screenManager, DynamicContentManager contentManager,
            GraphicsDeviceManager graphicsDeviceManager, SoundManager soundManager, InputManager inputManager,
            KeybindingManager keybindingManager)
        {
            mKeybindingManager = keybindingManager;
            mSoundManager = soundManager;
            mInputManager = inputManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mButtonTexture = mContentManager.Load<Texture2D>("ButtonNew");
            mButtonHoverTexture = mContentManager.Load<Texture2D>("ButtonNewHover");

            mKeybindFieldTexture = mContentManager.Load<Texture2D>("ButtonNew");
            mKeybindFieldFont = mContentManager.Load<SpriteFont>("basic_font");
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
                }

                if (button != null)
                {
                    mButtons.Add(button);
                }
            }
        }

        public void Update(GameTime gameTime, InputState inputState)
        {
            // Check for the "Exit" action
            if (mInputManager.IsActionInputted(inputState, ActionType.Exit))
            {
                // Handle the "Exit" action
                HandleButtonClick(ButtonType.Exit);
            }
            AdjustPositions();

            // Update other components
            UpdateButtons(inputState);
            UpdateKeybindFields(inputState);
        }

        private void AdjustPositions()
        {
            int startY = mGraphicsDeviceManager.PreferredBackBufferHeight / 2 - ((int)mButtonSize.Y * 4) + 8;
            // Loop counter
            int n = 0;

            foreach (var field in mKeybindFields)
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
        
        private void UpdateKeybindFields(InputState inputState)
        {
            foreach (var field in mKeybindFields)
            {
                field.Update(inputState);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            DrawButtons(spriteBatch);
            DrawKeybindFields(spriteBatch);
            spriteBatch.End();
        }
        public void DrawWithoutButtons(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.End();
        }
        private void DrawButtons(SpriteBatch spriteBatch)
        {
            foreach (var button in mButtons)
            {
                button?.Draw(spriteBatch);
            }
        }
        
        private void DrawKeybindFields(SpriteBatch spriteBatch)
        {
            foreach (var field in mKeybindFields)
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
            }
        }
        
        

        private void Exit()
        {
            ScreenManager.RemoveScreen();
        }

        private void ChangeKeybindings()
        {
            //TODO change keybindins
        }
        
    }
}

