using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;
using System;

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

    public Action OnExitingPlayerLevel;

    [SerializeField] private int _levelsToPass = 2;

    private List<LevelObject> _levelsList;
    private List<LevelObject> _bossLevelsList;
    private LevelObject _playerLevel;
    private int _numberOfGroupsPlayed = 0;
    private int _numberOfSingleGroupLevelsUsed = 0;

    private Transform _playerTransform;
    private bool _playerSpawned = false;

    private LevelObject _currentLevel;

    private void Awake()
    {
        _instance = this;
        _levelsList = Utilities.GetListOfObjectsFromContainer<LevelObject>(transform, "Regular");
        _bossLevelsList = Utilities.GetListOfObjectsFromContainer<LevelObject>(transform, "Boss");
        _playerLevel = transform.Find("PlayersLevel").GetComponent<LevelObject>();
    }

    private void Start()
    {
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
        _playerSpawned = true;

        _numberOfSingleGroupLevelsUsed++;

        return true;
    }

    private enum completionState
    {
        exitingPlayerLevel,
        exitingBossLevel,
        groupNotCompleted,
        groupCompleted,
    }

    private completionState checkGroupCompleted()
    {
        if (_currentLevel.GetLevelType().Equals(LevelType.Boss))
            return completionState.exitingBossLevel;

        if (_currentLevel.Equals(_playerLevel))
            return completionState.exitingPlayerLevel;

        if (_numberOfSingleGroupLevelsUsed == _levelsToPass)
            return completionState.groupCompleted;

        return completionState.groupNotCompleted;
    }

    public static void CheckCompletionStateStatic()
    {
        _instance.checkCompletionState();
    }

    private void checkCompletionState()
    {
        switch (checkGroupCompleted())
        {
            case completionState.exitingBossLevel:
                exitingBossLevel();
                break;

            case completionState.exitingPlayerLevel:
                exitingPlayerLevel();
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

    private void exitingBossLevel()
    {
        _numberOfSingleGroupLevelsUsed = 0;
        transferPlayerToAnotherLevel();
    }

    private void exitingPlayerLevel()
    {
        OnExitingPlayerLevel?.Invoke();
    }

    public static void TransferPlayerToBossLevelStatic()
    {
        _instance.transferPlayerToBossLevel();
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
        LevelObject level = getNotPlayedLevels(_levelsList).GetRandomElement();

        if (level != null)
        {
            level.SetupLevel(true);
            _numberOfSingleGroupLevelsUsed++;

            _playerTransform.position = level.GetSpawnPortalPosition();

            _currentLevel.ClearLevel();
            _currentLevel = level;
            Debug.Log($"Entered: {_currentLevel.name}");
        }
    }

    private void groupCompleted()
    {
        _numberOfGroupsPlayed++;

        Debug.Log(completionState.groupCompleted);
        Debug.Log($"Number of groups played: {_numberOfGroupsPlayed}");
        if (_currentLevel.GetLevelType().Equals(LevelType.Boss))
        {
            transferPlayerToAnotherLevel();
            return;
        }

        transferPlayerToPlayerLevel();
        _numberOfSingleGroupLevelsUsed = 0;
    }

    private void transferPlayerToPlayerLevel()
    {
        if (_playerLevel == null)
        {
            Debug.LogError("No PlayerLevel currently existing!!");
            return;
        }

        Debug.Log("Transfer player to player level!");

        if (!_playerLevel.IsReady())
            _playerLevel.SetupLevel(true);

        _currentLevel = _playerLevel;
        _playerTransform.position = _playerLevel.GetSpawnPortalPosition();
    }

    private List<LevelObject> getNotPlayedLevels(List<LevelObject> levels)
    {
        return levels.FindAll(level => !level.WasPlayed());
    }
}
