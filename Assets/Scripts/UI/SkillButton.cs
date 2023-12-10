using TMPro;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = transform.Find("Name").GetComponent<TextMeshProUGUI>();
    }

    public void SetButtonText(string text)
    {
        _text.SetText(text);
    }
}
