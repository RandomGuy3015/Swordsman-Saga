using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace hausaufgabe;

public class BasicObject
{
    protected Texture2D _texture;
    protected Vector2 _position;
    protected Vector2 _origin;
    protected Vector2 _scale;

    public BasicObject(Texture2D texture, Vector2 position)
    {
        _texture = texture;
        _position = position;
        _origin = new Vector2(texture.Width / 2, texture.Height / 2);
        _scale = new Vector2(1, 1);
    }

    public virtual void Update()
    {
        
    }
    public virtual void Draw()
    {
        Global.spriteBatch.Draw(_texture, _position, null, Color.White, 0, _origin, _scale, SpriteEffects.None, 0);
    }
}