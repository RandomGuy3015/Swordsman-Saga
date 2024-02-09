using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.Engine.ScreenManagement.MenuManagement;
using Swordsman_Saga.Engine.DynamicContentManagement;
using System.Diagnostics;
using MonoGame.Extended.TextureAtlases;
using Swordsman_Saga.Engine.DataPersistence.Data;

namespace Swordsman_Saga.GameElements.Screens.HUDs
{
    public class ResourceHud: IHud
    {
        public int StoneCount { get; set; }
        public int WoodCount { get; set; }
        public int AIStoneCount { get; set; }
        public int AIWoodCount { get; set; }

        //public ScreenManager ScreenManager { get; }
        //private DynamicContentManager mContentManager;
        public static ResourceHud Instance { get; private set; }
        private GraphicsDeviceManager mGraphicsDeviceManager;
        readonly private ResourceDisplay mWood;
        readonly private ResourceDisplay mStone;
        readonly private ResourceDisplay mAIWood;
        readonly private ResourceDisplay mAIStone;
        readonly private Texture2D mBackgroundTexture;
        readonly private bool mIsDebug;
        private Rectangle mRectangle;

        public ResourceHud(DynamicContentManager contentManager, GraphicsDeviceManager graphicsDeviceManager, bool isDebug)
        {
            //mContentManager = contentManager;
            Instance = this;
            mGraphicsDeviceManager = graphicsDeviceManager;
            mRectangle = new Rectangle(graphicsDeviceManager.PreferredBackBufferWidth - 150, 0, 150, 30); // Here position of the display could be set
            mBackgroundTexture = contentManager.Load<Texture2D>("Button");

            mIsDebug = isDebug;

            // Starting Values
            StoneCount = 350;
            WoodCount = 350;
            AIStoneCount = 350;
            AIWoodCount = 350;

            mWood = new ResourceDisplay(new Vector2(0, 0), 1f, "wood", WoodCount,
                contentManager);
            mStone = new ResourceDisplay(new Vector2(0, 0), 1f, "stone",StoneCount,
                contentManager);
            mAIWood = new ResourceDisplay(new Vector2(0, 0), 1f, "wood", AIWoodCount,
                contentManager);
            mAIStone = new ResourceDisplay(new Vector2(0, 0), 1f, "stone", AIStoneCount,
                contentManager);
            CalculatePositions();
        }

        private void CalculatePositions()
        {
            mWood.UpdatePosition(new Vector2((int)mRectangle.X + 2, (int)mRectangle.Y + 4));
            mStone.UpdatePosition(new Vector2((int)(mRectangle.X + mRectangle.Width / 2), (int)mRectangle.Y + 4));
            mAIWood.UpdatePosition(new Vector2((int)mRectangle.X + 2, (int)mRectangle.Y + 40));
            mAIStone.UpdatePosition(new Vector2((int)(mRectangle.X + mRectangle.Width / 2), (int)mRectangle.Y + 40));
        }

        public bool UseResources(Vector2 resources, int team)
        {
            if (team == 0)
            {
                if (WoodCount >= resources.X && StoneCount >= resources.Y)
                {
                    WoodCount -= (int)resources.X;
                    StoneCount -= (int)resources.Y;
                    return true;
                }
                return false;
            }
            else
            {
                if (AIWoodCount >= resources.X && AIStoneCount >= resources.Y)
                {
                    AIWoodCount -= (int)resources.X;
                    AIStoneCount -= (int)resources.Y;
                    return true;
                }
                return false;
            }
        }
        public bool PollResources(Vector2 resources, int team)
        {
            if (team == 0)
            {
                if (WoodCount >= resources.X && StoneCount >= resources.Y)
                {
                    return true;
                }
                return false;
            }
            else
            {

                if (AIWoodCount >= resources.X && AIStoneCount >= resources.Y)
                {
                    return true;
                }
                return false;
            }
        }

        public void ChangePosition(float x, float y)
        {
            mRectangle.X = (int)x; mRectangle.Y = (int)y; 
            CalculatePositions();
        }
        
        public void Update()
        {
            mWood.ChangeAmount(WoodCount);
            mStone.ChangeAmount(StoneCount);
            mAIWood.ChangeAmount(AIWoodCount);
            mAIStone.ChangeAmount(AIStoneCount);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mBackgroundTexture, mRectangle, Color.White);
            mWood.Draw(spriteBatch);
            mStone.Draw(spriteBatch);
            if (mIsDebug)
            {
                spriteBatch.Draw(mBackgroundTexture, new Rectangle(mRectangle.X, mRectangle.Y + 35, mRectangle.Width, mRectangle.Height), Color.White);
                mAIWood.Draw(spriteBatch);
                mAIStone.Draw(spriteBatch);
            }
        }

        public void UpdatePosition()
        {
            mRectangle = new Rectangle(mGraphicsDeviceManager.PreferredBackBufferWidth - 150, 0, 150, 30);
            CalculatePositions();
        }

        public void SaveData(ref GameData gameData)
        {
            gameData.mWoodCount = WoodCount;
            gameData.mStoneCount = StoneCount;
            gameData.mAIWoodCount = AIWoodCount;
            gameData.mAIStoneCount = AIStoneCount;
        }

        public void LoadData(GameData gameData)
        {
            WoodCount = gameData.mWoodCount;
            StoneCount = gameData.mStoneCount;
            AIStoneCount = gameData.mAIWoodCount;
            AIWoodCount = gameData.mAIStoneCount;
        }
    }
}

