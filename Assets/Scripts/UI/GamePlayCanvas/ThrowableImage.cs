using UnityEngine;
using UnityEngine.UI;

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

    private Image _image;

    private void Awake()
    {
        _instance = this;
        _image = transform.Find("Image").GetComponent<Image>();
    }

    public void SetThrowableImage(ThrowableWeapon throwable)
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
