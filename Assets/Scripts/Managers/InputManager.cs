using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get { return _instance; }
    }

    private Dictionary<ControlKeyType, KeyCode> _controlsDictionary = new Dictionary<ControlKeyType, KeyCode>();

    private GameAssets _gameAssets;
    private GamePlayCanvas _canvas;
    private PlayerController _playerController;
    private PlayerStats _playerStats;
    private GameManager _gameManager;
    private PlayerWeapons _playerWeapons;
    private TextConsoleUI _textConsoleUI;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (_instance != null && _instance != this)
            Destroy(_instance.gameObject);

        _instance = this;
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _canvas = GamePlayCanvas.Instance;
        _playerController = PlayerController.Instance;
        _playerWeapons = PlayerWeapons.Instance;
        _textConsoleUI = TextConsoleUI.Instance;
        _playerStats = PlayerStats.Instance;
        _gameManager = GameManager.Instance;

        setupInitialControls();
    }

    private void Update()
    {
        gameplayInput();
    }

    private void setupInitialControls()
    {
        foreach (ControlKey control in _gameAssets.ControlList.controls)
            _controlsDictionary.Add(control.ControlKeyType, control.KeyCode);

        if (_controlsDictionary.Values.Count != Enum.GetValues(typeof(ControlKeyType)).Length - 1)
        {
            Debug.LogError("Not all controls are available");
            Debug.LogError($"{_controlsDictionary.Values.Count} in dictionary, " +
                $"{Enum.GetValues(typeof(ControlKeyType)).Length - 1} in the enum");
            return;
        }
    }

    public static KeyCode GetKeyPressed()
    {
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyUp(keyCode))
                return keyCode;
        }

        return KeyCode.None;
    }

    public bool SetControlKey(ControlKeyType controlKeyType, KeyCode chosenKeyCode)
    {
        if (!_controlsDictionary.TryGetValue(controlKeyType, out KeyCode keyCode))
            return false;

        if (keyCode == KeyCode.None)
            return false;

        _controlsDictionary[controlKeyType] = chosenKeyCode;
        return true;
    }

    public bool KeyInUse(KeyCode chosenKeyCode)
    {
        foreach (var value in _controlsDictionary.Values)
            if (value == chosenKeyCode)
                return true;

        return false;
    }

    private void gameplayInput()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;

        togglePauseUI();
        playerInventoryInput();
        playerControllerInput();
        textConsoleSwitchParagraphs();
        playerWeaponsInput();
    }

    private void playerInventoryInput()
    {
        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.Inventory]))
            _canvas.ShowInventory();

        if (Input.GetKeyUp(_controlsDictionary[ControlKeyType.Collectibles]))
            _canvas.ShowCollectiblesUI();

        if (Input.GetKeyUp(_controlsDictionary[ControlKeyType.ToggleCrafting]))
            _canvas.ToggleCraftingUI();
    }

    private void playerControllerInput()
    {
        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.TriggerRunning]))
            _playerController.TriggerRunning();

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.HerbalBooster]))
            _playerController.HerbalBooster();

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.LimbProtector]))
            _playerController.LimbProtector();

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.LimbPatcher]))
            _playerController.LimbPatcher();
    }

    private void togglePauseUI()
    {
        if (Input.GetKeyUp(_controlsDictionary[ControlKeyType.PauseExit]))
            _canvas.TogglePauseUI();
    }

    private void textConsoleSwitchParagraphs()
    {
        if (Input.GetKeyUp(_controlsDictionary[ControlKeyType.TextConsoleSwitchParagraph]))
            _textConsoleUI.SwitchParagraphs();
    }

    private void playerWeaponsInput()
    {
        if (!_playerStats.IsAlive() || !_gameManager.IsGameRunning() || _gameManager.IsPaused())
            return;

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.Reload]))
            _playerWeapons.ReloadWeapon();

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.NextWeapon]))
            _playerWeapons.SwitchWeapon(1);

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.PreviousWeapon]))
            _playerWeapons.SwitchWeapon(-1);

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.NextThrowable]))
            _playerWeapons.SwitchThrowables();

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.ShowWeapon]))
            _playerWeapons.ShowWeapon();

        if (Input.GetKeyDown(_controlsDictionary[ControlKeyType.UseThrowable]))
            _playerWeapons.UseThrowable();
    }
}
