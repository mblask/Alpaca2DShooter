using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalType
{
    Spawn,
    Exit
}

public class Portal : MonoBehaviour
{
    private const int MIN_ARTEFACTS_REQUIRED = 1;
    private const int MAX_ARTEFACTS_REQUIRED = 2;

    public PortalType PortalType;

    public static event Action<Portal> OnTriggerLockedPortal;

    private int _numOfArtefactsRequired;
    [SerializeField] private List<ArtefactItem> _artefactsRequired = new List<ArtefactItem>();
    private int _portalIndex;
    private bool _locked;

    public void SetRequiredArtefacts()
    {
        if (_locked)
        {
            _artefactsRequired = new List<ArtefactItem>();

            _numOfArtefactsRequired = UnityEngine.Random.Range(MIN_ARTEFACTS_REQUIRED, MAX_ARTEFACTS_REQUIRED + 1);

            for (int i = 0; i < _numOfArtefactsRequired; i++)
            {
                ArtefactItem artefact = ItemSpawner.Instance.GetAvailableArtefacts()[UnityEngine.Random.Range(0, ItemSpawner.Instance.GetAvailableArtefacts().Count)];

                if (!_artefactsRequired.Contains(artefact))
                {
                    _artefactsRequired.Add(artefact);
                }
                else
                    i--;
            }
        }
    }

    public void SetPortalIndex(int index)
    {
        _portalIndex = index;
    }

    public int GetPortalIndex()
    {
        return _portalIndex;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag(ConstsEnums.PlayerTag))
            return;

        if (!GameManager.Instance.IsGameRunning())
            return;

        PlayerArtefacts playerArtefacts = collider.GetComponent<PlayerArtefacts>();

        if (PortalType == PortalType.Exit)
            if (!_locked)
                transportCharacterToRandomLevel(collider.transform);
            else
            {
                if (playerArtefacts.CheckRequiredArtefacts(_artefactsRequired))
                    transportCharacterToRandomLevel(collider.transform);
                else
                    OnTriggerLockedPortal?.Invoke(this);
            }
    }

    private void transportCharacterToRandomLevel(Transform characterTransform)
    {
        Portal spawnPortal = LevelsManager.Instance?.GetRandomPortal();

        if (spawnPortal != null)
            characterTransform.position = spawnPortal.transform.position;
        else
            GameManager.Instance?.TriggerVictory();
    }

    public void PortalLocked(bool value)
    {
        _locked = value;
    }

    public List<ArtefactItem> GetRequiredArtefacts()
    {
        return _artefactsRequired;
    }
}
