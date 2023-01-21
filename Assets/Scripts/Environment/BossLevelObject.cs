using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelObject : LevelObject
{
    private Transform _NPCBossTransform;

    protected override void spawnEnemies()
    {
        if (_enemySpawnPoints == null)
            return;

        if (_enemySpawnPoints.Count == 0)
            return;

        foreach (SpawnPoint spawnPoint in _enemySpawnPoints)
        {
            _NPCBossTransform = Instantiate(GameAssets.Instance.NPCBossPrefab, spawnPoint.Location, Quaternion.identity, _npcContainer);
        }

        _enemySpawnPoints.ForEach(spawnPoint => Destroy(spawnPoint.gameObject));
        _playerSpawnPoints.ForEach(spawnPoint => Destroy(spawnPoint.gameObject));

        _exitPortal.SetBossPortal(_NPCBossTransform);
    }
}
