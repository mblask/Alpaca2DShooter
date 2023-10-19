using UnityEngine;

public class TerminalUI : MonoBehaviour
{
    private static TerminalUI _instance;
    public static TerminalUI Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _container;
    private AlpacaButtonUI _turnOffTrapsButton;
    private AlpacaButtonUI _showExitButton;
    private AlpacaButtonUI _switchAllegianceButton;

    private Terminal _terminal;

    private Camera _mainCamera;

    private void Awake()
    {
        _instance = this;
        _container = transform.Find("Container");
        _turnOffTrapsButton = _container.Find("TurnOffTrapsButton").GetComponent<AlpacaButtonUI>();
        _showExitButton = _container.Find("ShowExitButton").GetComponent<AlpacaButtonUI>();
        _switchAllegianceButton = _container.Find("SwitchAllegianceButton").GetComponent<AlpacaButtonUI>();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _turnOffTrapsButton.onLeftClick = () => { _terminal?.TurnOnOff(); };
        _showExitButton.onLeftClick = () => { _terminal?.ShowExitPortal(); };
        _switchAllegianceButton.onLeftClick = () => { _terminal?.Hack(); };
    }

    private void Update()
    {
        if (_terminal == null)
            return;

        if (!_container.gameObject.activeSelf)
            return;

        float yOffset = 1.0f;
        transform.position =
             _mainCamera.WorldToScreenPoint(_terminal.transform.position + Vector3.up * yOffset);
    }

    public void ActivateUI(bool value)
    {
        _container.gameObject.SetActive(value);
    }

    public void SetTerminal(Terminal terminal)
    {
        _terminal = terminal;
        ActivateUI(true);
    }
}
