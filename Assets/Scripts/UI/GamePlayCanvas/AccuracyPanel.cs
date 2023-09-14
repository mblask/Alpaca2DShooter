using UnityEngine;
using TMPro;

public class AccuracyPanel : MonoBehaviour
{
    private static AccuracyPanel _instance;
    public static AccuracyPanel Instance
    {
        get
        {
            return _instance;
        }
    }

    private TextMeshProUGUI _accuracyText;

    public void Awake()
    {
        _instance = this;
        _accuracyText = transform.Find("AccuracyText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {        
        SetupAccuracyText(0.0f);
    }

    public void SetupAccuracyText(float accuracy)
    {
        accuracy *= 100.0f;
        _accuracyText.SetText(accuracy.ToString("F0") + "%");
    }
}
