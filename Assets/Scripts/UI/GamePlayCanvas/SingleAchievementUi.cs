using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SingleAchievementUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string _title = string.Empty;
    public string Title => _title;
    private string _description = string.Empty;
    public string Description => _description;
    private Achievement _achievement;
    private TextMeshProUGUI _titleText;

    private GamePlayCanvas _canvas;

    private void Awake()
    {
        _titleText = transform.Find("Title").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;
    }

    public void SetupUI(Achievement achievement)
    {
        _achievement = achievement;
        _title = achievement.AchievementType.ToString();
        _description = achievement.Description;
        _titleText.SetText(achievement.AchievementType.ToString());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _canvas.ShowAchievementInfo(_achievement);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _canvas.HideAchievementInfo();
    }
}
