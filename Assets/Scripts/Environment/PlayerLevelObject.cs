using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelObject : LevelObject
{
    public override void SetupLevel(bool levelNeedsSpawnPortal)
    {
        spawnPortals();
    }

    protected override void spawnPortals(bool spawnPortalNeeded = true)
    {
        SpawnPoint portalSpawnPoint = transform.Find("Locations").Find("PortalSpawnPoints").Find("PortalSpawnPoint").GetComponent<SpawnPoint>();
        _exitPortal = Instantiate(GameAssets.Instance.ExitPortal, portalSpawnPoint.Location, Quaternion.identity, _environmentContainer).GetComponent<Portal>();

        _exitPortal.SetPlayerLevelPortal(true);
    }
}
