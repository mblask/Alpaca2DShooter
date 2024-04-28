using TMPro;
using UnityEngine;

public class InputButtonSelector : MonoBehaviour
{
    [SerializeField]
    private ControlKeyType _control;
    private AlpacaButtonUI _inputButton;
    private TextMeshProUGUI _characterText;

    private InputButtonSelectorContainer _selectorContainer;
    private InputManager _inputManager;
    private bool _keySelectionOpen = false;
    public bool KeySelectionOpen { get { return _keySelectionOpen; } set { _keySelectionOpen = value; } }

    private GamePlayCanvas _canvas;

    private void Awake()
    {
        _inputButton = transform.Find("Button").GetComponent<AlpacaButtonUI>();
        _characterText = transform.Find("Button").Find("Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _canvas = GamePlayCanvas.Instance;
        _inputButton.onLeftClick = () => { openKeySelection(); };
    }

    private void Update()
    {
        if (!_keySelectionOpen)
            return;

        if (Input.GetMouseButton(0))
            return;

        keySelectionProcess();
    }

    public void SetContainer(InputButtonSelectorContainer container)
    {
        _selectorContainer = container;
    }

    private void openKeySelection()
    {
        _selectorContainer.DisableAllSelectors();
        _keySelectionOpen = true;
        _canvas.ShowSelectKeyUI();
    }

    private void keySelectionProcess()
    {
        KeyCode chosenKeyCode = InputManager.GetKeyPressed();
        if (chosenKeyCode == KeyCode.None || chosenKeyCode == KeyCode.Mouse0)
            return;

        if (_inputManager.KeyInUse(chosenKeyCode))
        {
            _canvas.ShowKeyUsedUI();
            return;
        }

        changeKey(chosenKeyCode);
        _canvas.HideKeyUsedUI();
    }

    private void changeKey(KeyCode chosenKey)
    {
        if (!_inputManager.SetControlKey(_control, chosenKey))
            return;

        _characterText.SetText(chosenKey.ToString());
        _keySelectionOpen = false;
    }
}
