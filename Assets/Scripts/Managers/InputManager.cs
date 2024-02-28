using UnityEngine;

public class InputManager : MonoBehaviour
{
    private KeyCode _pauseAndExit = KeyCode.Escape;
    private KeyCode _reloadWeapon = KeyCode.R;
    private KeyCode _showWeapon = KeyCode.F;
    private KeyCode _nextWeapon = KeyCode.E;
    private KeyCode _previousWeapon = KeyCode.Q;
    private KeyCode _nextThrowable = KeyCode.Tab;
    private KeyCode _useThrowable = KeyCode.G;
    private KeyCode _textConsoleSwitchParagraph = KeyCode.Space;
    private KeyCode _toggleCrafting = KeyCode.C;
    private KeyCode _inventory = KeyCode.I;
    private KeyCode _collectibles = KeyCode.T;
    private KeyCode _herbalBooster = KeyCode.Alpha1;
    private KeyCode _limbProtector = KeyCode.Alpha2;
    private KeyCode _limbPatcher = KeyCode.Alpha3;

    private GamePlayCanvas _canvas;
    private PlayerController _playerController;
    private PlayerStats _playerStats;
    private GameManager _gameManager;
    private PlayerWeapons _playerWeapons;
    private TextConsoleUI _textConsoleUI;

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;
        _playerController = PlayerController.Instance;
        _playerWeapons = PlayerWeapons.Instance;
        _textConsoleUI = TextConsoleUI.Instance;
        _playerStats = PlayerStats.Instance;
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        togglePauseUI();
        playerInventoryInput();
        playerControllerInput();
        textConsoleSwitchParagraphs();
        playerWeaponsInput();
    }

    private void playerInventoryInput()
    {
        if (Input.GetKeyDown(_inventory))
            _canvas.ShowInventory();

        if (Input.GetKeyUp(_collectibles))
            _canvas.ShowCollectiblesUI();

        if (Input.GetKeyUp(_toggleCrafting))
            _canvas.ToggleCraftingUI();
    }

    private void playerControllerInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            _playerController.TriggerRunning();

        if (Input.GetKeyDown(_herbalBooster))
            _playerController.HerbalBooster();

        if (Input.GetKeyDown(_limbProtector))
            _playerController.LimbProtector();

        if (Input.GetKeyDown(_limbPatcher))
            _playerController.LimbPatcher();
    }

    private void togglePauseUI()
    {
        if (Input.GetKeyUp(_pauseAndExit))
            _canvas.TogglePauseUI();
    }

    private void textConsoleSwitchParagraphs()
    {
        if (Input.GetKeyUp(_textConsoleSwitchParagraph))
            _textConsoleUI.SwitchParagraphs();
    }

    private void playerWeaponsInput()
    {
        if (!_playerStats.IsAlive() || !_gameManager.IsGameRunning() || _gameManager.IsPaused())
            return;

        if (Input.GetKeyDown(_reloadWeapon))
            _playerWeapons.ReloadWeapon();

        if (Input.GetKeyDown(_nextWeapon))
            _playerWeapons.SwitchWeapon(1);

        if (Input.GetKeyDown(_previousWeapon))
            _playerWeapons.SwitchWeapon(-1);

        if (Input.GetKeyDown(_nextThrowable))
            _playerWeapons.SwitchThrowables();

        if (Input.GetKeyDown(_showWeapon))
            _playerWeapons.ShowWeapon();

        if (Input.GetKeyDown(_useThrowable))
            _playerWeapons.UseThrowable();
    }
}
