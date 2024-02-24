using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AchievementUI : MonoBehaviour
{
    private static AchievementUI _instance;
    public static AchievementUI Instance
    {
        get
        {
            return _instance;
        }
    }

    private TextMeshProUGUI _newAchievementsText;
    private TextMeshProUGUI _achievementNameText;
    private Animator _animator;

    private const string NEW_ACHIEVEMENTS_TEXT = "New achievements: ";

    private string _defaultString = "GoToDefault";
    private string _triggerString = "ShowAchievement";

    private void Awake()
    {
        _instance = this;
        _newAchievementsText = transform.Find("Container").Find("AchievementText").GetComponent<TextMeshProUGUI>();
        _achievementNameText = transform.Find("Container").Find("Type").GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    public void AchievementUnlocked(AchievementType type)
    {
        _animator.SetTrigger(_defaultString);
        _animator.SetTrigger(_triggerString);
        _newAchievementsText.SetText(NEW_ACHIEVEMENTS_TEXT + "1");
        _achievementNameText.SetText(type.ToString());
    }

    public void ManyAchievementsUnlocked(List<AchievementType> types)
    {
        _animator.SetTrigger(_defaultString);
        _animator.SetTrigger(_triggerString);
        _newAchievementsText.SetText(NEW_ACHIEVEMENTS_TEXT + types.Count.ToString());
        string achievementsText = string.Empty;
        foreach (AchievementType type in types)
            achievementsText += type.ToString() + "\n";
        
        _achievementNameText.SetText(achievementsText);
    }
}
