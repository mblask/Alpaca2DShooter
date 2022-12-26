using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class RandomWeaponSpawnObject : MonoBehaviour
{
    private ItemSpawner _itemSpawner;

    [SerializeField] private bool _spawnRandomly;
    [SerializeField] private WeaponItem _specificWeapon;

    private void Start()
    {
        spawnItem();
    }

    private void spawnItem()
    {
        _itemSpawner = ItemSpawner.Instance;

        if (_spawnRandomly)
        {
            if (Utilities.ChanceFunc(50))
                Destroy(gameObject);
        }

        if (_specificWeapon == null)
        {
            List<WeaponItem> weapons = GameAssets.Instance.AvailableWeaponsList;

            Transform weaponObject = _itemSpawner?.SpawnItem(transform.position, weapons.GetRandomElement());
            weaponObject.transform.parent = transform.parent;
        }
        else
        {
            _itemSpawner?.SpawnItem(transform.position, _specificWeapon);
        }


        Destroy(gameObject);
    }
}
