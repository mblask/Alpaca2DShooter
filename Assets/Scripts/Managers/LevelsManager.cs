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
    [SerializeField] private int _groupsToPass = 2;

    private List<LevelObject> _levelsList;
    private List<LevelObject> _playedLevelsList = new List<LevelObject>();
    private List<LevelObject> _bossLevelsList;
    private LevelObject _playerLevel;
    private int _numberOfGroupsPlayed = 0;
    private int _numberOfSingleGroupLevelsUsed = 0;

    private Transform _playerTransform;
    private Transform _playersLevelSpawnPoint;
    private bool _playerSpawned = false;

    private LevelObject _currentLevel;

    private GameManager _gameManager;

    private void Awake()
    {
        _instance = this;
        _levelsList = Utilities.GetListOfObjectsFromContainer<LevelObject>(transform, "Regular");
        _bossLevelsList = Utilities.GetListOfObjectsFromContainer<LevelObject>(transform, "Boss");
        _playerLevel = transform.Find("PlayersLevel").GetComponent<LevelObject>();
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

        if (_playerLevel == null)
            return;

        _playerLevel.SetupLevel(true);
        _currentLevel = _playerLevel;
        _playerTransform = Instantiate(GameAssets.Instance.Player, _playerLevel.GetSpawnPortalPosition(), Quaternion.identity, null);
    }

    private bool spawnPlayerRandomly()
    {
        if (_levelsList.Count == 0)
            return false;

        if (_playerSpawned)
            return true;

        LevelObject randomLevel = _levelsList.GetRandomElement();

        if (!randomLevel.ContainsPlayerSpawnPoints())
            return false;

        _playerTransform = randomLevel.SpawnPlayer();
        randomLevel.SetupLevel(false);

        _currentLevel = randomLevel;
        _playedLevelsList.Add(randomLevel);
        
        _playerSpawned = true;
        
        _numberOfSingleGroupLevelsUsed++;
        _numberOfGroupsPlayed++;

        return true;
    }

    private enum completionState
    {
        exitingPlayerLevel,
        groupNotCompleted,
        groupCompleted,
        gameCompleted,
    }

    private completionState checkGroupCompleted()
    {
        if (_currentLevel.GetLevelType().Equals(LevelType.Boss))
            return completionState.groupCompleted;

        if (_currentLevel.Equals(_playerLevel))
            return completionState.exitingPlayerLevel;

        if (_numberOfSingleGroupLevelsUsed != _levelsToPass)
            return completionState.groupNotCompleted;

        if (_numberOfGroupsPlayed == _groupsToPass)
            return completionState.gameCompleted;

        return completionState.groupCompleted;
    }

    public static void CheckCompletionStateStatic()
    {
        _instance.checkCompletionState();
    }

    private void checkCompletionState()
    {
        switch (checkGroupCompleted())
        {
            case completionState.exitingPlayerLevel:
                exitingPlayersLevel();
                break;

            case completionState.gameCompleted:
                gameCompleted();
                break;

            case completionState.groupCompleted:
                groupCompleted();
                break;

            case completionState.groupNotCompleted:
                groupNotCompleted();
                break;

            default:
                break;
        }
    }

    private enum onPlayerLevelExit
    {
        boss,
        repeat,
    }

    private void exitingPlayersLevel()
    {
        Debug.Log("Exiting player level!");

        onPlayerLevelExit onPlayerLevelExit = onPlayerLevelExit.boss;

        switch (onPlayerLevelExit)
        {
            case onPlayerLevelExit.boss:
                Debug.Log("Go to boss level!");
                transferPlayerToBossLevel();
                break;
            case onPlayerLevelExit.repeat:
                Debug.Log("Go repeat levels!");
                break;
            default:
                break;
        }
    }

    private void transferPlayerToBossLevel()
    {
        LevelObject level = _bossLevelsList.GetRandomElement();

        if (level != null)
        {
            if (!level.IsReady())
                level.SetupLevel();
            
            _currentLevel = level;
            _playerTransform.position = level.GetSpawnPortalPosition();
        }
    }

    private void groupNotCompleted()
    {
        Debug.Log(completionState.groupNotCompleted);
        Debug.Log("Transfer player to another level!");

        transferPlayerToAnotherLevel();
    }

    private void transferPlayerToAnotherLevel()
    {
        LevelObject level = getRandomLevelsExcluding(_levelsList, _playedLevelsList).GetRandomElement();

        if (level != null)
        {
            level.SetupLevel(true);
            _playedLevelsList.Add(level);
            _numberOfSingleGroupLevelsUsed++;

            _playerTransform.position = level.GetSpawnPortalPosition();

            _currentLevel.ClearLevel();
            _currentLevel = level;
            Debug.Log("Enter: " + _currentLevel.name);
        }
    }

    private void groupCompleted()
    {
        Debug.Log(completionState.groupCompleted);
        if (_currentLevel.GetLevelType().Equals(LevelType.Boss))
        {
            _numberOfSingleGroupLevelsUsed = 0;
            transferPlayerToAnotherLevel();
            return;
        }

        transferPlayerToPlayerLevel();
        _numberOfGroupsPlayed++;
        _numberOfSingleGroupLevelsUsed = 0;
    }

    private void transferPlayerToPlayerLevel()
    {
        if (_playerLevel != null)
        {
            Debug.Log("Transfer player to player level!");

            if (!_playerLevel.IsReady())
                _playerLevel.SetupLevel(true);

            _currentLevel = _playerLevel;
            _playerTransform.position = _playerLevel.GetSpawnPortalPosition();
            return;
        }

        Debug.LogError("No PlayerLevel currently existing!!");
    }

    private void gameCompleted()
    {
        Debug.Log(completionState.gameCompleted);
        Debug.Log("Game completed!");
    }

    private List<LevelObject> getRandomLevelsExcluding(List<LevelObject> levels, List<LevelObject> excludedLevels)
    {
        if (levels.Count == excludedLevels.Count)
            return null;

        List<LevelObject> leftoverLevels = levels.FindAll(level => !excludedLevels.Contains(level));

        return leftoverLevels;
    }
}
