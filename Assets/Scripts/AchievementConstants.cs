using System.Collections.Generic;

public class AchievementConstants
{
    public const float LIGHTNING_GOAL = 120.0f;
    public const int PACIFIST_GOAL = 0;
    public const int BLOODTHIRST_GOAL = 50;
    public const float SURVIVALIST_GOAL = 0.05f;
    public const float IRONMAN_GOAL = 0.1f;
    public const float SNIPER_GOAL = 0.8f;
    public const int CRAFTER_GOAL = 20;
    public const int MEDIC_GOAL = 20;
    public const int HACKER_GOAL = 5;

    private const string _noneDescription = "N/A";
    private const string _lightningDescription = "Lightning fast movement through the maze. You finish the game in a record time.";
    private const string _bloodthirstDescription = "Nobody stands a chance against you. You reach an enemy-elimination-milestone, 50 killed.";
    private const string _pacifistDescription = "You are not one for the fight. You kill no one in your quest to get to the end.";
    private const string _ironmanDescription = "Bullets, what are bullets? You are basically inpenetrable to your enemy's guns. You lose less than 10% of your health.";
    private const string _survivalistDescription = "You, sir, clearly know your limits. You finish the game with less than 5% of you health.";
    private const string _crafterDescription = "Natural born engineer. Nothing is impossible for you to craft and you love it. You've crafted 20 items.";
    private const string _medicDescription = "Healing is the path of your choice. Nothing can stop you if you know how to heal yourself. You've used 20 healing items.";
    private const string _sniperDescription = "Once you aim, nothing can escape your crosshairs. You've finished the game with an accuracy greater than 80%.";
    private const string _hackerDescription = "Zeros and ones, that's all there is for you. You've successfully hacked 10 terminals.";

    private static Dictionary<AchievementType, Achievement> _achievementDictionary =
        new Dictionary<AchievementType, Achievement>
    {
            { AchievementType.None,
                new Achievement { AchievementType = AchievementType.None, Description = _noneDescription } },
            { AchievementType.Lightning,
                new Achievement { AchievementType = AchievementType.Lightning, Description = _lightningDescription } },
            { AchievementType.Bloodthirst,
                new Achievement { AchievementType = AchievementType.Bloodthirst, Description = _bloodthirstDescription } },
            { AchievementType.Pacifist,
                new Achievement { AchievementType = AchievementType.Pacifist, Description = _pacifistDescription } },
            { AchievementType.Ironman,
                new Achievement { AchievementType = AchievementType.Ironman, Description = _ironmanDescription } },
            { AchievementType.Survivalist,
                new Achievement { AchievementType = AchievementType.Survivalist, Description = _survivalistDescription } },
            { AchievementType.Crafter,
                new Achievement { AchievementType = AchievementType.Crafter, Description = _crafterDescription } },
            { AchievementType.Medic,
                new Achievement { AchievementType = AchievementType.Medic, Description = _medicDescription } },
            { AchievementType.Sniper,
                new Achievement { AchievementType = AchievementType.Sniper, Description = _sniperDescription } },
            { AchievementType.Hacker,
                new Achievement { AchievementType = AchievementType.Hacker, Description = _hackerDescription } }
    };

    public static Achievement GetAchievement(AchievementType type, string value = null)
    {
        if (!_achievementDictionary.TryGetValue(type, out Achievement result))
        {
            return new Achievement { AchievementType = AchievementType.None };
        }

        if (!string.IsNullOrEmpty(value))
            result.Value = value;

        return result;
    }
}
