using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.DynamicContentManagement;
using Swordsman_Saga.Engine.SettingsManagement;

namespace Swordsman_Saga.Engine.SoundManagement
{
    public class SoundManager
    {
        private List<SoundEffectInstanceWrapper> mPlayingSoundInstances = new List<SoundEffectInstanceWrapper>();
        private SongItem mBackgroundMusic;
        private DynamicContentManager mContentManager;
        private SettingsManager mSettingsManager;
        private int mGlobalSoundVolumePercent = 50;
        private bool mIsMuted = false;
        private int mVolumeInPercent = 100;

        private class SoundEffectInstanceWrapper
        {
            public SoundEffectInstance mSoundEffectInstance;
            public string mSoundFilePath;
            
            public SoundEffectInstanceWrapper(SoundEffectInstance soundEffectInstance, string soundFilePath)
            {
                mSoundEffectInstance = soundEffectInstance;
                mSoundFilePath = soundFilePath;
            }
        }

        public bool IsMuted
            {
                get { return mIsMuted; }
            }

        public SoundManager(DynamicContentManager contentManager, SettingsManager settingsManager)
        {
            mContentManager = contentManager;
            this.mSettingsManager = settingsManager;
        }

        public void Load(SoundData data)
        {
            SetBackgroundMusicVolume(data.mMusicVolume);
            SetMasterSoundVolume(data.mSoundVolume);
        }

        public void Save(ref SoundData data)
        {
            data.mMusicVolume = GetBackgroundMusicVolume();
            data.mSoundVolume = GetMasterSoundVolume();
        }
        public void SetBackgroundMusicVolume(int percent)
        {
            if (mBackgroundMusic?.mSong == null)
            {
                return;
            }
            mBackgroundMusic.mVolume = ((float) percent / 100);
            MediaPlayer.Volume = mBackgroundMusic.mVolume;
        }
        
        public int GetBackgroundMusicVolume()
        {
            return (int) (mBackgroundMusic.mVolume * 100);
        }

        public void SetMasterSoundVolume(int percent)
        {
            mGlobalSoundVolumePercent = percent;
        }
        public int GetMasterSoundVolume()
        {
            return mGlobalSoundVolumePercent;
        }
        

        public void PlayBackgroundMusic(string backgroundMusicFilePath, float volumePercent)
        {
            mBackgroundMusic = new SongItem(backgroundMusicFilePath, volumePercent, mContentManager);
            MediaPlayer.Play(mBackgroundMusic.mSong);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = mBackgroundMusic.mVolume;
        }

        public SoundEffectInstance PlaySound(string soundFilePath, float volumePercent, bool looped, bool forceCreateNewInstance = true)
        {
            // Remove stopped instances
            mPlayingSoundInstances.RemoveAll(instance => instance.mSoundEffectInstance.State == SoundState.Stopped);

            // Check if the maximum number of simultaneous sound instances is reached
            
            // if createNewInstance is false, then stop the oldest sound instance of that sound and play the sound.
            if (!forceCreateNewInstance)
            {
                // if instance of that sound is already playing, return null.
                if (mPlayingSoundInstances.Find(instance => instance.mSoundFilePath == soundFilePath) != null)
                {
                    return null;
                }
                var item = new SoundItem(soundFilePath, volumePercent, mContentManager);
                SoundEffectInstance sound = RunSound(item.mSound, item.mSound.CreateInstance(), item.mStandardVolume,
                            volumePercent, looped);
                mPlayingSoundInstances.Add(new SoundEffectInstanceWrapper(sound, soundFilePath));
                return sound;
                
            }
            return null;
        }

        public void StopAllSounds()
        {
            foreach (var soundInstance in mPlayingSoundInstances)
            {
                soundInstance.mSoundEffectInstance.Stop();
            }
            mPlayingSoundInstances = new List<SoundEffectInstanceWrapper>();
        }
        
        public void StopSound(SoundEffectInstance soundInstance)
        {
            soundInstance.Stop();
            mPlayingSoundInstances.Remove(mPlayingSoundInstances.Find(instance => instance.mSoundEffectInstance == soundInstance));
        }
        
        public void PauseBackgroundMusic()
        {
            MediaPlayer.Pause();
        }
        
        public void ResumeBackgroundMusic()
        {
            MediaPlayer.Resume();
        }
        
        public void StopBackgroundMusic()
        {
            MediaPlayer.Stop();
        }
        
        public void PauseSound(SoundEffectInstance soundInstance)
        {
            soundInstance.Pause();
        }
        
        public void ResumeSound(SoundEffectInstance soundInstance)
        {
            soundInstance.Resume();
        } protected virtual SoundEffectInstance RunSound(
            SoundEffect sound, SoundEffectInstance instance, float standardVolumePercent, float volumePercent, bool looped)
        {
            // Set volume parameter of instance according to global settings and volume parameter passed to RunSound.
            instance.Volume = volumePercent * standardVolumePercent * mGlobalSoundVolumePercent / 100;
            instance.IsLooped = looped;
            instance.Play();
            return instance;
        }
        public void AdjustAllVolume(float delta)
        {
            mVolumeInPercent = MathHelper.Clamp(mVolumeInPercent + (int)(delta * 100), 0, 100);
            // Adjust the background music volume
            SetBackgroundMusicVolume(mVolumeInPercent);
            // Adjust the master volume
            SetMasterSoundVolume(mVolumeInPercent);
        }
        
        public void ToggleMute()
        {
            mIsMuted = !mIsMuted;

            // Mute or unmute background music
            if (mBackgroundMusic?.mSong != null)
            {
                if (mIsMuted)
                {
                    MediaPlayer.Volume = 0;
                }
                else
                {
                    MediaPlayer.Volume = mBackgroundMusic.mVolume;
                }
            }

            // Mute or unmute all playing sound instances
            foreach (var soundInstance in mPlayingSoundInstances)
            {
                if (soundInstance != null)
                {
                    soundInstance.mSoundEffectInstance.Volume = mIsMuted ? 0 : soundInstance.mSoundEffectInstance.Volume;
                }
            }
        }
    }
}