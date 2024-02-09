using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Swordsman_Saga.Engine.DataPersistence.Data;
using Swordsman_Saga.Engine.ObjectManagement;
using Swordsman_Saga.Engine.ScreenManagement;
using Swordsman_Saga.GameElements.GameObjects.Buildings;
using Swordsman_Saga.GameElements.GameObjects.Units;
using Swordsman_Saga.GameElements.Screens.HUDs;
using Swordsman_Saga.GameElements.Screens.Menus.Save_Slot_Menu;

namespace Swordsman_Saga.Engine.DataPersistence;

class AchievementsManager
{
    private int highestStoneCount = 0;
    private int highestWoodCount = 0;
    private int highestBuildingCount = 0;
    private int highestLumberCount = 0;
    private int highestQuarryCount = 0;
    private int highestBarracksCount = 0;
    private AchievementsData mAchievementsData;
    private FileDataHandler mFileDataHandler;
    private string mFileName = "data.achievements";

    private Dictionary<AchievementId, Achievement> mAchievements;
    
   // Achievement IDs
    private enum AchievementId
    {
        Ac01A,
        Ac01B,
        Ac01C,
        Ac02A,
        Ac02B,
        Ac02C,
        Ac03A,
        Ac03B,
        Ac03C,
        Ac04,
        Ac05,
        Ac06,
        Ac07,
        Ac08,
        Ac09,
        Ac10
    }

    public AchievementsManager()
    {
        mAchievements = new Dictionary<AchievementId, Achievement>()
        {
            { AchievementId.Ac01A, new Achievement((int)AchievementId.Ac01A, "Cobbler", "Besitze 1000 Stone") },
            { AchievementId.Ac01B, new Achievement((int)AchievementId.Ac01B, "Mason", "Besitze 5000 Stone") },
            { AchievementId.Ac01C, new Achievement((int)AchievementId.Ac01C, "Master Builder", "Besitze 20000 Stone") },
            { AchievementId.Ac02A, new Achievement((int)AchievementId.Ac02A, "Lumberjack", "Besitze 1000 Wood") },
            { AchievementId.Ac02B, new Achievement((int)AchievementId.Ac02B, "Forest Master", "Besitze 5000 Wood") },
            { AchievementId.Ac02C, new Achievement((int)AchievementId.Ac02C, "Timber Trader", "Besitze 20000 Wood") },
            { AchievementId.Ac03A, new Achievement((int)AchievementId.Ac03A, "Hamlet", "Besitze 10 Gebaeude gleichzeitig") },
            { AchievementId.Ac03B, new Achievement((int)AchievementId.Ac03B, "Village", "Besitze 50 Gebaeude gleichzeitig") },
            { AchievementId.Ac03C, new Achievement((int)AchievementId.Ac03C, "Empire", "Besitze 200 Gebaeude gleichzeitig") },
            { AchievementId.Ac04, new Achievement((int)AchievementId.Ac04, "Lumber Cutter", "Platziere deine erstes lumbercamp") },
            { AchievementId.Ac05, new Achievement((int)AchievementId.Ac05, "Quarry Pioneer", "Platziere deine erste quarry") },
            { AchievementId.Ac06, new Achievement((int)AchievementId.Ac06, "Militarised", "Platziere deine erste Kaserne") },
            { AchievementId.Ac07, new Achievement((int)AchievementId.Ac07, "Barrack", "Gewinne mit nur einer Barracks") },
            { AchievementId.Ac08, new Achievement((int)AchievementId.Ac08, "Dream 2.0", "Gewinne eine Runde in nur 5 Minuten") },
            { AchievementId.Ac09, new Achievement((int)AchievementId.Ac09, "Roll Credits", "Gewinne nur mit Swordsmen") },
            { AchievementId.Ac10, new Achievement((int)AchievementId.Ac10, "Soloist", "Gewinne mit nur einer Einheit") },
        };
    }

    public void Load(AchievementsData achievementsData)
    {
        for (int i = 0; i < 16; i++)
        {
            mAchievements[(AchievementId)i].IsCompleted = achievementsData.mCompletion[i];
            mAchievements[(AchievementId)i].Progress = achievementsData.mProgress[i];
        }
    }
    public void Save(ref AchievementsData achievementsData)
    {
        for (int i = 0; i < 16; i++)
        {
            achievementsData.mCompletion[i] = mAchievements[(AchievementId)i].IsCompleted;
            achievementsData.mProgress[i] = mAchievements[(AchievementId)i].Progress;
        }
    }
    public Dictionary<string, Achievement> GetAchievements()
    {
        return mAchievements.ToDictionary(entry => entry.Value.Title, entry => entry.Value);
    }


    private float CalculateProgress(AchievementId achievementId, ResourceHud resourceHud,
                                    int ownedBuildings, int ownedLumbercamps, int ownedQuarrys,
                                    int ownedBarracks, int ownedSwordsman, int ownedUnits,
                                    bool win, TimeSpan gameDuration)
    {

        switch (achievementId)
        {
            case AchievementId.Ac01A:
                return Math.Min(highestStoneCount / 1000f, 1.0f);
            case AchievementId.Ac01B:
                return Math.Min(highestStoneCount / 5000f, 1.0f);
            case AchievementId.Ac01C:
                return Math.Min(highestStoneCount / 20000f, 1.0f);
            case AchievementId.Ac02A:
                return Math.Min(highestWoodCount / 1000f, 1.0f);
            case AchievementId.Ac02B:
                return Math.Min(highestWoodCount / 5000f, 1.0f);
            case AchievementId.Ac02C:
                return Math.Min(highestWoodCount / 20000f, 1.0f);
            case AchievementId.Ac03A:
                return Math.Min(highestBuildingCount / 10f, 1.0f);
            case AchievementId.Ac03B:
                return Math.Min(highestBuildingCount / 50f, 1.0f);
            case AchievementId.Ac03C:
                return Math.Min(highestBuildingCount / 200f, 1.0f);
            case AchievementId.Ac04:
                return highestLumberCount >= 1 ? 1.0f : 0.0f;
            case AchievementId.Ac05:
                return highestQuarryCount >= 1 ? 1.0f : 0.0f;
            case AchievementId.Ac06:
                return highestBarracksCount >= 1 ? 1.0f : 0.0f;
            case AchievementId.Ac07:
                return win && ownedBarracks == 1 ? 1.0f : 0.0f;
            case AchievementId.Ac08:
                return win && gameDuration.TotalMinutes < 5 ? 1.0f : 0.0f;
            case AchievementId.Ac09:
                return win && ownedSwordsman > 0 && ownedUnits == ownedSwordsman ? 1.0f : 0.0f;
            case AchievementId.Ac10:
                return win && ownedUnits == 1 ? 1.0f : 0.0f;
            default:
                return 0.0f;
        }
    }


    public void UpdateAchievementProgress(ResourceHud resourceHud, int ownedBuildings, int ownedLumbercamps,
                                          int ownedQuarrys, int ownedBarracks, int ownedSwordsman,
                                          int ownedUnits, bool win, TimeSpan gameDuration)
    {
        highestStoneCount = Math.Max(highestStoneCount, resourceHud.StoneCount);
        highestWoodCount = Math.Max(highestWoodCount, resourceHud.WoodCount);


        foreach (var achievementEntry in mAchievements)
        {
            AchievementId achievementId = achievementEntry.Key;
            Achievement achievement = achievementEntry.Value;
            achievement.Progress = CalculateProgress(achievementId, resourceHud, ownedBuildings,
                                                     ownedLumbercamps, ownedQuarrys, ownedBarracks,
                                                     ownedSwordsman, ownedUnits, win, gameDuration);
            achievement.IsCompleted = achievement.Progress >= 1.0f;
        }
    }



    // needs to get adjusted to changes of achievements
    public void Update(Dictionary<string, IGameObject> gameObjects, ResourceHud resourceHud, bool win, TimeSpan gameDuration)
    {
        // wood count
        if (resourceHud.StoneCount > 1000)
        { 
            mAchievements[AchievementId.Ac01A].IsCompleted = true;
        }
        if (resourceHud.StoneCount > 5000)
        {
            mAchievements[AchievementId.Ac01B].IsCompleted = true;
        }
        if (resourceHud.StoneCount > 20000)
        {
            mAchievements[AchievementId.Ac01C].IsCompleted = true;
        }
        // stone count
        if (resourceHud.WoodCount > 1000)
        {
            mAchievements[AchievementId.Ac02A].IsCompleted = true;
        }
        if (resourceHud.WoodCount > 5000)
        {
            mAchievements[AchievementId.Ac02B].IsCompleted = true;
        }
        if (resourceHud.WoodCount > 20000)
        {
            mAchievements[AchievementId.Ac02C].IsCompleted = true;
        }
        
        // gameObjects
        int ownedBuildings = 0;
        int ownedUnits = 0;
        int ownedSwordsman = 0;
        int ownedLumbercamps = 0;
        int ownedQuarrys = 0;
        int ownedBarracks = 0;
        foreach (var gameObject in gameObjects)
        {
            if (gameObject.Value is IBuilding building && building.Team == 0)
            {
                ownedBuildings++;
                switch (building)
                {
                    case Barracks:
                        ownedBarracks++;
                        break;
                    case Quarry:
                        ownedQuarrys++;
                        break;
                    case LumberCamp:
                        ownedLumbercamps++;
                        break;
                    default: break;
                }
                
            }

            if (gameObject.Value is IUnit unit && unit.Team == 0)
            {
                ownedUnits++;
                switch (unit)
                {
                    case Swordsman:
                        ownedSwordsman++;
                        break;
                    default: break;
                }
            }
        }
        highestBuildingCount = Math.Max(highestBuildingCount, ownedBuildings);
        highestLumberCount = Math.Max(highestLumberCount, ownedLumbercamps);
        highestQuarryCount = Math.Max(highestQuarryCount, ownedQuarrys);
        highestBarracksCount = Math.Max(highestBarracksCount, ownedBarracks);

        if (ownedBuildings >= 10)
        {
            mAchievements[AchievementId.Ac03A].IsCompleted = true;
        }
        if (ownedBuildings >= 50)
        {
            mAchievements[AchievementId.Ac03B].IsCompleted = true;
        }
        if (ownedBuildings >= 2000)
        {
            mAchievements[AchievementId.Ac03C].IsCompleted = true;
        }

        if (ownedLumbercamps >= 1)
        {
            mAchievements[AchievementId.Ac04].IsCompleted = true;
        }
        if (ownedQuarrys >= 1)
        {
            mAchievements[AchievementId.Ac05].IsCompleted = true;
        }
        if (ownedBarracks >= 1)
        {
            mAchievements[AchievementId.Ac06].IsCompleted = true;
        }

        // missing: win in less than 5 min
        
        if (win && ownedBarracks == 1)
        {
            mAchievements[AchievementId.Ac07].IsCompleted = true;
        }
        if (win && ownedSwordsman == 1)
        {
            mAchievements[AchievementId.Ac08].IsCompleted = true;
        }
        if (win && ownedUnits == 1)
        {
            mAchievements[AchievementId.Ac09].IsCompleted = true;
        }
        UpdateAchievementProgress(resourceHud, ownedBuildings, ownedLumbercamps, ownedQuarrys,
                          ownedBarracks, ownedSwordsman, ownedUnits, win, gameDuration);
    }
        
}
