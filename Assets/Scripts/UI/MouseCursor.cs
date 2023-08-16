using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    private const string GUN_NAME = "Gun";
    private const string SILENCED_GUN_NAME = "Silenced Gun";
    private const string MACHINEGUNE_NAME = "Machine Gun";
    private const string SHOTGUN_NAME = "Shotgun";

    private static MouseCursor _instance;
    public static MouseCursor Instance
    {
        get
        {
            return _instance;
        }
    }

    private float _maxPossibleCursorScale = 2.0f;
    [SerializeField] private float _startingCrosshairScale = 1.75f;

    private Camera _camera;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _camera = Camera.main;
        transform.localScale = Vector3.one * _startingCrosshairScale;

        if (PlayerWeapons.Instance != null)
            PlayerWeapons.Instance.OnShooting += onShooting;
    }

    private void LateUpdate()
    {
        cursorPosition();
        resizeCursor();
    }

    private void OnDisable()
    {
        if (PlayerWeapons.Instance != null)
            PlayerWeapons.Instance.OnShooting -= onShooting;
    }

    private void onShooting(PlayerWeapons playerWeapons)
    {
        Weapon weapon = playerWeapons.CurrentWeapon;
        float cursorResizeFactor = 1.0f;

        switch (weapon.WeaponItem.ItemName)
        {
            case GUN_NAME:
            case SILENCED_GUN_NAME:
                cursorResizeFactor = 1.1f;
                break;
            case MACHINEGUNE_NAME:
                cursorResizeFactor = 1.05f;
                break;
            case SHOTGUN_NAME:
                cursorResizeFactor = 1.15f;
                break;
            default:
                break;
        }

        changeCursorSize(cursorResizeFactor);
    }

    private void cursorPosition()
    {
        Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    private void changeCursorSize(float sizeIncrease)
    {
        Vector2 localScale = transform.localScale;

        if (localScale.x < _maxPossibleCursorScale)
        {
            localScale *= sizeIncrease;
            transform.localScale = localScale;
        }
    }

    private void resizeCursor()
    {
        float localXScale = transform.localScale.x;
        float resizeSpeed = 1.2f;

        if (localXScale > _startingCrosshairScale)
        {
            Vector2 localScale = transform.localScale;
            localScale -= Vector2.one * resizeSpeed * Time.deltaTime;
            transform.localScale = localScale;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _startingCrosshairScale / 1.5f);
    }
}