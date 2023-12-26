using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemsUIMenu : MonoBehaviour
{
    private static ConsumableItemsUIMenu _instance;

    private ConsumableItemUI[] _consumableItemsUI = new ConsumableItemUI[3];

    private void Awake()
    {
        _instance = this;
        _consumableItemsUI = transform.Find("Container").GetComponentsInChildren<ConsumableItemUI>();
    }

    private void Start()
    {
        PlayerInventory.Instance.OnSuccessfulAdd += populateConsumableItemsUI;
        PlayerInventory.Instance.OnItemRemovedFromInventory += populateConsumableItemsUI;
        populateConsumableItemsUI();
    }

    private void OnDisable()
    {
        PlayerInventory.Instance.OnSuccessfulAdd -= populateConsumableItemsUI;
        PlayerInventory.Instance.OnItemRemovedFromInventory -= populateConsumableItemsUI;
    }

    private void populateConsumableItemsUI()
    {
        List<Item> items = PlayerInventory.GetItemsStatic();
        List<ConsumableItem> consumableItems = new List<ConsumableItem>();

        foreach (Item item in items)
        {
            if (item is ConsumableItem)
            {
                ConsumableItem consumable = item as ConsumableItem;
                if (!consumableItems.Contains(consumable))
                    consumableItems.Add(consumable);
            }
        }

        foreach (ConsumableItemUI itemUI in _consumableItemsUI)
        {
            ConsumableItem consumableItem = itemUI.GetItem() as ConsumableItem;
            if (consumableItems.Contains(consumableItem))
                itemUI.EnableUI();
            else
                itemUI.DisableUI();
        }
    }

    private List<Item> getConsumableItems()
    {
        List<Item> consumableItems = new List<Item>();
        foreach (ConsumableItemUI itemUI in _consumableItemsUI)
        {
            consumableItems.Add(itemUI.GetItem());
        }

        return consumableItems;
    }

    public static List<Item> GetConsumableItemsUIStatic()
    {
        return _instance.getConsumableItems();
    }
}
