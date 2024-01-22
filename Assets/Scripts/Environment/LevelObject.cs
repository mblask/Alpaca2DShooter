using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class LevelObject : MonoBehaviour
{
    private Transform _locationsContainer;
    private List<SpawnPoint> _enemySpawnPoints = new List<SpawnPoint>();
    private List<SpawnPoint> _trapsSpawnPoints = new List<SpawnPoint>();
    private List<Transform> _spawnedTraps = new List<Transform>();
    private List<SpawnPoint> _playerSpawnPoints = new List<SpawnPoint>();
    [SerializeField] private List<SpawnPoint> _portalSpawnPoints;
    private Portal _spawnPortal;
    private Portal _exitPortal;
    private NPCBase _levelBoss;

    private Transform _environmentContainer;
    private Transform _npcContainer;

    [SerializeField] private LevelType _levelType;
    [SerializeField] private bool _wasPlayed = false;

    [SerializeField] private int _levelNumber = 1;

    private bool _bossSpawned = false;

    private bool _isReady = false;

    private void Awake()
    {
        _locationsContainer = transform.Find("Locations");

        if (_locationsContainer == null)
            return;

        _enemySpawnPoints = Utilities.GetListOfObjectsFromContainer<SpawnPoint>(_locationsContainer, "EnemySpawnPoints");
        _trapsSpawnPoints = Utilities.GetListOfObjectsFromContainer<SpawnPoint>(_locationsContainer, "TrapSpawnPoints");
        _playerSpawnPoints = Utilities.GetListOfObjectsFromContainer<SpawnPoint>(_locationsContainer, "PlayerSpawnPoints");
        _portalSpawnPoints = Utilities.GetListOfObjectsFromContainer<SpawnPoint>(_locationsContainer, "PortalSpawnPoints");

        _environmentContainer = transform.AddNewGameObject("Environment");
        _npcContainer = transform.AddNewGameObject("NPCs");
    }

    public Transform SpawnPlayer()
    {
        if (_playerSpawnPoints == null)
            return null;

        if (_playerSpawnPoints.Count == 0)
            return null;

        SpawnPoint randomSpawnPoint = _playerSpawnPoints.FindAll(point => point.IsActive()).GetRandomElement();
        
        Transform playerTransform = Instantiate(GameAssets.Instance.Player, randomSpawnPoint.Location, Quaternion.identity, null);
        randomSpawnPoint.SetActive(false);

        float deactivateSpawnPointsInRadius = 6.0f;
        deactivateSpawnPointsAround(randomSpawnPoint.Location, deactivateSpawnPointsInRadius);

        return playerTransform;
    }

    public void SetupLevel(int levelNumber, bool levelNeedsSpawnPortal = true)
    {
        _levelNumber = levelNumber;

        spawnPortals(levelNeedsSpawnPortal);
        spawnEnemies();
        spawnTraps();

        _isReady = true;
        _wasPlayed = true;
    }

    public void ClearLevel()
    {
        resetSpawnPoints();

        clearPortals();
        clearNPCs();
        clearTraps();

        _isReady = false;
    }

    private void clearPortals()
    {
        if (_exitPortal?.transform != null)
            Destroy(_exitPortal.gameObject);
        
        if (_spawnPortal?.transform != null)
            Destroy(_spawnPortal.gameObject);
    }

    private void clearNPCs()
    {
        if (_npcContainer == null)
            return;

        foreach (Transform npc in _npcContainer)
            Destroy(npc.gameObject);
    }

    private void clearTraps()
    {
        if (_spawnedTraps == null)
            return;

        if (_spawnedTraps.Count == 0)
            return;

        foreach (Transform trap in _spawnedTraps)
            if (trap != null) Destroy(trap.gameObject);
    }

    private void deactivateSpawnPointsAround(Vector3 position, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
        foreach (Collider2D collider in colliders)
        {
            SpawnPoint spawnPoint = collider.GetComponent<SpawnPoint>();
            if (spawnPoint != null)
                spawnPoint.SetActive(false);
        }
    }

    private void spawnPortals(bool spawnPortalNeeded = true)
    {
        spawnExitPortal();
        spawnSpawnPortal(spawnPortalNeeded);

        _portalSpawnPoints.ForEach(spawnPoint => spawnPoint.SetActive(false));
    }

    private void spawnExitPortal()
    {
        SpawnPoint randomSpawnPoint = _portalSpawnPoints.FindAll(point => point.IsActive()).GetRandomElement();

        if (_environmentContainer == null)
            transform.AddNewGameObject("Environment");

        _exitPortal = Instantiate(GameAssets.Instance.ExitPortal, randomSpawnPoint.Location, Quaternion.identity, _environmentContainer).GetComponent<Portal>();

        float deactivateSpawnPointsInRadius = 6.0f;
        if (_levelType.Equals(LevelType.Boss))
        {
            deactivateSpawnPointsInRadius *= 0.5f;
            _exitPortal.ClosePortal(true);
        }

        if (_levelType != LevelType.Player)
            deactivateSpawnPointsAround(randomSpawnPoint.Location, deactivateSpawnPointsInRadius);
        
        randomSpawnPoint.SetActive(false);
    }

    private void spawnSpawnPortal(bool spawnPortalNeeded)
    {
        if (!spawnPortalNeeded)
            return;

        SpawnPoint randomSpawnPoint = _portalSpawnPoints.FindAll(point => point.IsActive()).GetRandomElement();
        _spawnPortal = Instantiate(GameAssets.Instance.SpawnPortal, randomSpawnPoint.Location, Quaternion.identity, _environmentContainer).GetComponent<Portal>();

        float deactivateSpawnPointsInRadius = 6.0f;
        deactivateSpawnPointsAround(randomSpawnPoint.Location, deactivateSpawnPointsInRadius);
        randomSpawnPoint.SetActive(false);
    }

    public Vector3 GetSpawnPortalPosition()
    {
        if (_spawnPortal == null || _spawnPortal.transform == null)
            return default;

        return _spawnPortal.transform.position;
    }

    private void spawnEnemies()
    {
        if (_levelType.Equals(LevelType.Player))
            return;

        if (_enemySpawnPoints == null)
            return;

        if (_enemySpawnPoints.Count == 0)
            return;

        if (_levelType.Equals(LevelType.Boss))
        {
            int bossLevel = LevelsManager.Instance.BossLevel;
            foreach (SpawnPoint spawnPoint in _enemySpawnPoints)
            {
                if (_bossSpawned)
                    continue;

                if (!spawnPoint.IsActive())
                    continue;

                Transform bossTransform = Instantiate
                    (GameAssets.Instance.NPCBossPrefab, spawnPoint.transform.position, Quaternion.identity, _npcContainer);
                _levelBoss = bossTransform.GetComponent<NPCBase>();
                _levelBoss.SetBossPositionLevel(this);
                _levelBoss.SetBossLevel(bossLevel);
                _bossSpawned = true;
            }
        }

        int chanceToSpawnEnemy = 75;
        int chanceToSpawnDoubleEnemies = 10;
        foreach (SpawnPoint spawnPoint in _enemySpawnPoints)
        {
            if (!spawnPoint.IsActive())
                continue;

            if (!Utilities.ChanceFunc(chanceToSpawnEnemy))
                continue;

            if (Utilities.ChanceFunc(chanceToSpawnDoubleEnemies))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 position = spawnPoint.Location + i * Vector3.up;
                    spawnEnemy(position);
                }

                continue;
            }

            spawnEnemy(spawnPoint.Location);
        }

        _enemySpawnPoints.ForEach(spawnPoint => spawnPoint.SetActive(false));
        _playerSpawnPoints.ForEach(spawnPoint => spawnPoint.SetActive(false));
    }

    private void spawnEnemy(Vector3 position)
    {
        Instantiate(GameAssets.Instance.NPCPrefab, position, Quaternion.identity, _npcContainer);
    }

    public void BossKilled()
    {
        _exitPortal.ClosePortal(false);
    }

    private void spawnTraps()
    {
        if (_levelType.Equals(LevelType.Player))
            return;

        //if spawn points list is null, return
        if (_trapsSpawnPoints == null)
            return;

        //if there are no spawn points in the level, return
        if (_trapsSpawnPoints.Count == 0)
            return;

        //spawn traps at every spawn point; only 33% of traps will initialize themselves
        foreach (SpawnPoint spawnPoint in _trapsSpawnPoints)
        {
            if (!spawnPoint.IsActive())
                continue;

            _spawnedTraps.Add(Instantiate(GameAssets.Instance.FiringTrap, spawnPoint.Location, Quaternion.identity, _npcContainer));
        }

        //deactivate any remaining spawn points in the level
        _trapsSpawnPoints.ForEach(spawnPoint => spawnPoint.SetActive(false));
    }

    private void resetSpawnPoints()
    {
        _portalSpawnPoints.ForEach(point => point.SetActive(true));
        _enemySpawnPoints.ForEach(point => point.SetActive(true));
        _trapsSpawnPoints.ForEach(point => point.SetActive(true));
        _playerSpawnPoints.ForEach(point => point.SetActive(true));
    }

    public bool ContainsPlayerSpawnPoints()
    {
        if (_playerSpawnPoints != null)
            return _playerSpawnPoints.Count > 0;

        return false;
    }

    public LevelType GetLevelType()
    {
        return _levelType;
    }

    public bool IsReady()
    {
        return _isReady;
    }

    public void SetPlayed(bool value)
    {
        _wasPlayed = value;
    }

    public bool WasPlayed()
    {
        return _wasPlayed;
    }
}
