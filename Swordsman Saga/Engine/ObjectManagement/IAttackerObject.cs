using Microsoft.Xna.Framework.Graphics;

namespace Swordsman_Saga.Engine.ObjectManagement;
public interface IAttackerObject
{
    Texture2D TextureIsAttacking { get; set; }
    //bool IsStriking{ get; set; }
    //bool IsAttacking { get; set; }
}
