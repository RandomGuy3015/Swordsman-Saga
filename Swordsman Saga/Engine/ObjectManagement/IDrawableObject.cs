using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataTypes;

namespace Swordsman_Saga.Engine.ObjectManagement;

interface IDrawableObject
{
    Texture2D Texture { get; }
    void Draw(SpriteBatch spriteBatch, DiamondGrid grid, bool showHitbox, bool showTextureRectangle);
}