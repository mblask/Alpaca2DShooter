using UnityEngine;
using AlpacaMyGames;

public class SpawnObjectRandomly : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] private int _spawnChance = 50;

    private void Awake()
    {
        if (!Utilities.ChanceFunc(_spawnChance))
            Destroy(gameObject);
    }
}
