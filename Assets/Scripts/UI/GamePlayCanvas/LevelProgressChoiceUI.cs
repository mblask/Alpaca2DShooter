using UnityEngine;
using UnityEngine.UI;

public class LevelProgressChoiceUI : MonoBehaviour, IUiObject
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

    private void Awake()
    {
        _instance = this;

        _container = transform.Find("Container");
        _readyButton = transform.Find("Container").Find("ReadyButton").GetComponent<Button>();
        _cancelButton = transform.Find("Container").Find("CancelButton").GetComponent<Button>();
    }

    private void Start()
    {
        _readyButton.onClick.AddListener(readyButton);
        _cancelButton.onClick.AddListener(HideUI);
    }

    public void ShowUI(bool value)
    {
        if (_container == null)
            return;

        if (value)
            GamePlayCanvas.AddOpenUiStatic(this);
        else
            GamePlayCanvas.RemoveOpenUiStatic(this);

        _container.gameObject?.SetActive(value);
    }

    public void HideUI()
    {
        ShowUI(false);
    }

    private void readyButton()
    {
        LevelsManager.TransferPlayerToBossLevelStatic();
        ShowUI(false);
    }
}
