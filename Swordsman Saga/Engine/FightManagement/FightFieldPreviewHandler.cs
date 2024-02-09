using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.DataTypes.Grids;

namespace Swordsman_Saga.GameLogic;

class FightFieldPreviewHandler
{
        public Diamond SnappedToGridDiamond { get; private set; }
        
        public void Draw(SpriteBatch spriteBatch, Color color ) {
            if (true) //check if field is hovered over by the mouse and Fight-mode is active (F button has been pressed,
            // and mouse has not been clicked yet)
            {
                // SnappedToGridDiamond.DrawFieldOutline(spriteBatch, color);
            }
        }
}

