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

    private TextMeshProUGUI _text;
    private Animator _animator;

    private string _defaultString = "GoToDefault";
    private string _triggerString = "ShowAchievement";

    private void Awake()
    {
        _instance = this;
        _text = transform.Find("Container").Find("Type").GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    public void UpdateText(AchievementType type)
    {
        _animator.SetTrigger(_defaultString);
        _animator.SetTrigger(_triggerString);
        _text.SetText(type.ToString());
    }
}
