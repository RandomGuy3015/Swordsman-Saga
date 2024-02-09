using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace hausaufgabe;

public class Background : BasicObject
{
    public Background(Texture2D texture, Vector2 position) : base(texture, position)
    {
        _scale = new Vector2((float)Global.graphics.PreferredBackBufferWidth / (float)_texture.Width,
            (float)Global.graphics.PreferredBackBufferHeight / (float)_texture.Height);
    }

    public override void Update()
    {
        //
    }
}