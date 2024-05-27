using TMPro;
using UnityEngine;

public class AchievementInfo : MonoBehaviour, IUiObject
{
    private Transform _container;
    private TextMeshProUGUI _title;
    private TextMeshProUGUI _description;
    private TextMeshProUGUI _value;

    private void Awake()
    {
        _container = transform.Find("Container");
        _title = _container.Find("Title").GetComponent<TextMeshProUGUI>();
        _description = _container.Find("Description").GetComponent<TextMeshProUGUI>();
        _value = _container.Find("Value").GetComponent<TextMeshProUGUI>();
    }

    public void ActivateUI(Achievement achievement)
    {
        GamePlayCanvas.AddOpenUiStatic(this);
        _title.SetText(achievement.AchievementType.ToString());
        _description.SetText(achievement.Description);
        _value.SetText(achievement.Value);
        _container.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        _title.ClearMesh();
        _description.ClearMesh();
        _value.ClearMesh();
        _container.gameObject.SetActive(false);
    }
}
