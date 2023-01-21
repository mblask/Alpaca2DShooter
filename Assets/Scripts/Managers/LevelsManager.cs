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

    private Transform _playerTransform;
    private PlayerLevelObject _playerLevelObject;
    private Transform _playersLevelSpawnPoint;
    private bool _playerSpawned = false;

    private GameManager _gameManager;

    private bool _levelsFinished = false;
    public static bool levelsFinished
    {
        get
        {
            return _instance._levelsFinished;
        }
    }

    private void Awake()
    {
        _instance = this;
        _levels = new List<LevelObject>(GetComponentsInChildren<LevelObject>());
        _playerLevelObject = transform.Find("PlayersLevel").GetComponent<PlayerLevelObject>();
        _playersLevelSpawnPoint = transform.Find("PlayersLevel").Find("Locations").Find("PlayerSpawnPoints").Find("PlayerSpawnPoint");
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;

        if (!spawnPlayerRandomly())
            spawnPlayerInPlayersLevel();
    }

    private void spawnPlayerInPlayersLevel()
    {
        if (_playerSpawned)
            return;

        if (_playersLevelSpawnPoint == null)
            return;

        _playerLevelObject.SetupLevel(false);
        Instantiate(GameAssets.Instance.Player, _playersLevelSpawnPoint.position, Quaternion.identity, null);
    }

    private bool spawnPlayerRandomly()
    {
        if (_levels.Count == 0)
            return false;

        LevelObject randomLevel = _levels.GetRandomElement();

        if (!randomLevel.ContainsPlayerSpawnPoints())
            return false;

        _playerTransform = randomLevel.SpawnPlayer();
        randomLevel.SetupLevel(false);

        _usedLevels.Add(randomLevel);
        _playerSpawned = true;

        return true;
    }

    public static LevelObject SetupRandomNewLevelStatic()
    {
        return _instance.setupRandomNewLevel();
    }

    private bool passedRequiredLevels()
    {
        if (_levelsToPass == _usedLevels.Count)
        {
            finishLevel();
            return true;
        }

        return false;
    }

    private void finishLevel()
    {
        Debug.Log("Level finished");
        Debug.Log("Victory");
        _levelsFinished = true;
        _gameManager.SetGameRunning(false);
    }

    private void transferPlayerToPlayersLevel()
    {
        Debug.Log("Move player to the player's level!");
        _playerTransform.position = _playersLevelSpawnPoint.position;
    }

    private LevelObject setupRandomNewLevel()
    {
        if (passedRequiredLevels())
        {
            transferPlayerToPlayersLevel();
            return null;
        }

        LevelObject level = getRandomLevelsExcluding(_levels, _usedLevels).GetRandomElement();

        if (level == null)
        {
            finishLevel();
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
