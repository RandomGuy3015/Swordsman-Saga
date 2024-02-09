using Microsoft.Xna.Framework;
using Swordsman_Saga.Engine.DataPersistence;
using Swordsman_Saga.Engine.SoundManagement;

namespace Swordsman_Saga.Engine.ObjectManagement;
interface IGameObject : IDrawableObject, IDataPersistence
{
    string Id { get; internal set; }
    Vector2 Position { get; set; }
    Rectangle TextureRectangle { get; set; }
    Vector2 TextureOffset { get; set; }
    Rectangle HitboxRectangle { get; set; }
    Vector2 HitboxOffset { get; set; }

    // Moved it to gameObject as it's stupid to have both IsAllied and Player as separate variables, shit like arrow wouldn't be able to have a team
    // 0 == Player, 1 == Enemy, -1 == MapObject
    int Team { get; set; }
    SoundManager Sound { get; internal set; }


    string TypeToString()
    {
        return this.GetType().Name;
    }

    void GenerateGuid()
    {
        Id = System.Guid.NewGuid().ToString();
    }
    void SetSoundManager(SoundManager soundManager)
    {
        this.Sound = soundManager;
    }

    void InitializeRectangles(int textureRectangleWidth, int textureRectangleHeight, int hitboxRectangleWidth,
        int hitboxRectangleHeight)
    {
        // calculate Offsets
        TextureOffset = new Vector2(textureRectangleWidth / 2, textureRectangleHeight / 2);
        HitboxOffset = new Vector2(hitboxRectangleWidth / 2, hitboxRectangleHeight / 4);
        // init Rectangles
        TextureRectangle = new Rectangle((int)Position.X - (int)TextureOffset.X, (int)Position.Y - (int)TextureOffset.Y, textureRectangleWidth, textureRectangleHeight);
        HitboxRectangle = new Rectangle((int)Position.X -(int)HitboxOffset.X, (int)Position.Y - (int)HitboxOffset.Y, hitboxRectangleWidth, hitboxRectangleHeight);
    }
}