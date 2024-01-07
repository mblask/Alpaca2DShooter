using UnityEngine;
using UnityEngine.UI;

public class WeaponImage : MonoBehaviour
{
    private static WeaponImage _instance;
    public static WeaponImage Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _imageTransform;
    private Image _image;
    private PlayerWeapons _playerWeapons;

    private void Awake()
    {
        _instance = this;
        _imageTransform = transform.Find("Image");
        _image = _imageTransform.GetComponent<Image>();
    }

    private void Start()
    {
        _playerWeapons = PlayerWeapons.Instance;
        activateUI(_playerWeapons.GetWeapons().Count > 0);
    }

    public void SetWeaponImage(Weapon weapon)
    {
        if (_image == null || weapon == null)
            return;

        _image.sprite = weapon.WeaponItem.ItemSprite;
    }

    private void activateUI(bool value)
    {
        _imageTransform.gameObject.SetActive(value);
    }
}
