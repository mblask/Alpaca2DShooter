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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBase playerBase = collision.GetComponent<PlayerBase>();
        if (playerBase == null)
            return;

        if (!_portalType.Equals(PortalType.Exit))
            return;

        LevelsManager.CheckCompletionStateStatic();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerBase playerBase = collision.GetComponent<PlayerBase>();
        if (playerBase == null)
            return;

        //Debug.Log("Cancel active UI elements! - How to?");
    }
}
