using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine.SettingsManagement;

public interface IKeybindField
{
    void Draw(SpriteBatch spriteBatch);
    
    void Update(InputState inputState);
    
    public void ChangePosition(float x, float y)
    {
    }
}