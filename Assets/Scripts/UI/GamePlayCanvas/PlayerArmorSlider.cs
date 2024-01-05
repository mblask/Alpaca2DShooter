using UnityEngine;
using UnityEngine.UI;

public class PlayerArmorSlider : MonoBehaviour
{
    private static PlayerArmorSlider _instance;
    public static PlayerArmorSlider Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _container;
    private Slider _playerArmorSlider;

    private PlayerStats _playerStats;

    private void Awake()
    {
        _instance = this;
        _container = transform.Find("Container");
        _playerArmorSlider = _container.Find("ArmorSlider").GetComponent<Slider>();
    }

    private void Start()
    {
        _playerStats = PlayerStats.Instance;
        UpdatePlayerArmorSlider(_playerStats.BodyArmor.GetCurrentValue());
    }

    public void UpdatePlayerArmorSlider(float value)
    {
        _container.gameObject.SetActive(value > 0.0f);

        _playerArmorSlider.maxValue = 100.0f;
        _playerArmorSlider.value = value;
    }
}
