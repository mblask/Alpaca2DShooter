using System.Collections.Generic;

public static class Constants
{
    //Tags
    public const string ENEMY_TAG = "Enemy";

    //NPC AI
    public static readonly List<string> NPC_RANDOM_MESSAGES = new List<string>
    {
        "*Hmmph*", "*Grunt*", "*Ahh*", "What a'\n'boring day..."
    };
    public static readonly List<string> NPC_ALERT_MESSAGES = new List<string>
    {
        "What was that?", "Huh??", "Who is there?", "Was that a shot??"
    };
}