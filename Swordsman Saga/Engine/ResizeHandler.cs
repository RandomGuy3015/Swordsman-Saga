using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine
{
    public class ResizeHandler
    {
        private readonly GraphicsDeviceManager mGraphics;
        private readonly InputManager mInputManager;
        private bool mIsTogglingFullScreen;

        public ResizeHandler(GraphicsDeviceManager graphics, InputManager inputManager, GameWindow window)
        {
            mGraphics = graphics;
            mInputManager = inputManager;
            window.ClientSizeChanged += OnResize;
            mIsTogglingFullScreen = false;
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            if (!mIsTogglingFullScreen && mGraphics.GraphicsDevice != null && !mGraphics.IsFullScreen)
            {
                // Handle resizing here, e.g., updating the viewport
                mGraphics.PreferredBackBufferWidth = mGraphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
                mGraphics.PreferredBackBufferHeight = mGraphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
                mGraphics.ApplyChanges();

            }

            // Additional resize logic can be added here
        }

        public void Update(InputState inputState)
        {

            // Check for the 'F11' key trigger to toggle full-screen mode
            if (mInputManager.IsKeyTriggered(Keys.F11))
            {
                mIsTogglingFullScreen = true;
                mGraphics.ToggleFullScreen();
                mGraphics.PreferredBackBufferWidth = mGraphics.GraphicsDevice.DisplayMode.Width - 80;
                mGraphics.PreferredBackBufferHeight = mGraphics.GraphicsDevice.DisplayMode.Height - 80;
                mGraphics.ApplyChanges();
                mIsTogglingFullScreen = false;
            }

            if (mGraphics.IsFullScreen)
            {
                mGraphics.PreferredBackBufferWidth = mGraphics.GraphicsDevice.DisplayMode.Width;
                mGraphics.PreferredBackBufferHeight = mGraphics.GraphicsDevice.DisplayMode.Height;
            }
        }
    }
}
