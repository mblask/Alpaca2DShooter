using AlpacaMyGames;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    [SerializeField]
    private bool _spawnRandomly = false;
    
    [SerializeField]
    private Item _item;

    private void Start()
    {
        if (_spawnRandomly && Utilities.ChanceFunc(50))
            Destroy(gameObject);

        ItemSpawner.Instance.SpawnItem(transform.position, _item);
    }
}
