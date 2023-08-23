using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private ThrowableItem _item;
    public ThrowableItem ThrowableItem => _item;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private bool _armed = false;
    private float _explosionTime = 1.0f;
    private float _timer = 0.0f;
    private float _explosionRadius = 3.0f;
    private float _friction = 0.995f;
    private float _thresholdVelocity = 0.05f;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 0.0f;
        _rigidbody.mass = 5.0f;
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = !_armed;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
    }

    private void Update()
    {
        if (_rigidbody.velocity.sqrMagnitude > 0.0f)
        {
            _rigidbody.velocity *= _friction;
        
            if (_rigidbody.velocity.sqrMagnitude < _thresholdVelocity)
                _rigidbody.velocity = Vector2.zero;
        }

        if (!_armed)
            return;

        _timer += Time.deltaTime;
        if (_timer >= _explosionTime)
            explode();
    }

    public void SetItem(ThrowableItem throwableItem)
    {
        _item = throwableItem;
        _spriteRenderer.color = _item.Color;
    }

    public void ArmGrenade()
    {
        _armed = true;
        _collider.isTrigger = false;

        PickupItem pickupItem = GetComponent<PickupItem>();
        if (pickupItem != null)
            pickupItem.SetPickable(false);
    }

    public void ThrowGrenade(Vector2 forceVector)
    {
        _rigidbody.AddForce(forceVector, ForceMode2D.Impulse);
    }

    private void explode()
    {
        DestructionArea destructionArea = Instantiate(_gameAssets.DestructionArea, transform.position, Quaternion.identity, null).GetComponent<DestructionArea>();
        destructionArea.SetDestructionRadius(_explosionRadius);
        destructionArea.SetDamage(_item.WeaponDamage);
        
        Destroy(gameObject);
    }
}
