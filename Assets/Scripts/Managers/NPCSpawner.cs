using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class NPCSpawner : MonoBehaviour
{
    private LevelsManager _levelManager;
    private GameAssets _gameAssets;

    private Transform _enemySpawnParent;
    private List<Transform> _enemies;

    private void Start()
    {
        _levelManager = LevelsManager.Instance;
        _gameAssets = GameAssets.Instance;

        spawnNPCs();
    }

    private void spawnNPCs()
    {
        if (_levelManager == null)
            return;

        List<SpawnPoint> spawnPoints = _levelManager.GetEnemySpawnPoints();

        if (_enemySpawnParent == null)
            _enemySpawnParent = new GameObject("EnemyContainer").transform;

        if (_enemies == null)
            _enemies = new List<Transform>();

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (!Utilities.ChanceFunc(75))
                continue;

            if (Utilities.ChanceFunc(25))
            {
                spawnNumberOfNPCs(spawnPoint.Location, 2, _enemySpawnParent);
                continue;
            }

            _enemies.Add(spawnNPC(spawnPoint.Location, _enemySpawnParent));
        }
    }

    private void spawnNumberOfNPCs(Vector2 position, int numberOfEnemies, Transform parent = null)
    {
        if (numberOfEnemies == 0)
            return;

        if (numberOfEnemies == 1)
            spawnNPC(position, parent);
        else
        {
            float angle = 0.0f;
            float deltaAngle = 360.0f / numberOfEnemies;
            float distanceFromOrigin = 1.0f;
            for (int i = 0; i < numberOfEnemies; i++)
            {
                Vector2 spawnPosition = position + Utilities.GetVectorFromAngle(angle) * distanceFromOrigin;
                _enemies.Add(spawnNPC(spawnPosition, parent));
                angle += deltaAngle;
            }
        }
    }

    private Transform spawnNPC(Vector2 position, Transform parent = null)
    {
        return Instantiate(_gameAssets.NPCPrefab, position, Quaternion.identity, parent);
    }
}
