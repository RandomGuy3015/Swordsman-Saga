using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

namespace Swordsman_Saga.Engine.DataTypes.Grids
{
    public class Diamond
    {
        // centered position around X, Y
        public int X {  get; set; }
        public int Y { get; set; }
        public int Size { get; private set; }

        public Diamond(int x, int y, int size)
        {
            X = x; Y = y; Size = size;
        }
        public Diamond(Vector2 pos, int size)
        {
            X = (int) pos.X;
            Y = (int) pos.Y;
            Size = size;
        }

        public bool Contains(Vector2 p)
        {
            int dx = Math.Abs((int) p.X - X);
            int dy = Math.Abs((int) p.Y - Y);

            return ((int) dx + 2 * (int) dy) <= Size;
        }

        public void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.Color color)
        {
            
            spriteBatch.DrawLine(X - Size, Y, X, Y - Size / 2, color);
            spriteBatch.DrawLine(X, Y - Size / 2, X + Size, Y, color);
            spriteBatch.DrawLine(X - Size, Y, X, Y + Size / 2, color);
            spriteBatch.DrawLine(X, Y + Size / 2, X + Size, Y, color);
            
            /* If you want to draw from the corner not the center
            spriteBatch.DrawLine(X, Y, X + Size, Y - Size / 2, color);
            spriteBatch.DrawLine(X + Size, Y - Size / 2, X + 2 * Size, Y, color);
            spriteBatch.DrawLine(X, Y, X + Size, Y + Size / 2, color);
            spriteBatch.DrawLine(X + Size, Y + Size / 2, X + 2 * Size, Y, color);
            */
        }

    }
}
