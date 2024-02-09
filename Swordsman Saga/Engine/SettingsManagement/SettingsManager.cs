using System.Collections.Generic;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine.SettingsManagement
{
    public class SettingsManager
    {
        // field for saving the Keymappings.
        private Dictionary<ActionType, List<KeyEvent>> mKeyBindings;
        
        // Default KeyBindings
        private Dictionary<ActionType, List<KeyEvent>> mDefaultKeyBindings = new Dictionary<ActionType, List<KeyEvent>>();

        // Global sound volume. To be set in settings menu.
        private float mSoundVolume = 100;

        public SettingsManager()
        {
        }

        public void SetKeyBindings(Dictionary<ActionType, List<KeyEvent>> keyMapping)
        {
            mKeyBindings = keyMapping;
        }

        public Dictionary<ActionType, List<KeyEvent>> GetKeyBindings()
        {
            return mKeyBindings;
        }

        public void SetSoundVolume(float soundVolume)
        {
            mSoundVolume = soundVolume;
        }
        
        public float GetSoundVolume()
        {
            return mSoundVolume;
        }
        
        public void SetDefaultKeyBindings(Dictionary<ActionType, List<KeyEvent>> defaultKeyBindings)
        {
            mDefaultKeyBindings = defaultKeyBindings;
        }
    }
}