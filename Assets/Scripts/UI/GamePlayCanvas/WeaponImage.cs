using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponImage : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Image _image;
    private PlayerWeapons _playerWeapons;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _text = transform.Find("DurabilityText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _playerWeapons = PlayerWeapons.Instance;

        //_playerWeapons.OnShooting += PlayerWeapons_UpdateDurabilityText;
        _playerWeapons.OnWeaponChanged += PlayerWeapons_OnWeaponsChanged;

        //UpdateDurabilityText(_playerWeapons.GetCurrentWeapon());
    }

    public void PlayerWeapons_OnWeaponsChanged(PlayerWeapons playerWeapons)
    {
        Weapon currentPlayerWeapon = playerWeapons.GetCurrentWeapon();

        SetWeaponImage(currentPlayerWeapon);
        //UpdateDurabilityText(currentPlayerWeapon.Durability);
    }

    public void SetWeaponImage(Weapon weapon)
    {
        if (_image == null || weapon == null)
            return;

        _image.sprite = weapon.WeaponItem.ItemSprite;
    }

    //public void PlayerWeapons_UpdateDurabilityText(PlayerWeapons playerWeapons)
    //{
    //    UpdateDurabilityText(playerWeapons.GetCurrentWeapon().Durability);
    //}

    public void UpdateDurabilityText(float value)
    {
        _text.SetText("Dur: " + value.ToString("F0"));
    }

    public void UpdateDurabilityText(Weapon weapon)
    {
        if (weapon == null)
        {
            _text.SetText("No weapon");
        }
        else
        {
            _text.SetText("Dur: " + weapon.Durability.ToString("F0"));
        }
    }
}
