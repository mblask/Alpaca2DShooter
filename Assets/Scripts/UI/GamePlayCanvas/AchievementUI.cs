using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementUI : MonoBehaviour, IUiObject
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
    private const string MANY_ACHIEVEMENTS_TEXT = "Check main menu";

    private string _defaultTriggerString = "GoToDefault";
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
        _animator.SetTrigger(_defaultTriggerString);
        _animator.SetTrigger(_triggerString);
        _newAchievementsText.SetText(NEW_ACHIEVEMENTS_TEXT + "1");
        _achievementNameText.SetText(type.ToString());
        GamePlayCanvas.AddOpenUiStatic(this);
    }

    public void ManyAchievementsUnlocked(List<AchievementType> types)
    {
        _animator.SetTrigger(_defaultTriggerString);
        _animator.SetTrigger(_triggerString);
        _newAchievementsText.SetText(NEW_ACHIEVEMENTS_TEXT + types.Count.ToString());
        _achievementNameText.SetText(MANY_ACHIEVEMENTS_TEXT);
        GamePlayCanvas.AddOpenUiStatic(this);
    }

    public void HideUI()
    {
        if (_animator == null)
            return;

        _animator.SetTrigger(_defaultTriggerString);
        GamePlayCanvas.RemoveOpenUiStatic(this);
    }
}
