using TMPro;
using UnityEngine;

public class CreditPanel : MonoBehaviour
{
    private static CreditPanel _instance;
    public static CreditPanel Instance
    {
        get
        {
            return _instance;
        }
    }

    private TextMeshProUGUI _creditsText;

    private const string CREDITS = "Credits: ";

    private void Awake()
    {
        _instance = this;
        _creditsText = transform.Find("Panel").Find("CreditText").GetComponent<TextMeshProUGUI>();
    }

    public void UpdateCreditsText(float credits)
    {
        string creditsText = CREDITS + credits.ToString("F1");
        _creditsText.SetText(creditsText);
    }
}
