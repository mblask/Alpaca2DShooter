using UnityEngine;
using UnityEngine.UI;

public class PlayerOrBossLevelUI : MonoBehaviour
{
    private static PlayerOrBossLevelUI _instance;
    public static PlayerOrBossLevelUI Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _container;
    private Button _cancelButton;
    private Button _playerButton;
    private Button _bossButton;

    private LevelsManager _levelsManager;

    private void Awake()
    {
        _instance = this;
        _container = transform.Find("Container");
        _cancelButton = _container.Find("CancelButton").GetComponent<Button>();
        _playerButton = _container.Find("PlayerLevelButton").GetComponent<Button>();
        _bossButton = _container.Find("BossLevelButton").GetComponent<Button>();
    }

    private void Start()
    {
        _levelsManager = LevelsManager.Instance;

        _cancelButton.onClick.AddListener(() =>
        {
            ActivateUI(false);
        });

        _playerButton.onClick.AddListener(() =>
        {
            _levelsManager.TransferPlayerToLevel(LevelType.Player);
            ActivateUI(false);
        });

        _bossButton.onClick.AddListener(() =>
        {
            _levelsManager.TransferPlayerToLevel(LevelType.Boss);
            ActivateUI(false);
        });
    }

    public void ActivateUI(bool value)
    {
        if (_container == null)
            return;

        if (value.Equals(_container.gameObject.activeSelf))
            return;

        _container.gameObject.SetActive(value);
    }
}
