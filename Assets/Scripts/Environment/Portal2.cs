using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class Portal2 : MonoBehaviour
{
    [SerializeField] private PortalType _portalType;

    private List<ArtefactItem> _requiredArtefacts;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBase playerBase = collision.GetComponent<PlayerBase>();
        if (playerBase == null)
            return;

        if (!_portalType.Equals(PortalType.Exit))
            return;

        transferPlayer(playerBase);
    }

    private void transferPlayer(PlayerBase playerBase)
    {
        LevelObject nextLevel = LevelsManager2.SetupRandomNewLevelStatic();

        if (nextLevel == null)
            return;

        Vector3 nextSpawnPosition = nextLevel.GetSpawnPortalPosition();
        playerBase.transform.position = nextSpawnPosition;
    }
}
