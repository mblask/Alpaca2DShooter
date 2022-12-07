using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWeaponSpawnObject : MonoBehaviour
{
    private ItemSpawner _itemSpawner;

    [SerializeField] private bool _spawnRandomly;

    private void Start()
    {
        _itemSpawner = ItemSpawner.Instance;

        if (_spawnRandomly)
        {
            if (0 == Random.Range(0, 2))
                Destroy(gameObject);
        }

        List<WeaponItem> weapons = GameAssets.Instance.AvailableWeaponsList;

        Transform weaponObject = _itemSpawner.SpawnItem(transform.position, weapons[Random.Range(0, weapons.Count)]);
        weaponObject.transform.parent = transform.parent;

        Destroy(gameObject);
    }
}
