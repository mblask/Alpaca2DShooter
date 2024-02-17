using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class ItemSpawner : MonoBehaviour
{
    private static ItemSpawner _instance;
    public static ItemSpawner Instance
    {
        get
        {
            return _instance;
        }
    }

    [Range(0.0f, 100.0f)] public float EnemyDropRate = 25.0f;

    [Header("Droppable items")]
    [SerializeField] private List<Item> _availableItems = new List<Item>();
    [SerializeField] private List<Item> _bossDrops = new List<Item>();
    private List<Item> _itemPool = new List<Item>();

    private GameAssets _gameAssets;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
    }

    public Transform SpawnItem(Vector3 position, Item item)
    {
        Transform itemToSpawn = _gameAssets.ItemTemplate;
        if (item.ItemPrefab != null)
            itemToSpawn = item.ItemPrefab.transform;

        Transform spawnedItem = Instantiate(itemToSpawn, position, Quaternion.identity, null);
        PickupItem pickup = spawnedItem.GetComponent<PickupItem>();
        pickup.SetItem(item);

        Light2D light = spawnedItem.GetComponentInChildren<Light2D>();
        if (light != null)
            light.color = item.Color;

        switch (item)
        {
            case NonInventoryItem:
                spawnedItem.GetComponent<SpriteRenderer>().color = (item as NonInventoryItem).Color;

                ThrowableItem throwable = item as ThrowableItem;
                if (throwable == null)
                    break;

                if (spawnedItem.GetComponent<Rigidbody2D>() == null)
                    spawnedItem.gameObject.AddComponent<Rigidbody2D>();

                switch (throwable.Type)
                {
                    case ThrowableWeaponType.Mine:
                        spawnedItem.gameObject.AddComponent<Mine>();
                        spawnedItem.GetComponent<Mine>().SetItem(throwable);
                        break;
                    case ThrowableWeaponType.Grenade:
                        spawnedItem.gameObject.AddComponent<Grenade>();
                        spawnedItem.GetComponent<Grenade>().SetItem(throwable);
                        break;
                    case ThrowableWeaponType.FlashGrenade:
                        spawnedItem.gameObject.AddComponent<FlashGrenade>();
                        spawnedItem.GetComponent<FlashGrenade>().SetItem(throwable);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        spawnedItem.name = item.ItemName;

        return spawnedItem;
    }

    private void generateItemPool(bool bossDrop = false)
    {
        float randomDropChance = Random.Range(0.0f, 100.0f);

        foreach (Item item in _availableItems)
            if (item != null)
                if (item.ChanceToDrop > randomDropChance)
                    _itemPool.Add(item);

        if (!bossDrop)
            return;

        foreach (Item item in _bossDrops)
            if (item != null)
                if (item.ChanceToDrop > randomDropChance)
                    _itemPool.Add(item);
    }

    private void clearItemPool()
    {
        _itemPool.Clear();
    }

    public void SpawnRandomItemAt(Vector3 position, bool bossDrop = false)
    {
        if (!shouldDropItem() || bossDrop)
            return;

        generateItemPool(bossDrop);

        if (_itemPool.Count > 0)
        {
            Item randomItem = _itemPool.GetRandomElement();
            SpawnItem(position, randomItem);

        }

        clearItemPool();
    }

    private bool shouldDropItem()
    {
        int randomNumber = Random.Range(0, 100);

        return randomNumber <= EnemyDropRate;
    }
}
