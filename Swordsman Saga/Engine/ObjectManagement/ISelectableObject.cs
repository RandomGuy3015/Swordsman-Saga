using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Swordsman_Saga.Engine.ObjectManagement
{
    interface ISelectableObject : IGameObject
    {
        Texture2D TextureIsSelected { get; }
        public Vector2 Size { get; set; }
        public bool IsSelected { get; set; }
        
        public bool ContainsPoint(Vector2 point)
        {
            return (point.X >= Position.X && point.X <= Position.X + Size.X) &&
                   (point.Y >= Position.Y && point.Y <= Position.Y + Size.Y);
        }
    }
}