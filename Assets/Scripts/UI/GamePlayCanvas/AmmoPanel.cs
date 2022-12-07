using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoPanel : MonoBehaviour
{
    private TextMeshProUGUI _ammoText;
    private PlayerWeapons _playerWeapons;

    private void Awake()
    {
        _ammoText = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        _playerWeapons = PlayerWeapons.Instance;

        _playerWeapons.OnAmmoPanelUIChanged += UpdateAmmoText;
    }

    private void OnDisable()
    {
        if (_playerWeapons != null)
            _playerWeapons.OnAmmoPanelUIChanged -= UpdateAmmoText;
    }

    public void UpdateAmmoText(int value, int total)
    {
        _ammoText.SetText(value.ToString() + " / " + total.ToString());
    }
}
