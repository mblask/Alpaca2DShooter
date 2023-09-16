using UnityEngine;
using UnityEngine.UI;

public class LevelProgressChoiceUI : MonoBehaviour
{
    private static LevelProgressChoiceUI _instance;

    public static LevelProgressChoiceUI Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _container;
    private Button _readyButton;
    private Button _cancelButton;

    private LevelsManager _levelsManager;

    private void Awake()
    {
        _instance = this;

        _container = transform.Find("Container");
        _readyButton = transform.Find("Container").Find("ReadyButton").GetComponent<Button>();
        _cancelButton = transform.Find("Container").Find("CancelButton").GetComponent<Button>();
    }

    private void Start()
    {
        _levelsManager = LevelsManager.Instance;

        _readyButton.onClick.AddListener(readyButton);
        _cancelButton.onClick.AddListener(cancelButton);
    }

    public void ShowUI()
    {
        activateUI(true);
    }

    private void hideUI()
    {
        activateUI(false);
    }

    private void activateUI(bool value)
    {
        _container.gameObject.SetActive(value);
    }

    private void cancelButton()
    {
        hideUI();
    }

    private void readyButton()
    {
        LevelsManager.TransferPlayerToBossLevelStatic();
        hideUI();
    }
}
