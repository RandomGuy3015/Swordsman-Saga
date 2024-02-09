using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.SoundManagement;

namespace Swordsman_Saga.GameElements.Screens.HUDs
{
    class TownhallOverlay: IHud
    {
        public bool IsVisiblebtn { get; set; }
        public IBuilding mSelectedBuilding;
        private DynamicContentManager mContentManager;
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private ObjectHandler mObjectHandler;
        private InputManager mInputManager;
        private SoundManager mSoundManager;
        private Color mFontColor;
        private SpriteFont mFont;

        public void SetSelectedBuilding(IBuilding building)
        {
            mSelectedBuilding = building;
        }
        public TownhallOverlay(DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, InputManager inputManager, SoundManager soundManager, ObjectHandler objectHandler)
        {
            mSoundManager = soundManager;
            mInputManager = inputManager;
            mContentManager = contentManager;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mObjectHandler = objectHandler;
            mFontColor = Color.Black;
            mFont = contentManager.Load<SpriteFont>("basic_font");
            Initialize();
        }
        private void Initialize()
        {
            IsVisiblebtn = false;
        }

        public void ToggleVisibility()
        {
            IsVisiblebtn = !IsVisiblebtn;
        }



        public void Update(GameTime gameTime, InputState inputState)
        {
            if (!IsVisiblebtn) { return; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisiblebtn) { return; }
            spriteBatch.DrawString(mFont, "Townhall", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(mFont, "Health: " + mSelectedBuilding.Health, new Vector2(10, 30), Color.White);

        }


        public void UpdatePosition()
        {
        }
    }
}
