using System;
using System.Collections.Generic;
using System.Runtime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace hausaufgabe;

public class Game1 : Game
{
    private List<BasicObject> _objects;
    public Game1()
    {
        Global.graphics = new GraphicsDeviceManager(this);
        Global.content = this.Content;
        Global.mouse = new MyMouse();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _objects = new List<BasicObject>();
        _objects.Add(new Background(
            Global.content.Load<Texture2D>("Background"),
            new Vector2(Global.graphics.PreferredBackBufferWidth/2, 
                Global.graphics.PreferredBackBufferHeight/2)));
        _objects.Add(new Icon(
            Global.content.Load<Texture2D>("unilogo"),
            new Vector2(Global.graphics.PreferredBackBufferWidth/2, 
                Global.graphics.PreferredBackBufferHeight/2)));
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Global.spriteBatch = new SpriteBatch(GraphicsDevice);
        // TODO: use this.Content to load your game content here
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        Global.mouse.Update();
        foreach (BasicObject basicObject in _objects)
        {
            basicObject.Update();
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        Global.spriteBatch.Begin();
        foreach (BasicObject basicObject in _objects)
        {
            basicObject.Draw();
        }
        Global.spriteBatch.End();
        base.Draw(gameTime);
    }
}
