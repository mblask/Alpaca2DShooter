using UnityEngine;

public class Mine : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private bool _armed = false;
    [SerializeField] private WeaponItem _mine;

    private Color _armedColor = new Color(0.6f, 0.0f, 0.0f);
    private Color _nonArmedColor = new Color(0.3f, 0.5f, 0.5f);

    private GameAssets _gameAssets;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = _nonArmedColor;
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;

        startSetup(_armed);
    }

    private void startSetup(bool isArmed)
    {
        _spriteRenderer.color = isArmed ? _armedColor : _nonArmedColor;
        PickupItem pickupItem = GetComponent<PickupItem>();
        if (pickupItem != null)
            pickupItem.SetPickable(!isArmed);
    }

    public void ArmMine()
    {
        _armed = true;
        _spriteRenderer.color = _armedColor;

        PickupItem pickupItem = GetComponent<PickupItem>();
        if (pickupItem != null)
            pickupItem.SetPickable(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable == null)
            return;

        if (!_armed)
            return;

        DestructionArea destructionArea = Instantiate(_gameAssets.DestructionArea, transform.position, Quaternion.identity, null).GetComponent<DestructionArea>();
        destructionArea.SetDamage(_mine.WeaponDamage);

        Destroy(gameObject);
    }
}
