using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class LevelsManager : Singleton<LevelsManager>
{
    public event Action<int> OnArtefactsSet;

    [SerializeField] private int _maxNumOfLevels;

    [Header("Portals")]
    [SerializeField] private List<LevelPortalsList> _levelPortals;
    [SerializeField] private List<LevelPortalsList> _randomLevelPortals;

    private List<Transform> _usedArtefactLocations;

    private List<Portal> _spawnPortals;
    private List<Portal> _exitPortals;
    private List<int> _usedPortals;

    private List<SpawnPoint> _enemySpawnPoints;


    private void Start()
    {
        _spawnPortals = new List<Portal>();
        _exitPortals = new List<Portal>();

        _usedPortals = new List<int>();
        _usedArtefactLocations = new List<Transform>();

        SpawnPoint[] spawnPoints = transform.Find("Locations").GetComponentsInChildren<SpawnPoint>();
        _enemySpawnPoints = new List<SpawnPoint>(spawnPoints);

        setSpawnPortalIndices();
        initializeLevelPortals();
    }

    public List<SpawnPoint> GetEnemySpawnPoints()
    {
        return _enemySpawnPoints;
    }

    public int GetTotalNumOfRequiredArtefacts()
    {
        int numOfArtefacts = 0;

        foreach (Portal portal in _exitPortals)
        {
            if (portal.GetRequiredArtefacts().Count != 0)
                numOfArtefacts += portal.GetRequiredArtefacts().Count;
        }

        return numOfArtefacts;
    }

    private List<LevelPortalsList> initializePoolOfRandomLevels()
    {
        List<LevelPortalsList> randomLevels = new List<LevelPortalsList>();

        if (_levelPortals == null)
            return null;

        if (_levelPortals.Count == 0)
            return null;

        for (int i = 0; i < _maxNumOfLevels; i++)
        {
            int randomLevelIndex = UnityEngine.Random.Range(0, _levelPortals.Count);
            
            if (randomLevels.Contains(_levelPortals[randomLevelIndex]))
            {
                i--;
                continue;
            }
            else
                randomLevels.Add(_levelPortals[randomLevelIndex]);
        }

        return randomLevels;
    }

    private void initializeLevelPortals()
    {
        _randomLevelPortals = initializePoolOfRandomLevels();

        if (_randomLevelPortals == null)
            return;

        int randomStartingLevelIndex = UnityEngine.Random.Range(0, _randomLevelPortals.Count);

        for (int i = 0; i < _randomLevelPortals.Count; i++)
        {
            int randomSpawnIndex = UnityEngine.Random.Range(0, _randomLevelPortals[i].PortalLocations.Count);

            if (randomStartingLevelIndex == i)
            {
                PlayerController.Instance.transform.position = _randomLevelPortals[i].PortalLocations[randomSpawnIndex].position;
            }
            else
            {
                Transform spawnLocation = _randomLevelPortals[i].PortalLocations[randomSpawnIndex];
                Portal spawnPortal = Instantiate(GameAssets.Instance.SpawnPortal, spawnLocation.position, Quaternion.identity, transform.Find("SpawnPortalsContainer")).GetComponent<Portal>();

                _spawnPortals.Add(spawnPortal);
            }

            int randomExitIndex = UnityEngine.Random.Range(0, _randomLevelPortals[i].PortalLocations.Count);

            int count = 0;
            while (count < 10)
            {
                randomExitIndex = UnityEngine.Random.Range(0, _randomLevelPortals[i].PortalLocations.Count);
                if (randomExitIndex != randomSpawnIndex)
                    break;

                count++;
            }

            Transform exitLocation = _randomLevelPortals[i].PortalLocations[randomExitIndex];
            Portal exitPortal = Instantiate(GameAssets.Instance.ExitPortal, exitLocation.position, Quaternion.identity, transform.Find("ExitPortalsContainer")).GetComponent<Portal>();

            bool lockPortal = 1 < UnityEngine.Random.Range(0, 4);

            exitPortal.PortalLocked(lockPortal);
            exitPortal.SetRequiredArtefacts();

            for (int j = 0; j < exitPortal.GetRequiredArtefacts().Count; j++)
            {
                ArtefactItem artefactToSpawn = exitPortal.GetRequiredArtefacts()[j];

                int randomArtefactLocationIndex = UnityEngine.Random.Range(0, exitPortal.GetRequiredArtefacts().Count);
                Transform artefactLocation = _randomLevelPortals[i].ArtefactLocations[randomArtefactLocationIndex];

                if (_usedArtefactLocations.Contains(artefactLocation))
                {
                    j--;
                    continue;
                }
                else
                {
                    Transform spawnedArtefact = ItemSpawner.Instance.SpawnItem(artefactLocation.position, artefactToSpawn);
                    spawnedArtefact.transform.parent = transform.Find("RequiredArtefacts");
                }

                _usedArtefactLocations.Add(artefactLocation);
            }

            _exitPortals.Add(exitPortal);
        }

        OnArtefactsSet?.Invoke(GetTotalNumOfRequiredArtefacts());
    }

    public Portal GetRandomPortal()
    {
        if (_usedPortals.Count == 0)
        {
            int randomPortalIndex = UnityEngine.Random.Range(0, _spawnPortals.Count);
            _usedPortals.Add(randomPortalIndex);

            return _spawnPortals[randomPortalIndex];
        }
        else
        {
            int randomPortalIndex = UnityEngine.Random.Range(0, _spawnPortals.Count);
            int count = 0;

            while (count <= _spawnPortals.Count * 2)
            {
                if (_usedPortals.Count == _spawnPortals.Count)
                    return null;

                randomPortalIndex = UnityEngine.Random.Range(0, _spawnPortals.Count);

                if (!_usedPortals.Contains(randomPortalIndex))
                {
                    _usedPortals.Add(randomPortalIndex);
                    break;
                }

                count++;
            }

            return _spawnPortals[randomPortalIndex];
        }
    }

    public List<Portal> GetSpawnPortals()
    {
        return _spawnPortals;
    }

    public List<int> GetUsedPortals()
    {
        return _usedPortals;
    }

    private void setSpawnPortalIndices()
    {
        int i = 0;

        foreach (Portal spawnPortal in _spawnPortals)
        {
            spawnPortal.SetPortalIndex(i);
            i++;
        }
    }
}
