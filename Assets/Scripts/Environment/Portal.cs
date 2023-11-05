using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private PortalType _portalType;
    public PortalType PortalType => _portalType;

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
    }
}
