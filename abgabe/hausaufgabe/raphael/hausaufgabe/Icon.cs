using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace hausaufgabe;

public class Icon : BasicObject
{
    // gibt definitiv nen besseren Ort das unterzubringen, aber muss mal fertig werdem -> pProjekt fpr ein ander mal
    private SoundEffect soundHit;
    private SoundEffect soundMiss;
    public Icon(Texture2D texture, Vector2 position) : base(texture, position)
    {
        float scale = Math.Min((float)Global.graphics.PreferredBackBufferWidth / (float)_texture.Width * 0.4f,
            (float)Global.graphics.PreferredBackBufferHeight / (float)_texture.Height * 0.4f);
        /*_scale = new Vector2((float)Global.graphics.PreferredBackBufferWidth / (float)_texture.Width * 0.2f,
            (float)Global.graphics.PreferredBackBufferHeight / (float)_texture.Height * 0.2f);
        */
        _scale = new Vector2(scale, scale);
        soundHit = Global.content.Load<SoundEffect>("Logo_hit");
        soundMiss = Global.content.Load<SoundEffect>("Logo_miss");
    }

    public override void Update()
    {
        if (MouseOnIcon())
        {
            // play sound 1
            soundHit.Play();
        }
        else
        {
            // play sound 2
            soundMiss.Play();
        }
    }

    public bool MouseOnIcon()
    {
 
        if (Global.mouse.position.X > _position.X - (float)_texture.Width * _scale.X / 2
            && Global.mouse.position.X < _position.X + (float)_texture.Width * _scale.X / 2
            && Global.mouse.position.Y > _position.Y - (float)_texture.Height * _scale.Y / 2
            && Global.mouse.position.Y < _position.Y + (float)_texture.Height * _scale.Y / 2)
        {
            return true;
        }

        return false;
    
    }
}
