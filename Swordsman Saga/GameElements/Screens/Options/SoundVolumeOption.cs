using System.Collections.Generic;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.Engine.SettingsManagement;
using Swordsman_Saga.Engine.SoundManagement;

namespace Swordsman_Saga.GameElements.Screens.Options;

public class SoundVolumeOption : IOption
{
    private SoundManager mSoundManager;
    public int Value { get; set; }
    public int MinValue { get; set; } = 0;
    public int MaxValue { get; set; } = 100;
    public bool IsEditable { get; set; }
    public List<ActionType> AllowedInputActions { get; set; } = new()
    {
        ActionType.PressKey0,
        ActionType.PressKey1,
        ActionType.PressKey2,
        ActionType.PressKey3,
        ActionType.PressKey4,
        ActionType.PressKey5,
        ActionType.PressKey6,
        ActionType.PressKey7,
        ActionType.PressKey8,
        ActionType.PressKey9,
        ActionType.PressBackSpaceKey,
    };

    public string Name { get; set; } = "SoundVolume";
    
    public SoundVolumeOption(SoundManager soundManager, int value)
    {
        mSoundManager = soundManager;
        Value = value;
    }
    
    public void Initialize(string name, int value, int minValue, int maxValue, List<ActionType> allowedInputActions)
    {
        Name = name;
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
        AllowedInputActions = allowedInputActions;
    }
    
    public void StartEditing()
    {
        IsEditable = true;
    }
    
    public void StopEditing()
    {
        IsEditable = false;
    }
    public void SetValue(int value)
    {
        Value = value;
    }
    
    public void SaveOption()
    {
        //TODO save option to file.
        //TODO save option at corresponding point in game.
        mSoundManager.SetMasterSoundVolume(Value);
    }
}