using UnityEngine;

public class InputButtonSelector : MonoBehaviour
{
    private ControlKeyType _control;
    private AlpacaButtonUI _inputButton;

    private InputManager _inputManager;
    private bool _keySelectionOpen = false;

    private void Awake()
    {
        _inputButton = transform.Find("Button").GetComponent<AlpacaButtonUI>();
    }

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _inputButton.onLeftClick = () => { openKeySelection(); };
    }

    private void Update()
    {
        if (!_keySelectionOpen)
            return;

        keySelectionProcess();
    }

    private void openKeySelection()
    {
        _keySelectionOpen = true;
    }

    private void keySelectionProcess()
    {
        KeyCode chosenKeyCode = InputManager.GetKeyPressed();
        if (chosenKeyCode == KeyCode.None)
            return;

        if (_inputManager.KeyInUse(chosenKeyCode))
        {
            Debug.Log("Key already used");
            return;
        }

        _inputManager.SetControlKey(_control, chosenKeyCode);
        _keySelectionOpen = false;
    }
}
