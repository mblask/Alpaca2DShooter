using UnityEngine;

public class InputManager : MonoBehaviour
{
    private KeyCode _reloadWeapon = KeyCode.R;
    private KeyCode _prepareWeapon = KeyCode.F;
    private KeyCode _nextWeapon = KeyCode.E;
    private KeyCode _previousWeapon = KeyCode.Q;
    private KeyCode _nextThrowable = KeyCode.Tab;
    private KeyCode _switchTextConsole = KeyCode.Space;
    private KeyCode _toggleCrafting = KeyCode.C;
    private KeyCode _inventory = KeyCode.I;
    private KeyCode _collectibles = KeyCode.T;
    private KeyCode _herbalBooster = KeyCode.Alpha1;
    private KeyCode _limbProtector = KeyCode.Alpha2;
    private KeyCode _limbPatcher = KeyCode.Alpha3;

    private GamePlayCanvas _canvas;
    private PlayerInventory _playerInventory;
    private PlayerController _playerController;
    private PlayerWeapons _playerWeapons;

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;
        _playerInventory = PlayerInventory.Instance;
        _playerController = PlayerController.Instance;
        _playerWeapons = PlayerWeapons.Instance;
    }

    private void Update()
    {
        togglePlayerInventory();
        toggleCollectibles();
        toggleCrafting();
        triggerRunning();
        herbalBooster();
        limbProtector();
        limbPatcher();
    }

    private void togglePlayerInventory()
    {
        if (Input.GetKeyDown(_inventory))
            _canvas.ShowInventory();
    }

    private void toggleCollectibles()
    {
        if (Input.GetKeyUp(_collectibles))
            _canvas.ShowCollectiblesUI();
    }

    private void toggleCrafting()
    {
        if (Input.GetKeyUp(_toggleCrafting))
            _playerInventory.ToggleCraftingUI();
    }

    private void triggerRunning()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            _playerController.TriggerRunning();
    }

    private void herbalBooster()
    {
        if (Input.GetKeyDown(_herbalBooster))
            _playerController.HerbalBooster();
    }

    private void limbProtector()
    {
        if (Input.GetKeyDown(_limbProtector))
            _playerController.LimbProtector();
    }

    private void limbPatcher()
    {
        if (Input.GetKeyDown(_limbPatcher))
            _playerController.LimbPatcher();
    }
}
