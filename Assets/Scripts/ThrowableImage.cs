using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThrowableImage : MonoBehaviour
{
    private static ThrowableImage _instance;

    private Transform _container;
    private Image _image;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _instance = this;
        _container = transform.Find("Container");
        _image = _container.Find("Image").GetComponent<Image>();
        _text = _container.Find("AmountText").GetComponent<TextMeshProUGUI>();
    }

    public static void UpdateThrowableUIStatic(ThrowableWeapon throwable)
    {
        _instance?.updateThrowableUI(throwable);
    }

    private void updateThrowableUI(ThrowableWeapon throwable)
    {
        if (throwable == null || throwable.TotalAmmo == 0)
        {
            _image.enabled = false;
            _text.SetText("");
            return;
        }

        _image.sprite = throwable.ThrowableItem.ItemSprite;
        _image.enabled = true;
        _text.SetText(throwable.TotalAmmo.ToString());
    }
}
