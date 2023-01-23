using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArtefacts : Singleton<PlayerArtefacts>
{
    public event Action<int> OnArtefactCollected;

    [Header("Items - read only")]
    [SerializeField] private List<ArtefactItem> _artefactsCollected = new List<ArtefactItem>();

    public bool AddArtefact(ArtefactItem artefact)
    {
        if (artefact == null)
            return false;

        _artefactsCollected.Add(artefact);

        OnArtefactCollected?.Invoke(_artefactsCollected.Count);

        return true;
    }

    public bool CheckRequiredArtefacts(List<ArtefactItem> requiredArtefacts)
    {
        bool containsRequiredArtefacts = true;

        foreach (ArtefactItem artefactItem in requiredArtefacts)
        {
            if (!_artefactsCollected.Contains(artefactItem))
            {
                containsRequiredArtefacts = false;
                break;
            }
        }

        if (containsRequiredArtefacts)
            _artefactsCollected.Clear();

        return containsRequiredArtefacts;
    }

    public List<ArtefactItem> GetArtefacts()
    {
        return _artefactsCollected;
    }
}