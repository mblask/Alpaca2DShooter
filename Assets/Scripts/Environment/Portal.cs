using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private PortalType _portalType;
    public PortalType PortalType => _portalType;

    private bool _portalIsClosed = false;

    public void ClosePortal(bool value)
    {
        _portalIsClosed = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_portalIsClosed)
            return;

        PlayerBase playerBase = collision.GetComponent<PlayerBase>();
        if (playerBase == null)
            return;

        if (!_portalType.Equals(PortalType.Exit))
            return;

        LevelsManager.EnteringExitPortal();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_portalIsClosed)
            return;

        PlayerBase playerBase = collision.GetComponent<PlayerBase>();
        if (playerBase == null)
            return;

        LevelsManager.ExitingExitPortal();
    }
}
