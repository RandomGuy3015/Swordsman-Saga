using System.Collections.Generic;

namespace Swordsman_Saga.Engine.DataPersistence.Data;
[System.Serializable]
public class StatisticsData
{
    public int mS1;
    public int mS2;
    public int mS3;
    public int mS4;
    public int mS5;
    public int mS6;

    public StatisticsData()
    {
        mS1 = 0;
        mS2 = 0;
        mS3 = 0;
        mS4 = 0;
        mS5 = 0;
        mS6 = 0;
    }
}