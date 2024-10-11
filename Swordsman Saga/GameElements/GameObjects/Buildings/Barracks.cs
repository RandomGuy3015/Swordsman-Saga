using System;
using System.Collections.Generic;
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
using static Swordsman_Saga.GameElements.Screens.HUDs.TroopSelectionOverlay;

namespace Swordsman_Saga.GameElements.GameObjects.Buildings;

class Barracks : IBuilding
{
    public FightManager FightManager { get; set; }

    public delegate void TroopSpawnHandler(TroopType troopType, Vector2 location);
    public event TroopSpawnHandler OnTroopSpawn;
    public delegate void ResourceDeductionHandler(int cost);
    public event ResourceDeductionHandler OnResourceDeduction;

    public int Team { get; set; }
    public string Id { get; set; }
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
    public TroopSelectionOverlay TroopSelection { get; set; }
    public SoundManager Sound { get; set; }


    public Queue<TroopType> mTrainingQueue;
    private const int MaxQueueSize = 20;
    private float mTrainingTime; // time it takes to train one unit
    public float mCurrentTrainingTime; // time since the current unit started training

    Dictionary<TroopType, float> mTrainingTimes = new Dictionary<TroopType, float>() {
    { TroopType.Swordsman, 17.0f }, { TroopType.Archer, 24.0f }, { TroopType.Knight, 50.0f }, {TroopType.Worker, 12.0f}};

    public Barracks(string id, int x, int y, int player, DynamicContentManager contentManager, FightManager fightManager)
    {

        if (id == null)
        {
            ((IGameObject)this).GenerateGuid();
        }
        else
        {
            Id = id;
        }
        Position = new Vector2(x, y);
        FightManager = fightManager;
        ((IGameObject)this).InitializeRectangles(100, 100, 50, 50);
        MaxHealth = 300;
        Health = MaxHealth;
        
        CompletionTimer = 1000;
        Team = player;

        if (player == 0)
        {
            TexturePreview = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_barracks_preview");
            TexturePreviewSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_barracks_preview");

            TextureBeingBuilt = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_barracks_being_built");
            TextureBeingBuiltSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_barracks_being_built_selected");

            Texture = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_barracks");
            TextureIsSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_barracks_selected");
        }
        else
        {
            
            TexturePreview = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_barracks_preview");

            TextureBeingBuilt = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_barrack_being_build");

            TextureIsSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_barracks_selected_to_be_attacked");

            Texture = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_barracks");
            

        }
        mTrainingQueue = new Queue<TroopType>();
        mCurrentTrainingTime = 0.0f;
    }
    protected virtual void RaiseTroopSpawn(TroopType troopType, Vector2 location)
    {
        OnTroopSpawn?.Invoke(troopType, location);
    }
    public void AddToQueue(TroopType troopType)
    {
        if (BuildState != 2) { return; }

        if (mTrainingQueue.Count < MaxQueueSize)
        {
            Vector2 unitCost = TroopSelection.GetUnitCost((int)troopType);


            if (ResourceHud.UseResources(unitCost, Team))
            {
                mTrainingQueue.Enqueue(troopType);
            }
        }
    }
    public void Update(GameTime gameTime)
    {
        // update training logic
        if (mTrainingQueue.Count > 0)
        {
            mCurrentTrainingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            TroopType currentUnit = mTrainingQueue.Peek(); // look at the first unit in the queue
            if (mTrainingTimes.TryGetValue(currentUnit, out float requiredTrainingTime))
            {
                if (mCurrentTrainingTime >= requiredTrainingTime)
                {
                    mCurrentTrainingTime = 0;
                    TroopType trainedTroop = mTrainingQueue.Dequeue();
                    // use the barracks' location
                    Vector2 spawnLocation = this.Position;
                    RaiseTroopSpawn(trainedTroop, spawnLocation);
                }
            }
        }

    }
    public float GetRemainingTrainingTime()
    {
        if (mTrainingQueue.Count > 0)
        {
            TroopType currentUnit = mTrainingQueue.Peek(); // look at the first unit in the queue
            if (mTrainingTimes.TryGetValue(currentUnit, out float requiredTrainingTime))
            {
                return requiredTrainingTime - mCurrentTrainingTime;
            }
        }
        return 0; // return 0 if there is no unit in training
    }
    public IEnumerable<TroopType> GetTrainingQueue()
    {
        return mTrainingQueue;
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

        if (data.mTrainingQueues.ContainsKey(Id))
        {
            data.mTrainingQueues.Remove(Id);
        }

        if (data.mCurrentTrainingTimes.ContainsKey(Id))
        {
            data.mCurrentTrainingTimes.Remove(Id);
        }

        data.mCurrentTrainingTimes.Add(Id, mCurrentTrainingTime);
        data.mTrainingQueues.Add(Id, mTrainingQueue);
        data.mBuildStates.Add(Id, BuildState);
        data.mCompletionTimers.Add(Id, CompletionTimer);
        data.mUnitPlayer.Add(Id, Team);
        data.mGameObjects.Add(Id, this.GetType().Name);
        data.mHealth.Add(Id, Health);
        data.mObjectPosition.Add(Id, Position);

    }
}