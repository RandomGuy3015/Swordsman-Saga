using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Swordsman_Saga.Engine.DynamicContentManagement;

namespace Swordsman_Saga.Engine.SoundManagement;

public class SongItem
{
    public float mVolume;
    public float mStandardVolume;
    public string mName;
    public Song mSong;

    public SongItem(string soundFilePath, float volume, DynamicContentManager contentManager)
    {
        mName = soundFilePath;
        mVolume = volume;
        mStandardVolume = volume;
        mSong = contentManager.Load<Song>(soundFilePath);
    }
}