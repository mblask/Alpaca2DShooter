using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class LevelsManager : MonoBehaviour
{
    private static LevelsManager _instance;

    public static LevelsManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] private int _levelsToPass = 2;

    private List<LevelObject> _levels;
    private List<LevelObject> _usedLevels = new List<LevelObject>();

    private bool _playerSpawned = false;

    private GameManager _gameManager;

    private void Awake()
    {
        _instance = this;
        _levels = new List<LevelObject>(GetComponentsInChildren<LevelObject>());
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
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
        if (_levelsToPass == _usedLevels.Count)
        {
            Debug.Log("Level finished");
            Debug.Log("Victory");
            _gameManager.SetGameRunning(false);
            return null;
        }

        LevelObject level = getRandomLevelsExcluding(_levels, _usedLevels).GetRandomElement();

        if (level == null)
        {
            Debug.Log("Level finished");
            Debug.Log("Victory");
            _gameManager.SetGameRunning(false);
            return null;
        }

        level.SetupLevel(true);
        _usedLevels.Add(level);

        return level;
    }

    private List<LevelObject> getRandomLevelsExcluding(List<LevelObject> levels, List<LevelObject> excludedLevels)
    {
        if (levels.Count == excludedLevels.Count)
            return null;

        List<LevelObject> leftoverLevels = levels.FindAll(level => !excludedLevels.Contains(level));

        return leftoverLevels;
    }
}
