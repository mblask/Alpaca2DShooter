using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class SpawnObjectRandomly : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] private int _spawnChance = 50;

    void Start()
    {
        if (!Utilities.ChanceFunc(_spawnChance))
            Destroy(gameObject);
    }
}
