using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public enum LevelType
{
    Normal,
    Player,
    Boss,
}

public class LevelObject : MonoBehaviour
{
    private Transform _locationsContainer;
    protected List<SpawnPoint> _enemySpawnPoints;
    private List<SpawnPoint> _trapsSpawnPoints;
    protected List<SpawnPoint> _playerSpawnPoints;
    private List<SpawnPoint> _portalSpawnPoints;
    private Transform _spawnPortalTransform;
    protected Portal _exitPortal;

    protected Transform _environmentContainer;
    protected Transform _npcContainer;

    private int _maxNumberOfArtefactsRequired = 3;
    private List<ArtefactItem> _requiredArtefacts;

    [SerializeField] private LevelType _levelType;
    private bool _isFinished = false;

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

        SpawnPoint randomSpawnPoint = _playerSpawnPoints.GetRandomElement();
        
        Transform playerTransform = Instantiate(GameAssets.Instance.Player, randomSpawnPoint.Location, Quaternion.identity, null);

        float destroySpawnPointsInRadius = 6.0f;
        destroySpawnPointsAround(_portalSpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
        destroySpawnPointsAround(_enemySpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
        destroySpawnPointsAround(_trapsSpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);

        return playerTransform;
    }

    public void SetupLevel(bool levelNeedsSpawnPortal = true)
    {
        spawnPortals(levelNeedsSpawnPortal);
        //spawnEnemies();
        spawnTraps();
        setRequiredArtefacts();

        _isReady = true;
    }

    public void ClearLevel()
    {
        if (_npcContainer == null)
            return;

        Debug.Log("Clear: " + name);

        foreach (Transform npc in _npcContainer)
            Destroy(npc.gameObject);
    }

    protected void setRequiredArtefacts()
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

    private void destroySpawnPointsAround(List<SpawnPoint> spawnPoints, Vector3 position, float radius)
    {
        if (spawnPoints == null)
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, radius);
        foreach (Collider2D collider in colliders)
        {
            SpawnPoint spawnPoint = collider.GetComponent<SpawnPoint>();
            if (spawnPoint == null)
                continue;

            if (!spawnPoints.Contains(spawnPoint))
                continue;

            spawnPoints.Remove(spawnPoint);
            Destroy(spawnPoint.gameObject);
        }
    }

    protected void spawnPortals(bool spawnPortalNeeded = true)
    {
        SpawnPoint randomSpawnPoint = _portalSpawnPoints.GetRandomElement();

        if (_environmentContainer == null)
            transform.AddNewGameObject("Environment");

        Transform portalTransform = Instantiate(GameAssets.Instance.ExitPortal, randomSpawnPoint.Location, Quaternion.identity, _environmentContainer);
        _portalSpawnPoints.Remove(randomSpawnPoint);
        
        float destroySpawnPointsInRadius = 6.0f;
        destroySpawnPointsAround(_enemySpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
        destroySpawnPointsAround(_trapsSpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
        Destroy(randomSpawnPoint.gameObject);

        _exitPortal = portalTransform.GetComponent<Portal>();

        if (spawnPortalNeeded)
        {
            randomSpawnPoint = _portalSpawnPoints.GetRandomElement();

            _spawnPortalTransform = Instantiate(GameAssets.Instance.SpawnPortal, randomSpawnPoint.Location, Quaternion.identity, _environmentContainer);
            
            _portalSpawnPoints.Remove(randomSpawnPoint);

            destroySpawnPointsAround(_enemySpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
            destroySpawnPointsAround(_trapsSpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
            Destroy(randomSpawnPoint.gameObject);
        }

        _portalSpawnPoints.ForEach(spawnPoint => Destroy(spawnPoint.gameObject));
    }

    public Vector3 GetSpawnPortalPosition()
    {
        if (_spawnPortalTransform == null)
            return default(Vector3);

        return _spawnPortalTransform.position;
    }

    protected void spawnEnemies()
    {
        if (_levelType.Equals(LevelType.Player))
            return;

        if (_enemySpawnPoints == null)
            return;

        if (_enemySpawnPoints.Count == 0)
            return;

        int chanceToSpawnEnemy = 75;
        int chanceToSpawnDoubleEnemies = 25;
        foreach (SpawnPoint spawnPoint in _enemySpawnPoints)
        {
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

        _enemySpawnPoints.ForEach(spawnPoint => Destroy(spawnPoint.gameObject));
        _playerSpawnPoints.ForEach(spawnPoint => Destroy(spawnPoint.gameObject));
    }

    protected void spawnTraps()
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
            Instantiate(GameAssets.Instance.FiringTrap, spawnPoint.Location, Quaternion.identity, _npcContainer);

        //destroy any remaining spawn points in the level
        _trapsSpawnPoints.ForEach(spawnPoint => Destroy(spawnPoint.gameObject));
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

    public void SetFinished(bool value)
    {
        _isFinished = value;
    }

    public bool IsFinished()
    {
        return _isFinished;
    }
}
