using TMPro;
using UnityEngine;

public class ThrowableAmmoPanel : MonoBehaviour
{
    private static ThrowableAmmoPanel _instance;
    public static ThrowableAmmoPanel Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _container;
    private TextMeshProUGUI _ammoText;

    private void Awake()
    {
        _instance = this;
        _container = transform.Find("Container");
        _ammoText = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateAmmoText(int value)
    {
        _container.gameObject.SetActive(value > 0);
        _ammoText.SetText(value.ToString());
    }
}
