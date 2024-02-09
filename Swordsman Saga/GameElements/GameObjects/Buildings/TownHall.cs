using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataTypes;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.FightManagement;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.SoundManagement;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.GameElements.Screens.HUDs;

namespace Swordsman_Saga.GameElements.GameObjects.Buildings;

class TownHall : IBuilding
{
    public int Team { get; set; }
    public string Id { get; set; }
    public FightManager FightManager { get; set; }
    public Texture2D Texture { get; set; }
    public Texture2D TextureIsSelected { get; set; }
    public Texture2D TexturePreview { get; set; }
    public Texture2D TexturePreviewSelected { get; set; }
    public Texture2D TextureBeingBuilt { get; set; }
    public Texture2D TextureBeingBuiltSelected { get; set; }

    public ResourceHud ResourceHud { get; set; }

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
    public SoundManager Sound { get; set; }

    public List<Worker> mWorkers;
    private int mMinWorker;

    public int WorkersMissing { get; set; }

    public TownHall(string id, int x, int y, int player, DynamicContentManager contentManager, FightManager fightManager)
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
        Position = new Vector2(x, y);
        ((IGameObject)this).InitializeRectangles(100, 100, 50, 50);
        // nur irgendwelche Beispielwerte
        MaxHealth = 700;
        Health = MaxHealth;
        Team = player;
        BuildState = 2;
        mMinWorker = 1;
        mWorkers = new List<Worker>();
        if (player == 0)
        {
            TexturePreview = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_townhall");
            TexturePreviewSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_townhall_selected");

            TextureBeingBuilt = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_townhall");
            TextureBeingBuiltSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_townhall_selected");

            Texture = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_townhall");
            TextureIsSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/friendly_townhall_selected");
        }
        else
        {

            Texture = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_townhall");
            TextureIsSelected = contentManager.Load<Texture2D>("2DAssets/Buildings/enemy_townhall_selected_to_be_attacked");
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (var worker in mWorkers.ToList())
        {
            if (worker.IsDead)
            {
                mWorkers.Remove(worker);
            }
        }

        if (mWorkers.Count < mMinWorker)
        {
            WorkersMissing = mMinWorker - mWorkers.Count;
        }
    }
}