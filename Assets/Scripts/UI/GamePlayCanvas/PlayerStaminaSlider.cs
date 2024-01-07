using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaSlider : MonoBehaviour
{
    private Slider _playerStaminaSlider;

    private void Awake()
    {
        _playerStaminaSlider = transform.Find("StaminaSlider").GetComponent<Slider>();
    }

    public void UpdatePlayerStaminaSlider(float value)
    {
        if (_playerStaminaSlider == null)
            return;

        _playerStaminaSlider.maxValue = PlayerStats.Instance.Stamina.GetFinalValue();
        _playerStaminaSlider.value = value;
    }
}
