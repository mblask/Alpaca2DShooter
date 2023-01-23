using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponImage : MonoBehaviour
{
    private Transform _imageTransform;
    private Image _image;
    private PlayerWeapons _playerWeapons;

    private void Awake()
    {
        _imageTransform = transform.Find("Image");
        _image = _imageTransform.GetComponent<Image>();
    }

    private void Start()
    {
        _playerWeapons = PlayerWeapons.Instance;
        activateUI(_playerWeapons.GetWeapons().Count > 0);
        _playerWeapons.OnWeaponChanged += PlayerWeapons_OnWeaponsChanged;
    }

    public void PlayerWeapons_OnWeaponsChanged(PlayerWeapons playerWeapons)
    {
        Weapon currentPlayerWeapon = playerWeapons.GetCurrentWeapon();

        SetWeaponImage(currentPlayerWeapon);
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