using UnityEngine;

public abstract class AbstractGranade : MonoBehaviour
{
    [SerializeField] protected ThrowableItem _item;
    public ThrowableItem ThrowableItem => _item;

    protected Rigidbody2D _rigidbody;
    protected Collider2D _collider;
    protected SpriteRenderer _spriteRenderer;

    protected bool _armed = false;
    protected float _explosionTime = 1.0f;
    protected float _timer = 0.0f;
    protected float _explosionRadius = 3.0f;
    protected float _friction = 0.995f;
    protected float _thresholdVelocity = 0.05f;

    protected GameAssets _gameAssets;
    protected CameraController _camera;

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
        _camera = CameraController.Instance;
    }

    private void Update()
    {
        float rotationSpeed = 2.0f;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 
            transform.rotation.eulerAngles.z + rotationSpeed * Mathf.Rad2Deg * Time.deltaTime);

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

    protected abstract void explode();
}