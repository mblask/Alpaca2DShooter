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

    private const int MAX_NUMBER_OF_ITEMS = 12;
    [SerializeField] private List<Item> _items = new List<Item>();

    private bool _craftingPossible = false;

    private ItemSpawner _itemSpawner;
    private AchievementManager _achievementManager;
    private GamePlayCanvas _uiCanvas;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _itemSpawner = ItemSpawner.Instance;
        _achievementManager = AchievementManager.Instance;
        _uiCanvas = GamePlayCanvas.Instance;
    }

    private void Update()
    {
        toggleInventoryUI();
        toggleCraftingUI();
    }

    private void toggleInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.I))
            _uiCanvas.ShowInventory();
    }

    private void toggleCraftingUI()
    {
        if (!_craftingPossible)
            return;

        if (Input.GetKeyDown(KeyCode.C))
            _uiCanvas.ShowCraftingUI();
    }

    public static List<Item> GetItemsStatic()
    {
        return _instance.getItems();
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
        return _instance.addToInventory(item);
    }

    private bool addToInventory(Item item)
    {
        if (_items.Count < MAX_NUMBER_OF_ITEMS)
        {
            _items.Add(item);
            _uiCanvas.UpdateItemContainer();
            _uiCanvas.PopulateConsumableItemsUI();

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

            _uiCanvas.UpdateItemContainer();
            _uiCanvas.PopulateConsumableItemsUI();

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
            _uiCanvas.UpdateItemContainer();
            _uiCanvas.PopulateConsumableItemsUI();
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
