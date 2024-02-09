using System.Drawing;
using System.Numerics;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine.ScreenManagement.MenuManagement;

public abstract class MenuElement
{
    public Vector2 mPosition;
    public Rectangle mSize;
    public bool mVisible;
    public void Interact(InputState inputState) {}
}