using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;


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

    [SerializeField] private List<Item> _availableItems = new List<Item>();
    private List<Transform> _spawnedArtefacts = new List<Transform>();
    [SerializeField] private List<ArtefactItem> _availableArtefacts;

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
        Transform spawnedItem = Instantiate(_gameAssets.ItemTemplate, position, Quaternion.identity, null);
        spawnedItem.GetComponent<PickupItem>().SetItem(item);

        switch (item)
        {
            case InventoryItem inventoryItem:
                spawnedItem.GetComponent<RotateObject>().RotationSpeed = 0.0f;
                spawnedItem.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().intensity = 0.0f;
                break;
            case NonInventoryItem nonInventoryItem:
                spawnedItem.GetComponent<SpriteRenderer>().color = (item as NonInventoryItem).Color;
                spawnedItem.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().color = (item as NonInventoryItem).Color;

                if (item is ThrowableItem)
                {
                    ThrowableItem throwable = item as ThrowableItem;
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
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }

        spawnedItem.name = item.ItemName;

        return spawnedItem;
    }

    private void generateItemPool()
    {
        float randomDropChance = Random.Range(0.0f, 100.0f);

        foreach (Item item in _availableItems)
            if (item != null)
                if (item.ChanceToDrop > randomDropChance)
                    _itemPool.Add(item);
    }

    private void clearItemPool()
    {
        _itemPool.Clear();
    }

    public void SpawnRandomItemAt(Vector3 position)
    {
        if (!shouldDropItem())
            return;

        generateItemPool();

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

    public void SpawnArtefacts(int artefactsRequired)
    {
        while (_spawnedArtefacts.Count < artefactsRequired)
        {
            int randomIndex = Random.Range(0, _availableArtefacts.Count);

            Vector2 randomPosition = 
                Vector2.right * Random.Range(0.0f, 20.0f) + Vector2.up * Random.Range(0.0f, 20.0f);

            Transform spawnedArtefact = SpawnItem(randomPosition, _availableArtefacts[randomIndex]);

            if (!_spawnedArtefacts.Contains(spawnedArtefact))
                _spawnedArtefacts.Add(spawnedArtefact);
        }
    }

    public List<ArtefactItem> GetAvailableArtefacts()
    {
        return _availableArtefacts;
    }
}
