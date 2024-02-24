using System.Collections.Generic;
using UnityEngine;

public class GamePlayCanvas : MonoBehaviour
{
    private static GamePlayCanvas _instance;
    public static GamePlayCanvas Instance
    {
        get
        {
            return _instance;
        }
    }

    private GameManager _gameManager;

    private AmmoPanel _ammoPanel;
    private PlayerArmorSlider _armorSlider;
    private PlayerHealthSlider _healthSlider;
    private PlayerStaminaSlider _staminaSlider;
    private WeaponImage _weaponImage;
    private ThrowableAmmoPanel _throwableAmmoPanel;
    private ThrowableImage _throwableImage;
    private ItemTooltip _itemTooltip;
    private InventoryUI _inventoryUI;
    private CollectiblesUI _collectiblesUI;
    private CraftingUI _craftingUI;
    private ConsumableItemsUIMenu _consumableItemsMenuUI;
    private WoundedUI _woundedUI;
    private LevelProgressChoiceUI _levelProgressChoiceUI;
    private InteractableTooltipUI _interactableTooltipUI;
    private TerminalUI _terminalUI;
    private PlayerOrBossLevelUI _playerOrBossLevelUI;
    private AchievementUI _achievementUI;
    private TextConsoleUI _textConsoleUI;
    private PauseMenu _pauseMenu;
    private InstanceCompleteUI _instanceCompleteUI;

    private bool _isPaused = false;

    private void Awake()
    {
        _instance = this;

        _ammoPanel = transform.GetComponentInChildren<AmmoPanel>();
        _armorSlider = transform.GetComponentInChildren<PlayerArmorSlider>();
        _healthSlider = transform.GetComponentInChildren<PlayerHealthSlider>();
        _staminaSlider = transform.GetComponentInChildren<PlayerStaminaSlider>();
        _weaponImage = transform.GetComponentInChildren<WeaponImage>();
        _throwableAmmoPanel = transform.GetComponentInChildren<ThrowableAmmoPanel>();
        _throwableImage = transform.GetComponentInChildren<ThrowableImage>();
        _itemTooltip = transform.GetComponentInChildren<ItemTooltip>();
        _inventoryUI = transform.GetComponentInChildren<InventoryUI>();
        _collectiblesUI = transform.GetComponentInChildren<CollectiblesUI>();
        _craftingUI = transform.GetComponentInChildren<CraftingUI>();
        _consumableItemsMenuUI = transform.GetComponentInChildren<ConsumableItemsUIMenu>();
        _woundedUI = transform.GetComponentInChildren<WoundedUI>();
        _levelProgressChoiceUI = transform.GetComponentInChildren<LevelProgressChoiceUI>();
        _interactableTooltipUI = transform.GetComponentInChildren<InteractableTooltipUI>();
        _terminalUI = transform.GetComponentInChildren<TerminalUI>();
        _playerOrBossLevelUI = transform.GetComponentInChildren<PlayerOrBossLevelUI>();
        _achievementUI = transform.GetComponentInChildren<AchievementUI>();
        _textConsoleUI = transform.GetComponentInChildren<TextConsoleUI>();
        _pauseMenu = transform.GetComponentInChildren<PauseMenu>();
        _instanceCompleteUI = transform.GetComponentInChildren<InstanceCompleteUI>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void UpdateAmmoText(int value, int total)
    {
        _ammoPanel.UpdateAmmoText(value, total);
    }

    public void UpdatePlayerArmorSlider(float value)
    {
        _armorSlider.UpdatePlayerArmorSlider(value);
    }

    public void UpdatePlayerHealthSlider(float value)
    {
        _healthSlider.UpdatePlayerHealthSlider(value);
    }

    public void UpdatePlayerStaminaSlider(float value)
    {
        _staminaSlider.UpdatePlayerStaminaSlider(value);
    }

    public void SetWeaponImage(Weapon weapon)
    {
        _weaponImage.SetWeaponImage(weapon);
    }

    public void UpdateThrowableAmmoText(int value)
    {
        _throwableAmmoPanel.UpdateAmmoText(value);
    }

    public void SetThrowableImage(ThrowableWeapon throwable)
    {
        _throwableImage.SetThrowableImage(throwable);
    }

    public void SetupItemTooltip(Item item)
    {
        _itemTooltip.SetupTooltip(item);
    }

    public void RemoveItemTooltip()
    {
        _itemTooltip.RemoveTooltip();
    }

    public void ShowInventory()
    {
        _inventoryUI.ShowInventory();
    }

    public void UpdateItemContainer()
    {
        _inventoryUI.UpdateItemContainer();
    }

    public void ShowCollectiblesUI()
    {
        _collectiblesUI.ShowUI();
    }

    public bool CraftingIsActive()
    {
        return _craftingUI.IsActive();
    }

    public void ShowCraftingUI(bool value)
    {
        _craftingUI.ShowCraftingUI(value);
    }

    public void PopulateCraftingSlots(CraftingRecipe recipe)
    {
        _craftingUI.PopulateCraftingSlots(recipe);
    }

    public void PopulateConsumableItemsUI()
    {
        _consumableItemsMenuUI.PopulateConsumableItemsUI();
    }

    public void ActivateWoundedUI()
    {
        _woundedUI.ActivateUI();
    }

    public void ShowLevelProgressChoiceUI(bool value)
    {
        _levelProgressChoiceUI.ShowUI(value);
    }

    public void SetInteractableTooltip(Transform objectToFollow, string text)
    {
        _interactableTooltipUI.SetTooltip(objectToFollow, text);
    }

    public void HideInteractableTooltip()
    {
        _interactableTooltipUI.HideTooltip();
    }

    public void ActivateTerminalUI(bool value)
    {
        _terminalUI.ActivateUI(value);
    }

    public void InsertedDataItem(DataItem dataItem)
    {
        _terminalUI.InsertedDataItem(dataItem);
    }

    public void UpdateButtonAvailability(float hackingSkill)
    {
        _terminalUI.UpdateButtonAvailability(hackingSkill);
    }

    public void AddDataItemsUI(List<DataItem> dataItems)
    {
        _terminalUI.AddDataItemsUI(dataItems);
    }

    public void SetTerminal(Terminal terminal)
    {
        _terminalUI.SetTerminal(terminal);
    }

    public void AddDataItem(DataItem dataItem)
    {
        _terminalUI.AddDataItem(dataItem);
    }

    public void ActivatePlayerOrBossUI(bool value)
    {
        _playerOrBossLevelUI.ActivateUI(value);
    }

    public void UpdateAchievementText(AchievementType type)
    {
        _achievementUI.UpdateText(type);
    }

    public void ActivateTextConsoleUI(string text)
    {
        _textConsoleUI.TextToWrite(text);
    }

    public void CloseTextConsoleUI(bool instantClose)
    {
        _textConsoleUI.CloseUI(instantClose);
    }

    public void TogglePauseUI()
    {
        _pauseMenu.TogglePause();
    }

    public void ActivateInstanceCompleteUI()
    {
        if (_gameManager.IsPaused())
            return;

        _instanceCompleteUI.ActivateUI(true);
    }

    public void ToogleInstanceCompleteUI()
    {
        if (_gameManager.IsPaused())
            return;
        
        _instanceCompleteUI.ToogleUI();
    }
}
