using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private ThrowableItem _item;
    public ThrowableItem ThrowableItem => _item;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;

    private bool _armed = false;
    private Color _armedColor = new Color(0.6f, 0.0f, 0.0f);
    private Color _nonArmedColor = new Color(0.3f, 0.5f, 0.5f);

    private GameAssets _gameAssets;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0.0f;
        _rigidbody.mass = 5.0f;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = _nonArmedColor;
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        startSetup(_armed);
    }

    private void Update()
    {
        if (_rigidbody.velocity.sqrMagnitude > 0)
        {
            _rigidbody.velocity *= 0.99f;
            if (_rigidbody.velocity.sqrMagnitude < 0.05f)
            {
                _rigidbody.velocity = Vector2.zero;
            }
        }
    }

    private void startSetup(bool isArmed)
    {
        _spriteRenderer.color = isArmed ? _armedColor : _nonArmedColor;
        PickupItem pickupItem = GetComponent<PickupItem>();
        if (pickupItem != null)
            pickupItem.SetPickable(!isArmed);
    }

    public void SetItem(ThrowableItem throwableItem)
    {
        _item = throwableItem;
    }

    public void ArmMine()
    {
        _armed = true;
        _spriteRenderer.color = _armedColor;

        PickupItem pickupItem = GetComponent<PickupItem>();
        if (pickupItem != null)
            pickupItem.SetPickable(false);
    }

    public void ThrowMine(Vector2 forceVector)
    {
        _rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable == null)
            return;

        if (!_armed)
            return;

        DestructionArea destructionArea = Instantiate(_gameAssets.DestructionArea, transform.position, Quaternion.identity, null).GetComponent<DestructionArea>();
        destructionArea.SetDamage(_item.WeaponDamage);

        Destroy(gameObject);
    }
}
