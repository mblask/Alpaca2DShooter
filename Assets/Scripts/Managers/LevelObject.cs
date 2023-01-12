using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class LevelObject : MonoBehaviour
{
    private Transform _locationsContainer;
    private List<SpawnPoint> _enemySpawnPoints;
    private List<SpawnPoint> _trapsSpawnPoints;
    private List<SpawnPoint> _playerSpawnPoints;
    private List<SpawnPoint> _portalSpawnPoints;
    private Transform _spawnPortalTransform;

    private Transform _environmentContainer;
    private Transform _npcContainer;

    private int _maxNumberOfArtefactsRequired = 3;
    private List<ArtefactItem> _requiredArtefacts;

    private void Awake()
    {
        _locationsContainer = transform.Find("Locations");

        if (_locationsContainer == null)
            return;
        
        _enemySpawnPoints = new List<SpawnPoint>(_locationsContainer.Find("EnemySpawnPoints").GetComponentsInChildren<SpawnPoint>());
        _trapsSpawnPoints = Utilities.GetListOfObjectsFromContainer<SpawnPoint>(_locationsContainer, "TrapSpawnPoints");
        _playerSpawnPoints = new List<SpawnPoint>(_locationsContainer.Find("PlayerSpawnPoints").GetComponentsInChildren<SpawnPoint>());
        _portalSpawnPoints = new List<SpawnPoint>(_locationsContainer.Find("PortalSpawnPoints").GetComponentsInChildren<SpawnPoint>());

        _environmentContainer = transform.AddNewGameObject("Environment");
        _npcContainer = transform.AddNewGameObject("NPCs");
    }

    public void SpawnPlayer()
    {
        if (_playerSpawnPoints == null)
            return;

        if (_playerSpawnPoints.Count == 0)
            return;

        SpawnPoint randomSpawnPoint = _playerSpawnPoints.GetRandomElement();

        Instantiate(GameAssets.Instance.Player, randomSpawnPoint.Location, Quaternion.identity, null);

        float destroySpawnPointsInRadius = 6.0f;
        destroySpawnPointsAround(_portalSpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
        destroySpawnPointsAround(_enemySpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
        destroySpawnPointsAround(_trapsSpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
    }

    public void SetupLevel(bool levelNeedsSpawnPortal)
    {
        spawnPortals(levelNeedsSpawnPortal);
        spawnEnemies();
        spawnTraps();
        setRequiredArtefacts();
    }

    private void setRequiredArtefacts()
    {
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

    private void spawnPortals(bool spawnPortalNeeded = true)
    {
        SpawnPoint randomSpawnPoint = _portalSpawnPoints.GetRandomElement();

        Instantiate(GameAssets.Instance.ExitPortal, randomSpawnPoint.Location, Quaternion.identity, _environmentContainer);
        _portalSpawnPoints.Remove(randomSpawnPoint);
        Destroy(randomSpawnPoint.gameObject);

        if (spawnPortalNeeded)
        {
            randomSpawnPoint = _portalSpawnPoints.GetRandomElement();

            _spawnPortalTransform = Instantiate(GameAssets.Instance.SpawnPortal, randomSpawnPoint.Location, Quaternion.identity, _environmentContainer);
            
            _portalSpawnPoints.Remove(randomSpawnPoint);

            float destroySpawnPointsInRadius = 6.0f;
            destroySpawnPointsAround(_enemySpawnPoints, randomSpawnPoint.Location, destroySpawnPointsInRadius);
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

    private void spawnEnemies()
    {
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

    private void spawnTraps()
    {
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
}
