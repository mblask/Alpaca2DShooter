using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InputButtonSelector : MonoBehaviour
{
    private ControlKeyType _control;
    private AlpacaButtonUI _inputButton;
    private TextMeshProUGUI _characterText;

    private InputButtonSelectorContainer _selectorContainer;
    private InputManager _inputManager;
    private bool _keySelectionOpen = false;
    public bool KeySelectionOpen { get { return _keySelectionOpen; } set { _keySelectionOpen = value; } }

    private void Awake()
    {
        _inputButton = transform.Find("Button").GetComponent<AlpacaButtonUI>();
        _characterText = transform.Find("Button").Find("Text").GetComponent<TextMeshProUGUI>();
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

    public void SetContainer(InputButtonSelectorContainer container)
    {
        _selectorContainer = container;
    }

    private void openKeySelection()
    {
        _selectorContainer.DisableAllSelectors();
        _keySelectionOpen = true;
    }

    private void keySelectionProcess()
    {
        KeyCode chosenKeyCode = InputManager.GetKeyPressed();
        if (chosenKeyCode == KeyCode.None || chosenKeyCode == KeyCode.Mouse0)
            return;

        if (_inputManager.KeyInUse(chosenKeyCode))
        {
            Debug.Log("Key already used");
            return;
        }

        changeKey(chosenKeyCode);
    }

    private void changeKey(KeyCode chosenKey)
    {
        _inputManager.SetControlKey(_control, chosenKey);
        _characterText.SetText(chosenKey.ToString());
        _keySelectionOpen = false;
    }
}
