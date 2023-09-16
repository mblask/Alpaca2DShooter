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
    private Transform _spawnPortalTransform;
    private Transform _exitPortalTransform;

    private Transform _environmentContainer;
    private Transform _npcContainer;

    private int _maxNumberOfArtefactsRequired = 3;
    private List<ArtefactItem> _requiredArtefacts;

    [SerializeField] private LevelType _levelType;
    [SerializeField] private bool _wasPlayed = false;

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

    public void SetupLevel(bool levelNeedsSpawnPortal = true)
    {
        spawnPortals(levelNeedsSpawnPortal);
        spawnEnemies();
        spawnTraps();
        setRequiredArtefacts();

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
        if (_exitPortalTransform != null)
            Destroy(_exitPortalTransform.gameObject);

        if (_spawnPortalTransform != null)
            Destroy(_spawnPortalTransform.gameObject);
    }

    private void clearNPCs()
    {
        if (_npcContainer == null)
            return;

        Debug.Log($"Clear: {name}");

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

    private void setRequiredArtefacts()
    {
        if (_levelType.Equals(LevelType.Boss) || _levelType.Equals(LevelType.Player))
            return;

        _requiredArtefacts = new List<ArtefactItem>();

        for (int i = 0; i < _maxNumberOfArtefactsRequired; i++)
        {
            if (!Utilities.ChanceFunc(67))
                continue;

            ArtefactItem artefact = GameAssets.Instance.AvailableArtefacts.GetRandomElement();

            if (_requiredArtefacts.Contains(artefact))
            {
                i--;
                continue;
            }

            _requiredArtefacts.Add(artefact);
        }
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
        SpawnPoint randomSpawnPoint = _portalSpawnPoints.FindAll(point => point.IsActive()).GetRandomElement();

        if (_environmentContainer == null)
            transform.AddNewGameObject("Environment");

        _exitPortalTransform = Instantiate(GameAssets.Instance.ExitPortal, randomSpawnPoint.Location, Quaternion.identity, _environmentContainer);
        
        float deactivateSpawnPointsInRadius = 6.0f;
        if (_levelType.Equals(LevelType.Boss))
            deactivateSpawnPointsInRadius *= 0.5f;
        
        deactivateSpawnPointsAround(randomSpawnPoint.Location, deactivateSpawnPointsInRadius);
        
        randomSpawnPoint.SetActive(false);

        if (spawnPortalNeeded)
        {
            randomSpawnPoint = _portalSpawnPoints.FindAll(point => point.IsActive()).GetRandomElement();

            _spawnPortalTransform = Instantiate(GameAssets.Instance.SpawnPortal, randomSpawnPoint.Location, Quaternion.identity, _environmentContainer);

            deactivateSpawnPointsAround(randomSpawnPoint.Location, deactivateSpawnPointsInRadius);

            randomSpawnPoint.SetActive(false);
        }

        _portalSpawnPoints.ForEach(spawnPoint => spawnPoint.SetActive(false));
    }

    public Vector3 GetSpawnPortalPosition()
    {
        if (_spawnPortalTransform == null)
            return default;

        return _spawnPortalTransform.position;
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

                Transform boss = GameAssets.Instance.GetBossById(bossLevel);
                Instantiate(boss, spawnPoint.transform.position, Quaternion.identity, _npcContainer);
                _bossSpawned = true;
            }
        }

        int chanceToSpawnEnemy = 75;
        int chanceToSpawnDoubleEnemies = 25;
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
                    Instantiate(GameAssets.Instance.NPCPrefab, position, Quaternion.identity, _npcContainer);
                }

                continue;
            }

            Instantiate(GameAssets.Instance.NPCPrefab, spawnPoint.Location, Quaternion.identity, _npcContainer);
        }

        _enemySpawnPoints.ForEach(spawnPoint => spawnPoint.SetActive(false));
        _playerSpawnPoints.ForEach(spawnPoint => spawnPoint.SetActive(false));
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
