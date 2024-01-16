using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    private float _bulletSpeed = 25.0f;
    private float _destroyAfter = 2.0f;

    private Rigidbody2D _rigidbody;
    private Vector2 _bulletDirection;
    private DamageData _bulletDamageData;
    private float _bulletDamage;
    private string _shooterTag;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _rigidbody.velocity = _bulletSpeed * _bulletDirection;
        Destroy(gameObject, _destroyAfter);
    }

    public void SetupBullet(Vector3 direction, DamageData damageData, string shooterTag)
    {
        _bulletDirection = direction;
        _bulletDamageData = damageData;

        if (shooterTag.Equals("") || shooterTag.Equals("Untagged"))
        {
            _shooterTag = Constants.ENEMY_TAG;
            return;
        }

        _shooterTag = shooterTag;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //KEEP AN EYE ON THIS VERY UNFORTUNATE CHECKER!!
        if (collision.CompareTag(_shooterTag))
            return;

        if (collision.GetComponent<TilemapCollider2D>() != null)
            Destroy(gameObject);

        if (collision.GetComponent<Door>() != null && collision.GetComponent<Door>().IsClosed())
            Destroy(gameObject);

        IDamagable damagable = collision.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.DamageObject(_bulletDamageData);
            AudioManager.Instance.PlayClip(SFXClip.BulletHitsCharacter);

            Destroy(gameObject);
        }
    }
}
