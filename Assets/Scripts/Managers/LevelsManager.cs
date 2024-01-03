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

    [Tooltip("Initial number of levels required to pass a group of levels\r\nRequired levels to pass decrease by one after each boss level")]
    [SerializeField] private int _levelsToPass = 2;
    private int _bossLevel = 0;
    public int BossLevel => _bossLevel;

    private List<LevelObject> _levelsList;
    private List<LevelObject> _bossLevelsList;
    private LevelObject _playerLevel;
    private int _numberOfGroupsPlayed = 0;
    [SerializeField] private int _numberOfSingleGroupLevelsUsed = 0;

    private bool _firstSpawn = true;
    [Tooltip("If true, when all boss levels completed it will reset them and start again\r\nIf false, when all boss levels completed, it will move player to other normal levels")]
    [SerializeField] private bool _rotateBossLevels = true;
    [SerializeField] private bool _spawnInPlayerLevel = true;
    private Transform _playerTransform;
    private bool _playerSpawned = false;

    private LevelObject _currentLevel;

    private GameManager _gameManager;

    private LevelProgressChoiceUI _levelProgressChoiceUI;
    private PlayerOrBossLevelUI _playerOrBossLevelUI;

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
        _levelProgressChoiceUI = LevelProgressChoiceUI.Instance;
        _playerOrBossLevelUI = PlayerOrBossLevelUI.Instance;

        if (_spawnInPlayerLevel)
            spawnPlayerInPlayersLevel();

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
        _playerSpawned = true;
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

    public enum CompletionState
    {
        firstExitingPlayerLevel,
        exitingPlayerLevel,
        exitingBossLevel,
        groupNotCompleted,
        groupCompleted,
    }

    private CompletionState checkGroupCompleted()
    {
        if (_currentLevel.GetLevelType().Equals(LevelType.Boss))
            return CompletionState.exitingBossLevel;

        if (_currentLevel.Equals(_playerLevel))
        {
            if (!_firstSpawn)
                return CompletionState.exitingPlayerLevel;
            
            _firstSpawn = false;
            return CompletionState.firstExitingPlayerLevel;
        }

        if (_numberOfSingleGroupLevelsUsed == _levelsToPass)
            return CompletionState.groupCompleted;

        return CompletionState.groupNotCompleted;
    }

    public static void EnteringExitPortal()
    {
        _instance.checkCompletionState();
    }

    public static void ExitingExitPortal()
    {
        if (_instance._currentLevel.Equals(_instance._playerLevel))
            _instance._levelProgressChoiceUI?.ShowUI(false);

        _instance._playerOrBossLevelUI?.ActivateUI(false);
    }

    private void checkCompletionState()
    {
        CompletionState state = checkGroupCompleted();
        switch (state)
        {
            case CompletionState.exitingBossLevel:
                exitingBossLevel();
                break;

            case CompletionState.firstExitingPlayerLevel:
                transferPlayerToAnotherLevel();
                break;

            case CompletionState.exitingPlayerLevel:
                exitingPlayerLevel();
                break;

            case CompletionState.groupCompleted:
                _playerOrBossLevelUI.ActivateUI(true);
                break;

            case CompletionState.groupNotCompleted:
                groupNotCompleted();
                break;

            default:
                break;
        }
    }

    private void exitingBossLevel()
    {
        _levelsToPass--;
        if (_levelsToPass == 0)
        {
            _gameManager.TriggerVictory();
            return;
        }

        _numberOfSingleGroupLevelsUsed = 0;
        transferPlayerToAnotherLevel();
    }

    private void exitingPlayerLevel()
    {
        _levelProgressChoiceUI.ShowUI(true);
    }

    public void TransferPlayerToLevel(LevelType levelType)
    {
        switch(levelType)
        {
            case LevelType.Player:
                transferPlayerToPlayerLevel();
                break;
            case LevelType.Boss:
                transferPlayerToBossLevel();
                break;
            case LevelType.Normal:
                transferPlayerToAnotherLevel();
                break;
        }
    }

    public static void TransferPlayerToBossLevelStatic()
    {
        _instance.transferPlayerToBossLevel();
    }

    private void transferPlayerToBossLevel()
    {
        LevelObject level = _bossLevelsList
            .FindAll(level => !level.WasPlayed()).GetRandomElement();

        if (level == null)
        {
            if (!_rotateBossLevels)
            {
                exitingBossLevel();
                return;
            }

            _bossLevel = 0;
            _bossLevelsList.ForEach(level => level.SetPlayed(false));
            level = _bossLevelsList.GetRandomElement();
        }

        _bossLevel++;
        if (!level.IsReady())
            level.SetupLevel();

        _currentLevel = level;
        _playerTransform.position = level.GetSpawnPortalPosition();
    }

    private void groupNotCompleted()
    {
        transferPlayerToAnotherLevel();
    }

    private void transferPlayerToAnotherLevel()
    {
        LevelObject level = getLevelsNotPlayed(_levelsList).GetRandomElement();

        if (level == null)
        {
            _levelsList.ForEach(level => level.SetPlayed(false));
            level = _levelsList.GetRandomElement();
        }

        level.SetupLevel(true);
        _numberOfSingleGroupLevelsUsed++;

        _playerTransform.position = level.GetSpawnPortalPosition();

        _currentLevel.ClearLevel();
        _currentLevel = level;
    }

    private void groupCompleted()
    {
        _numberOfGroupsPlayed++;

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
            return;
        }

        if (!_playerLevel.IsReady())
            _playerLevel.SetupLevel(true);

        _currentLevel.ClearLevel();
        _currentLevel = _playerLevel;
        _playerTransform.position = _playerLevel.GetSpawnPortalPosition();
    }

    private List<LevelObject> getLevelsNotPlayed(List<LevelObject> levels)
    {
        return levels.FindAll(level => !level.WasPlayed());
    }
}
