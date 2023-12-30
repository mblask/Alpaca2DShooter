using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private Transform _container;
    private Transform _itemSlotContainer;
    private const int MAX_NUMBER_OF_ITEMS = 12;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _container = transform.Find("Container");
        _itemSlotContainer = _container.Find("ItemSlotContainer");
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        UpdateItemContainer();

        PlayerInventory.Instance.OnToggleInventoryUI += showInventory;
        PlayerInventory.Instance.OnSuccessfulAdd += UpdateItemContainer;
        PlayerInventory.Instance.OnItemRemovedFromInventory += UpdateItemContainer;
    }

    private void OnDisable()
    {
        PlayerInventory.Instance.OnToggleInventoryUI -= showInventory;
        PlayerInventory.Instance.OnSuccessfulAdd -= UpdateItemContainer;
        PlayerInventory.Instance.OnItemRemovedFromInventory -= UpdateItemContainer;
    }

    private void showInventory()
    {
        bool isActive = !_container.gameObject.activeSelf;

        _container.gameObject.SetActive(isActive);
        
        UpdateItemContainer();
    }

    public void UpdateItemContainer()
    {
        if (!_container.gameObject.activeSelf)
        {
            ItemTooltip.RemoveTooltipStatic();
            return;
        }

        if (_itemSlotContainer.childCount > 0)
        {
            foreach (Transform itemSlot in _itemSlotContainer)
            {
                Destroy(itemSlot.gameObject);
            }
        }

        List<Item> inventoryItems = PlayerInventory.GetItemsStatic();

        for (int i = 0; i < MAX_NUMBER_OF_ITEMS; i++)
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
