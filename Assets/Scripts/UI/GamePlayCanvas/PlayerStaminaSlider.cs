using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaSlider : MonoBehaviour
{
    private Slider _playerStaminaSlider;

    private void Awake()
    {
        _playerStaminaSlider = transform.Find("StaminaSlider").GetComponent<Slider>();
    }

    private void Start()
    {
        PlayerStats.Instance.OnStaminaUIUpdate += UpdatePlayerStaminaSlider;
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnStaminaUIUpdate -= UpdatePlayerStaminaSlider;
    }

    public void UpdatePlayerStaminaSlider(float value)
    {
        if (_playerStaminaSlider == null)
            return;

        _playerStaminaSlider.maxValue = PlayerStats.Instance.Stamina.GetFinalValue();

        _playerStaminaSlider.value = value;
    }
}
