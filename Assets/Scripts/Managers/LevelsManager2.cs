using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class LevelsManager2 : MonoBehaviour
{
    private static LevelsManager2 _instance;

    private List<LevelObject> _levels;
    private List<LevelObject> _usedLevels = new List<LevelObject>();

    private bool _playerSpawned = false;

    private void Awake()
    {
        _instance = this;
        _levels = new List<LevelObject>(GetComponentsInChildren<LevelObject>());
    }

    private void Start()
    {
        spawnPlayerRandomly();
    }

    private void spawnPlayerRandomly()
    {
        LevelObject randomLevel = _levels.GetRandomElement();

        if (!randomLevel.ContainsPlayerSpawnPoints())
            return;

        randomLevel.SpawnPlayer();
        randomLevel.SetupLevel(false);

        _usedLevels.Add(randomLevel);
        _playerSpawned = true;
    }

    public static LevelObject SetupRandomNewLevelStatic()
    {
        return _instance.setupRandomNewLevel();
    }

    private LevelObject setupRandomNewLevel()
    {
        LevelObject level = getRandomLevelsExcluding(_levels, _usedLevels).GetRandomElement();

        if (level != null)
        {
            level.SetupLevel(true);
            _usedLevels.Add(level);
        }

        return level;
    }

    private List<LevelObject> getRandomLevelsExcluding(List<LevelObject> levels, List<LevelObject> excludedLevels)
    {
        if (levels.Count == excludedLevels.Count)
            return null;

        List<LevelObject> leftoverLevels = levels.FindAll(level => !excludedLevels.Contains(level));

        Debug.Log(leftoverLevels.Count);

        foreach (LevelObject level in leftoverLevels)
        {
            Debug.Log(level.gameObject.name);
        }

        return leftoverLevels;
    }
}
