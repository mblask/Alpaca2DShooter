using UnityEngine;

public class ExitPortalPosition : MonoBehaviour
{
    private static ExitPortalPosition _instance;
    public static ExitPortalPosition Instance
    {
        get
        {
            return _instance;
        }
    }

    private SpriteRenderer _spriteRenderer;

    private float _radiusFromPlayer = 2.0f;

    private PlayerStats _playerStats;

    [SerializeField] private Transform _exitPortal;

    private void Awake()
    {
        _instance = this;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    private void Update()
    {
        move();
    }

    private void move()
    {
        if (_exitPortal == null)
        {
            clearAndHide();
            return;
        }

        if (_playerStats == null)
            _playerStats = PlayerStats.Instance;

        Vector3 directionVector = _exitPortal.position - _playerStats.transform.position;
        transform.position = _playerStats.transform.position + directionVector.normalized * _radiusFromPlayer;
    }

    public void SetExitPortalPosition(Transform exitPortalTransform)
    {
        if (exitPortalTransform == null)
            return;

        _spriteRenderer.enabled = true;
        _exitPortal = exitPortalTransform;
    }

    private void clearAndHide()
    {
        _exitPortal = null;
        _spriteRenderer.enabled = false;
    }
}
