using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSlider : MonoBehaviour
{
    private Slider _playerHealthSlider;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _playerHealthSlider = transform.Find("HealthSlider").GetComponent<Slider>();
    }

    private void Start()
    {
        _playerStats = PlayerStats.Instance;
        PlayerStats.Instance.OnHealthUIUpdate += UpdatePlayerHealthSlider;
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnHealthUIUpdate -= UpdatePlayerHealthSlider;
    }

    public void UpdatePlayerHealthSlider(float value)
    {
        if (value < 0.0f)
            value = 0.0f;

        _playerHealthSlider.maxValue = _playerStats.Health.GetFinalValue();
        _playerHealthSlider.value = value;
    }
}
