using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.InputManagement;
using Swordsman_Saga.GameElements.Screens.HUDs;

namespace Swordsman_Saga.Engine.DataPersistence;

public class StatisticsManager
{
    private StatisticsData mStatisticsData;
    private FileDataHandler mFileDataHandler;
    private string mFileName = "data.statistics";

    private float mElapsedGameTime = 0;


    
    // Statistic IDs
    private enum StatisticId
    {
        S01,
        S02,
        S03,
        S04,
        S05,
        S06
    };
    
    // Statistics Values
    private Dictionary<StatisticId, int> mStatisticsValues;

    // Statistic Descriptions
    private static readonly Dictionary<StatisticId, string> sStatisticsDescriptions =
        new Dictionary<StatisticId, string>
        {
            { StatisticId.S01, "Total Playing Time (Seconds)" },
            { StatisticId.S02, "Number of Lost Units" },
            { StatisticId.S03, "Number of Trained Units" },
            { StatisticId.S04, "Number of Defeated Enemy Units" },
            { StatisticId.S05, "Number of Collected Wood" },
            { StatisticId.S06, "Number of Collected Stone" },
        };
    public StatisticsManager()
    {
        mStatisticsValues = new Dictionary<StatisticId, int>
        {
            { StatisticId.S01, 0 },
            { StatisticId.S02, 0 },
            { StatisticId.S03, 0 },
            { StatisticId.S04, 0 },
            { StatisticId.S05, 0 },
            { StatisticId.S06, 0 },
        };
    }

    public void Load(StatisticsData statData)
    {
        mStatisticsValues[StatisticId.S01] = statData.mS1;
        mStatisticsValues[StatisticId.S02] = statData.mS2;
        mStatisticsValues[StatisticId.S03] = statData.mS3;
        mStatisticsValues[StatisticId.S04] = statData.mS4;
        mStatisticsValues[StatisticId.S05] = statData.mS5;
        mStatisticsValues[StatisticId.S06] = statData.mS6;
    }
    public void Save(ref StatisticsData statData)
    {
        statData.mS1 = mStatisticsValues[StatisticId.S01];
        statData.mS2 = mStatisticsValues[StatisticId.S02];
        statData.mS3 = mStatisticsValues[StatisticId.S03];
        statData.mS4 = mStatisticsValues[StatisticId.S04];
        statData.mS5 = mStatisticsValues[StatisticId.S05];
        statData.mS6 = mStatisticsValues[StatisticId.S06];
    }
    public Dictionary<string, int> GetStatistics()
    {
        Dictionary<string, int> statistics = new Dictionary<string, int>();
        foreach (StatisticId statisticId in Enum.GetValues(typeof(StatisticId)))
        {
            statistics.Add(sStatisticsDescriptions[statisticId], mStatisticsValues[statisticId]);
        }
        return statistics;
    }
    
    public void UpdateGameTime(GameTime gameTime)
    {
        // Update Total Playing Time
        mElapsedGameTime += gameTime.ElapsedGameTime.Milliseconds;
        if (mElapsedGameTime > 1000)
        {
            mElapsedGameTime -= 1000;
            mStatisticsValues[StatisticId.S01] += 1;
        }
    }
    
    public void UnitLost()
    {
        mStatisticsValues[StatisticId.S02] += 1;
    }
    public void UnitTrained()
    {
        mStatisticsValues[StatisticId.S03] += 1;
    }
    public void EnemyUnitDefeated()
    {
        mStatisticsValues[StatisticId.S04] += 1;
    }

    public void WoodCollected(int amount)
    {
        mStatisticsValues[StatisticId.S05] += amount;
    }
    public void StoneCollected(int amount)
    {
        mStatisticsValues[StatisticId.S06] += amount;
    }
}