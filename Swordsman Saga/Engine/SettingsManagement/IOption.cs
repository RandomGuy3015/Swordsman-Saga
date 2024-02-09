using System;
using System.Collections.Generic;
using Swordsman_Saga.Engine.InputManagement;

namespace Swordsman_Saga.Engine.SettingsManagement;

public interface IOption
{
    string Name { get; set; }
    int Value { get;  set; }
    int MinValue { get; set; }
    int MaxValue { get; set; }

    // following Actiontypes (Keys) are allowed to be used as input for this option.
    public List<ActionType> AllowedInputActions { get; set; }

    void Initialize(string name, int value, int minValue, int maxValue, List<ActionType> allowedInputActions)
    {
        Name = name;
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
        AllowedInputActions = allowedInputActions;
    }
    
    public int GetValue()
    {
        return Value;
    }
    
    public void SetValue(int value)
    {
        Value = value;
    }
    
    public int GetMinValue()
    {
        return MinValue;
    }
    
    public int GetMaxValue()
    {
        return MaxValue;
    }
    public bool AllowedInputAction(ActionType actionType)
    {
        return AllowedInputActions.Contains(actionType);
    }
    
    public void SaveOption()
    {
        //TODO save option to file.
        //TODO save option at corresponding point in game.
    }
}