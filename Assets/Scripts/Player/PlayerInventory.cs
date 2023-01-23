using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour, ICrafting
{
    public Action OnSuccessfulAdd;
    public Action OnItemRemovedFromInventory;
    public Action OnToggleInventoryUI;
    public Action OnToggleCraftingUI;

    private static PlayerInventory _instance;
    public static PlayerInventory Instance
    {
        get
        {
            return _instance;
        }
    }

    private const int MAX_NUMBER_OF_ITEMS = 12;
    [SerializeField] private List<Item> _items = new List<Item>();

    private bool _craftingPossible = false;

    private ItemSpawner _itemSpawner;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _itemSpawner = ItemSpawner.Instance;
    }

    private void Update()
    {
        toggleInventoryUI();
        toggleCraftingUI();
    }

    private void toggleInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.I))
            OnToggleInventoryUI?.Invoke();
    }

    private void toggleCraftingUI()
    {
        if (!_craftingPossible)
            return;

        if (Input.GetKeyDown(KeyCode.C))
            OnToggleCraftingUI?.Invoke();
    }

    public static List<Item> GetItemsStatic()
    {
        return _instance.getItems();
    }

    public static void UseConsumableItemStatic(Item item)
    {
        _instance?.useConsumableItem(item);
    }

    private void useConsumableItem(Item item)
    {
        int itemIndex = _items.IndexOf(item);

        if (itemIndex >= 0)
        {
            ConsumableItem consumable = _items[itemIndex] as ConsumableItem;
            consumable.UseItem();
        }
    }

    public static bool AddToInventoryStatic(Item item)
    {
        return _instance.addToInventory(item);
    }

    private bool addToInventory(Item item)
    {
        if (_items.Count < MAX_NUMBER_OF_ITEMS)
        {
            _items.Add(item);
            OnSuccessfulAdd?.Invoke();

            return true;
        }

        return false;
    }

    public static bool ThrowFromInventoryStatic(Item item)
    {
        return _instance.throwFromInventory(item);
    }

    private bool throwFromInventory(Item item)
    {
        if (item == null)
            return false;

        if (_items.Remove(item))
        {
            float randomAngleRad = UnityEngine.Random.Range(0.0f, 6.28f);
            Vector3 position = transform.position + 2.0f * (Vector3.up * Mathf.Sin(randomAngleRad) + Vector3.right * Mathf.Cos(randomAngleRad));
            _itemSpawner.SpawnItem(position, item);

            OnItemRemovedFromInventory?.Invoke();

            return true;
        }

        return false;
    }

    public static bool DeleteItemFromInventoryStatic(Item item)
    {
        return _instance.deleteItemFromInventory(item);
    }

    private bool deleteItemFromInventory(Item item)
    {
        if (item == null)
            return false;

        if (_items.Remove(item))
        {
            OnItemRemovedFromInventory?.Invoke();
            return true;
        }
        else
            return false;
    }

    private List<Item> getItems()
    {
        return _items;
    }

    public void EnableCrafting(bool value)
    {
        _craftingPossible = value;
    }
}