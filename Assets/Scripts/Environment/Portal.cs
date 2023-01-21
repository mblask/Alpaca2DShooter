using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalType
{
    Spawn,
    Exit,
}

public class Portal : MonoBehaviour
{
    [SerializeField] private PortalType _portalType;

    private List<ArtefactItem> _requiredArtefacts;
    private LevelObject _currentLevel;

    private Transform _NPCBossTransform = null;
    private bool _isBossPortal = false;

    private bool _isPlayerLevelPortal = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBase playerBase = collision.GetComponent<PlayerBase>();
        if (playerBase == null)
            return;

        if (!_portalType.Equals(PortalType.Exit))
            return;

        transferPlayer(playerBase);
        clearCurrentLevel();
    }

    private void transferPlayer(PlayerBase playerBase)
    {
        if (_isBossPortal && _NPCBossTransform != null)
            return;

        if (LevelsManager.levelsFinished)
            return;

        LevelObject nextLevel = LevelsManager.SetupRandomNewLevelStatic();

        if (nextLevel == null)
            return;

        Vector3 nextSpawnPosition = nextLevel.GetSpawnPortalPosition();
        playerBase.transform.position = nextSpawnPosition;
    }

    private void clearCurrentLevel()
    {
        if (_currentLevel != null && !_isPlayerLevelPortal)
            _currentLevel.ClearLevel();
    }

    public void SetPortalLevel(LevelObject levelObject)
    {
        _currentLevel = levelObject;
    }

    public void SetBossPortal(Transform bossTransform)
    {
        if (bossTransform == null)
            return;

        _NPCBossTransform = bossTransform;
        _isBossPortal = true;
    }

    public void SetPlayerLevelPortal(bool value)
    {
        _isPlayerLevelPortal = value;
    }
}
