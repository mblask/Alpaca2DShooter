using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System;

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

    [SerializeField] private List<Item> _availableItems;
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

        NPCStats.OnEnemyDeath += EnemyStats_OnEnemyDeath;
    }

    private void OnDisable()
    {
        NPCStats.OnEnemyDeath -= EnemyStats_OnEnemyDeath;
    }

    private void EnemyStats_OnEnemyDeath(NPCStats enemyStats)
    {
        SpawnRandomItemAt(enemyStats.transform.position);
    }

    public Transform SpawnItem(Vector3 position, Item item)
    {
        Transform spawnedItem = Instantiate(_gameAssets.ItemTemplate, position, Quaternion.identity, null);
        spawnedItem.GetComponent<PickupItem>().SetItem(item);

        if (item is InventoryItem)
        {
            spawnedItem.GetComponent<RotateObject>().RotationSpeed = 0.0f;
            spawnedItem.GetComponentInChildren<Light2D>().intensity = 0.0f;
        }

        if (item is InstantaneousItem)
        {
            spawnedItem.GetComponent<SpriteRenderer>().color = (item as InstantaneousItem).Color;
            spawnedItem.GetComponentInChildren<Light2D>().color = (item as InstantaneousItem).Color;
        }

        if (item is ArtefactItem)
        {
            spawnedItem.GetComponent<SpriteRenderer>().color = (item as ArtefactItem).Color;
            spawnedItem.GetComponentInChildren<Light2D>().color = (item as ArtefactItem).Color;
        }

        spawnedItem.name = item.ItemName;

        return spawnedItem;
    }

    private void generateItemPool()
    {
        float randomDropChance = UnityEngine.Random.Range(0.0f, 100.0f);

        foreach (Item item in _availableItems)
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
            Item randomItem = _itemPool[UnityEngine.Random.Range(0, _itemPool.Count)];
            SpawnItem(position, randomItem);

        }

        clearItemPool();
    }

    private bool shouldDropItem()
    {
        int randomNumber = UnityEngine.Random.Range(0, 100);

        return randomNumber <= EnemyDropRate;
    }

    public void SpawnArtefacts(int artefactsRequired)
    {
        while (_spawnedArtefacts.Count < artefactsRequired)
        {
            int randomIndex = UnityEngine.Random.Range(0, _availableArtefacts.Count);

            Vector2 randomPosition = Vector2.right * UnityEngine.Random.Range(0.0f, 20.0f) + Vector2.up * UnityEngine.Random.Range(0.0f, 20.0f);

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
