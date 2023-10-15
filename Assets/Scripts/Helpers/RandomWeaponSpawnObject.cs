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

    private void spawnRandomly()
    {
        if (!_spawnRandomly)
            return;

        if (Utilities.ChanceFunc(50))
            Destroy(gameObject);
    }

    private void spawnItem()
    {
        _itemSpawner = ItemSpawner.Instance;

        spawnRandomly();

        if (_specificWeapon == null)
        {
            List<WeaponItem> weapons = GameAssets.Instance.WeaponsList;

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
