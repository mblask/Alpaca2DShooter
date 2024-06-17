using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, ICrafting
{
    private static PlayerInventory _instance;
    public static PlayerInventory Instance
    {
        get
        {
            return _instance;
        }
    }

    private const int MAX_NUMBER_OF_COLLECTIBLES = 5;
    [SerializeField] private List<Item> _collectibles = new List<Item>();
    private const int MAX_NUMBER_OF_ITEMS = 20;
    public int MaxNumberOfItems => MAX_NUMBER_OF_ITEMS;
    [SerializeField] private List<Item> _items = new List<Item>();

    private bool _craftingPossible = false;
    public bool CraftingPossible => _craftingPossible;

    private ItemSpawner _itemSpawner;
    private AchievementManager _achievementManager;
    private CollectiblesManager _collectiblesManager;
    private GamePlayCanvas _canvas;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _itemSpawner = ItemSpawner.Instance;
        _achievementManager = AchievementManager.Instance;
        _collectiblesManager = CollectiblesManager.Instance;
        _canvas = GamePlayCanvas.Instance;
    }

    private bool addCollectible(Item item)
    {
        if (_collectibles.Count < MAX_NUMBER_OF_COLLECTIBLES)
        {
            _collectibles.Add(item);
            _collectiblesManager.UnlockCollectible(item);
            return true;
        }

        return false;
    }

    public List<Item> GetCollectibles()
    {
        return _collectibles;
    }

    public bool RemoveCollectible(Item item)
    {
        return _collectibles.Remove(item);
    }

    public static List<Item> GetItemsStatic()
    {
        return _instance.getItems();
    }

    public static List<ConsumableItem> GetConsumables()
    {
        List<ConsumableItem> consumables = new List<ConsumableItem>();
        foreach (Item item in _instance.getItems())
        {
            ConsumableItem consumable = item as ConsumableItem;
            if (consumable != null)
                consumables.Add(consumable);
        }

        return consumables;
    }

    public List<Item> GetItems()
    {
        return _items;
    }

    public void UseConsumable(ConsumableType type)
    {
        ConsumableItem consumable = GameAssets.Instance.GetConsumableByType(type);
        if (!_items.Contains(consumable))
            return;

        if (consumable.UseItem())
            _achievementManager.CheckOnItemUsed(consumable);
    }

    public static bool AddToInventoryStatic(Item item)
    {
        return _instance.AddToInventory(item);
    }

    public bool AddToInventory(Item item)
    {
        if (item.IsCollectible)
            return addCollectible(item);

        if (_items.Count < MAX_NUMBER_OF_ITEMS)
        {
            _items.Add(item);
            _canvas.UpdateItemContainer();
            _canvas.PopulateConsumableItemsUI();

            return true;
        }

        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Invenory full", Color.white);
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

            _canvas.UpdateItemContainer();
            _canvas.PopulateConsumableItemsUI();

            return true;
        }

        return false;
    }

    public static bool DeleteItemFromInventoryStatic(Item item)
    {
        return _instance.DeleteItemFromInventory(item);
    }

    public bool DeleteItemFromInventory(Item item)
    {
        if (item == null)
            return false;

        if (_items.Remove(item))
        {
            _canvas.UpdateItemContainer();
            _canvas.PopulateConsumableItemsUI();
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
