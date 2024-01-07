using UnityEngine;
using AlpacaMyGames;

public class RandomWeaponSpawnObject : MonoBehaviour
{
    [SerializeField] private bool _spawnRandomly;
    [SerializeField] private WeaponItem _specificWeapon;

    private void Start()
    {
        spawnItem();
    }

    private void spawnItem()
    {
        if (_spawnRandomly && Utilities.ChanceFunc(50))
            Destroy(gameObject);

        ItemSpawner itemSpawner = ItemSpawner.Instance;
        GameAssets gameAssets = GameAssets.Instance;

        if (_specificWeapon == null)
        {
            Item randomWeapon = Utilities.ChanceFunc(50) ?
                gameAssets.WeaponsList.GetRandomElement() : gameAssets.ThrowablesList.GetRandomElement();

            Transform weaponObject = itemSpawner?.SpawnItem(transform.position, randomWeapon);
            weaponObject.transform.parent = transform.parent;
        }
        else
        {
            itemSpawner?.SpawnItem(transform.position, _specificWeapon);
        }

        Destroy(gameObject);
    }
}
