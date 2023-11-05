using System.Collections.Generic;

public class AchievementConstants
{
    private Dictionary<AchievementType, string> _achievementDescriptionDictionary =
        new Dictionary<AchievementType, string>
        {
            { AchievementType.None, "N/A" },
            { AchievementType.Lightning, "N/A" },
            { AchievementType.Bloodthirst, "N/A" },
            { AchievementType.Pacifist, "N/A" },
            { AchievementType.Ironman, "N/A" },
            { AchievementType.Survivalist, "N/A" },
            { AchievementType.Crafter, "N/A" },
            { AchievementType.Medic, "N/A" },
            { AchievementType.Sniper, "N/A" },
            { AchievementType.Hacker, "N/A" },
        };
}
