using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour, IUiObject
{
    private Transform _container;
    private Transform _itemSlotContainer;
    private int _maxNumberOfItems;

    private PlayerInventory _playerInventory;
    private GameAssets _gameAssets;
    private GamePlayCanvas _canvas;

    private void Awake()
    {
        _container = transform.Find("Container");
        _itemSlotContainer = _container.Find("ItemSlotContainer");
    }

    private void Start()
    {
        _playerInventory = PlayerInventory.Instance;
        _maxNumberOfItems = _playerInventory.MaxNumberOfItems;
        _gameAssets = GameAssets.Instance;
        _canvas = GamePlayCanvas.Instance;
        UpdateItemContainer();
    }

    public void ShowInventory()
    {
        bool isActive = !_container.gameObject.activeSelf;
        _container.gameObject.SetActive(isActive);

        if (isActive)
            GamePlayCanvas.AddOpenUiStatic(this);

        UpdateItemContainer();
    }

    public void HideUI()
    {
        GamePlayCanvas.RemoveOpenUiStatic(this);
        _container.gameObject.SetActive(false);
    }

    public void UpdateItemContainer()
    {
        if (!_container.gameObject.activeSelf)
        {
            _canvas.RemoveItemTooltip();
            return;
        }

        if (_itemSlotContainer.childCount > 0)
        {
            foreach (Transform itemSlot in _itemSlotContainer)
            {
                Destroy(itemSlot.gameObject);
            }
        }

        List<Item> inventoryItems = _playerInventory.GetItems();

        for (int i = 0; i < _maxNumberOfItems; i++)
        {
            Transform itemSlotTransform = Instantiate(_gameAssets.ItemSlotUIPrefab, _itemSlotContainer);

            if (i < inventoryItems.Count)
            {
                ItemSlotUI itemSlot = itemSlotTransform.GetComponent<ItemSlotUI>();
                itemSlot.AddItemToSlot(inventoryItems[i]);
            }
        }
    }
}
