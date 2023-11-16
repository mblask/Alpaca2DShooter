using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThrowableImage : MonoBehaviour
{
    private static ThrowableImage _instance;
    public static ThrowableImage Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _container;
    private Image _image;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _instance = this;
        _image = transform.Find("Image").GetComponent<Image>();
    }

    public void UpdateThrowableUI(ThrowableWeapon throwable)
    {
        if (throwable == null || throwable.TotalAmmo == 0)
        {
            _image.enabled = false;
            return;
        }

        _image.sprite = throwable.ThrowableItem.ItemSprite;
        _image.enabled = true;
    }
}
