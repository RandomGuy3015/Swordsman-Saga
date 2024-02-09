using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace hausaufgabe;

public class MyMouse
{
    public MouseState mouse;
    public Vector2 position;
    
    public MyMouse()
    {
        mouse = new MouseState();
    }

    public void Update()
    {
        mouse = Mouse.GetState();
        position = new Vector2(mouse.X, mouse.Y);
    }
}