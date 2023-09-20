using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    //Tags
    public const string ENEMY_TAG = "Enemy";

    //Interaction
    public const float SHORT_INTERACTION_DISTANCE = 2.0f;
    public const float INTERACTION_DISTANCE = 3.0f;

    //Colors
    public static readonly Color HIGHLIGHT_COLOR = new Color(0.6f, 1.0f, 0.6f, 1.0f);
    public static readonly Color DEFAULT_COLOR = Color.white;

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