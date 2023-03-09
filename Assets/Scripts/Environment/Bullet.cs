using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public static event Action<SFXClip> OnBulletHitsCharacter;

    private float _bulletSpeed = 25.0f;
    private float _destroyAfter = 2.0f;

    private Rigidbody2D _rigidbody;
    private Vector2 _bulletDirection;
    private float _bulletDamage;
    private string _shooterTag;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = _bulletSpeed * _bulletDirection;
        Destroy(gameObject, _destroyAfter);
    }

    public void SetupBullet(Vector3 direction, float damage, string shooterTag)
    {
        _bulletDirection = direction;
        _bulletDamage = damage;

        if (shooterTag.Equals(""))
        {
            _shooterTag = Constants.EnemyTag;
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
            damagable.DamageObject(_bulletDamage);
            OnBulletHitsCharacter?.Invoke(SFXClip.BulletHitsCharacter);

            Destroy(gameObject);
        }
    }
}
