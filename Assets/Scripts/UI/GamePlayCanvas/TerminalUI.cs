using System.Collections.Generic;
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
    private Transform _inventoryItemSelector;
    private Transform _itemButtonsContainer;

    private AlpacaButtonUI _turnOffTrapsButton;
    private AlpacaButtonUI _showExitButton;
    private AlpacaButtonUI _switchAllegianceButton;
    private AlpacaButtonUI _insertDriveButton;
    private AlpacaButtonUI _removeDriveButton;
    private AlpacaButtonUI _runDriveButton;

    private List<DataItem> _dataItems = new List<DataItem>();

    private Terminal _terminal; 
    private GameAssets _gameAssets;

    private Camera _mainCamera;

    private void Awake()
    {
        _instance = this;
        _container = transform.Find("Container");
        _inventoryItemSelector = _container.Find("InventoryItemSelectorUI");
        _itemButtonsContainer = _inventoryItemSelector.Find("ScrollView").Find("Viewport").Find("Content");

        _turnOffTrapsButton = _container.Find("ButtonContainer").Find("TurnOffTrapsButton").GetComponent<AlpacaButtonUI>();
        _showExitButton = _container.Find("ButtonContainer").Find("ShowExitButton").GetComponent<AlpacaButtonUI>();
        _switchAllegianceButton = _container.Find("ButtonContainer").Find("SwitchAllegianceButton").GetComponent<AlpacaButtonUI>();
        _insertDriveButton = _container.Find("ButtonContainer").Find("InsertDataCarrierButton").GetComponent<AlpacaButtonUI>();
        _removeDriveButton = _container.Find("ButtonContainer").Find("RemoveDataCarrierButton").GetComponent<AlpacaButtonUI>();
        _runDriveButton = _container.Find("ButtonContainer").Find("RunDriveButton").GetComponent<AlpacaButtonUI>();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _gameAssets = GameAssets.Instance;

        _turnOffTrapsButton.onLeftClick = () => { _terminal?.TurnOnOff(); };
        _showExitButton.onLeftClick = () => { _terminal?.ShowExitPortal(); };
        _switchAllegianceButton.onLeftClick = () => { _terminal?.Hack(); };
        _insertDriveButton.onLeftClick = () => { showExistingDataItems(); };
        _removeDriveButton.onLeftClick = () => { removeDataItem(); };
        _runDriveButton.onLeftClick = () => { _terminal?.RunDataCarrier(); };
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

    public void AddDataItemsUI(List<DataItem> dataItems)
    {
        bool itemsExist = dataItems != null && dataItems.Count > 0;
        _insertDriveButton.gameObject.SetActive(itemsExist);
        _removeDriveButton.gameObject.SetActive(!itemsExist);

        if (!itemsExist)
            return;

        _dataItems = dataItems;
    }

    private void showExistingDataItems()
    {
        _inventoryItemSelector.gameObject.SetActive(true);

        foreach (Transform child in _itemButtonsContainer)
        {
            Destroy(child.gameObject);
        }
        
        foreach (DataItem item in _dataItems)
        {
            ItemSelectorButton button = 
                Instantiate(_gameAssets.ItemSelectorButton, _itemButtonsContainer)
                .GetComponent<ItemSelectorButton>();
            button.SetItem(item);
        }
    }

    public void SetTerminal(Terminal terminal)
    {
        _terminal = terminal;
        ActivateUI(true);
    }

    private void removeDataItem()
    {
        _terminal?.RemoveDataItem();
        _insertDriveButton.gameObject.SetActive(true);
        _removeDriveButton.gameObject.SetActive(false);
        _runDriveButton.gameObject.SetActive(false);
    }

    public void AddDataItem(DataItem dataItem)
    {
        _terminal?.InsertDataItem(dataItem);
        _insertDriveButton.gameObject.SetActive(false);
        _removeDriveButton.gameObject.SetActive(true);
        _runDriveButton.gameObject.SetActive(true);
        _inventoryItemSelector.gameObject.SetActive(false);
    }
}
