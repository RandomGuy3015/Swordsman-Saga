using System;
using System.Collections.Generic;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.GameElements.GameObjects.Units;

namespace Swordsman_Saga.Engine.DataPersistence.Data;

[System.Serializable]
class AchievementsData
{
    public Dictionary<int, bool> mCompletion;
    public Dictionary<int, float> mProgress;


    public AchievementsData()
    {
        mCompletion = new Dictionary<int, bool>()
        {
            {0, false},
            {1, false}, {2, false}, {3, false}, {4, false}, {5, false}, {6, false}, {7, false}, {8, false}, {9, false}, {10, false}, {11, false}, {12, false}, {13, false}, {14, false}, {15, false}
        };
        mProgress = new Dictionary<int, float>()
        {
            {0, 0}, {1, 0}, {2, 0}, {3, 0}, {4, 0},
            {5, 0}, {6, 0}, {7, 0}, {8, 0}, {9, 0},
            {10, 0}, {11, 0}, {12, 0}, {13, 0}, {14, 0}, {15, 0}
        };
    }
}