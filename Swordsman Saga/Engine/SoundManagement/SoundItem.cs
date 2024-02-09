using Microsoft.Xna.Framework.Audio;
using Swordsman_Saga.Engine.DynamicContentManagement;

namespace Swordsman_Saga.Engine.SoundManagement;

public class SoundItem
{
    public float mStandardVolume;
    public string mName;
    public SoundEffect mSound;
    public SoundEffectInstance mInstance;
    
    public SoundItem(string soundFilePath, float volume, DynamicContentManager contentManager)
    {
        mName = soundFilePath;
        mStandardVolume = volume;
        mSound = contentManager.Load<SoundEffect>(soundFilePath);
        CreateInstance();
    }

    public virtual void CreateInstance()
    {
        mInstance = mSound.CreateInstance();
    }
}