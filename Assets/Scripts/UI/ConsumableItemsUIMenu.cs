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
        PopulateConsumableItemsUI();
    }

    public void PopulateConsumableItemsUI()
    {
        List<ConsumableItem> consumableItems = PlayerInventory.GetConsumables();
        foreach (ConsumableItemUI itemUI in _consumableItemsUI)
        {
            ConsumableItem consumableItem = itemUI.GetItem();
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
