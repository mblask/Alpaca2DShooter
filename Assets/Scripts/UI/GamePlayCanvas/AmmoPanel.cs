using UnityEngine;
using TMPro;

public class AmmoPanel : MonoBehaviour
{
    private static AmmoPanel _instance;
    public static AmmoPanel Instance
    {
        get
        {
            return _instance;
        }
    }

    private TextMeshProUGUI _ammoText;

    private void Awake()
    {
        _instance = this;
        _ammoText = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateAmmoText(int value, int total)
    {
        _ammoText.SetText(value.ToString() + " / " + total.ToString());
    }
}
