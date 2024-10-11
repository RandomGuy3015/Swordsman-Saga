using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.Screens.HUDs;

namespace Swordsman_Saga.GameElements.GameObjects.Buildings;

class LumberCamp : IResourceBuilding
{
    public int Team { get; set; }
    public string Id { get; set; }
    public FightManager FightManager { get; set; }
    public StatisticsManager StatisticsManager { get; set; }
    public Texture2D Texture { get; set; }
    public Texture2D TextureIsSelected { get; set; }
    public Texture2D TexturePreview { get; set; }
    public Texture2D TexturePreviewSelected { get; set; }
    public Texture2D TextureBeingBuilt { get; set; }
    public Texture2D TextureBeingBuiltSelected { get; set; }

    public Rectangle TextureRectangle { get; set; }
    public Vector2 TextureOffset { get; set; }
    public Rectangle HitboxRectangle { get; set; }
    public Vector2 HitboxOffset { get; set; }
    public Vector2 MoveOffsetInGroup { get; set; }
    public bool IsSelected { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int CompletionTimer { get; set; }
    public int BuildState { get; set; }

    public ResourceHud ResourceHud { get; set; }
    public int WoodGeneration { get; set; }
    public int Level { get; set; }
    public Vector2 UpgradeCost { get; set; }

    private double mResourcetime = 0;
    private bool mUpgrading = false;

    public bool UpgradeBuilding()
    {
        if (ResourceHud.WoodCount >= UpgradeCost.X && ResourceHud.StoneCount >= UpgradeCost.Y)
        {
            mUpgrading = true;
            ResourceHud.UseResources(UpgradeCost, Team);
            mResourcetime = 0;
            return true;
        }
        return false;
    }
    public SoundManager Sound { get; set; }


    public LumberCamp(string id, int x, int y, int player, DynamicContentManager contentManager, FightManager fightManager, StatisticsManager statisticsManager)
    {
        if (id == null)
        {
            ((IGameObject)this).GenerateGuid();
        }
        else
        {
            Id = id;
        }
        FightManager = fightManager;
        StatisticsManager = statisticsManager;
        Position = new Vector2(x, y);
        ((IGameObject)this).InitializeRectangles(100, 100, 50, 50);
        // nur irgendwelche Beispielwerte
        MaxHealth = 250;
        Health = MaxHealth;
        Team = player;
        WoodGeneration = 5;
        Level = 1;
        UpgradeCost = new Vector2(80, 120);
        CompletionTimer = 600;

        if (player == 0)
        {
            TexturePreview = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_lumbercamp_preview");
            TexturePreviewSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_lumbercamp_preview");

            TextureBeingBuilt = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_lumbercamp_being_built");
            TextureBeingBuiltSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_lumbercamp_being_built_selected");

            Texture = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_lumbercamp");
            TextureIsSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_lumbercamp_selected");
        }
        else
        {
            TexturePreview = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_lumber_camp_preview");

            TextureBeingBuilt = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_lumber_camp_being_build");

            TextureIsSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_lumber_camp_selected_to_be_attacked");

            Texture = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_lumber_camp");
        }
    }
    public void Update(GameTime gameTime)
    {
        mResourcetime += gameTime.ElapsedGameTime.TotalSeconds;
        if (mResourcetime >= 1 && mUpgrading == false)
        {
            if (BuildState == 2 && ResourceHud != null)
            {
                if (Team == 0)
                {
                    ResourceHud.WoodCount += WoodGeneration;
                    StatisticsManager.WoodCollected(WoodGeneration);
                }
                else
                {
                    ResourceHud.AIWoodCount += WoodGeneration;
                }
            }
            mResourcetime = 0;
        }

        if (mResourcetime >= 10 && mUpgrading)
        {
            WoodGeneration = (int)((float)WoodGeneration * 1.2f) + 5;
            UpgradeCost += new Vector2(80, 120);
            Level += 1;
            mUpgrading = false;
            mResourcetime = 0;
        }
    }

    public bool IsUpgrading()
    {
        return mUpgrading;
    }

    void IDataPersistence.SaveData(ref GameData data)
    {
        if (data.mHealth.ContainsKey(Id))
        {
            data.mHealth.Remove(Id);
        }
        if (data.mObjectPosition.ContainsKey(Id))
        {
            data.mObjectPosition.Remove(Id);
        }

        if (data.mGameObjects.ContainsKey(Id))
        {
            data.mGameObjects.Remove(Id);
        }
        if (data.mUnitPlayer.ContainsKey(Id))
        {
            data.mUnitPlayer.Remove(Id);
        }

        if (data.mCompletionTimers.ContainsKey(Id))
        {
            data.mCompletionTimers.Remove(Id);
        }

        if (data.mBuildStates.ContainsKey(Id))
        {
            data.mBuildStates.Remove(Id);
        }

        data.mBuildStates.Add(Id, BuildState);
        data.mCompletionTimers.Add(Id, CompletionTimer);
        data.mUnitPlayer.Add(Id, Team);
        data.mGameObjects.Add(Id, this.GetType().Name);
        data.mHealth.Add(Id, Health);
        data.mObjectPosition.Add(Id, Position);
        if (data.mWoodGeneration.ContainsKey(Id))
        {
            data.mWoodGeneration.Remove(Id);
        }
        if (data.mUpgradeCost.ContainsKey(Id))
        {
            data.mUpgradeCost.Remove(Id);
        }
        if (data.mLevel.ContainsKey(Id))
        {
            data.mLevel.Remove(Id);
        }

        data.mLevel.Add(Id, Level);
        data.mUpgradeCost.Add(Id, UpgradeCost);
        data.mWoodGeneration.Add(Id, WoodGeneration);
    }
}
