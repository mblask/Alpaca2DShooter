using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum FiringTrapState
{
    Search,
    Attack,
    GoBack,
}

public class FiringTrap : MonoBehaviour, IDamagable
{
    public static event Action<SFXClip> OnWeaponShootingAudio;

    [SerializeField] private List<WeaponItem> _weaponList;
    [SerializeField] private bool _spawnRandomly;
    [SerializeField] private float _searchRadius;
    [SerializeField] private float _trackingRadius;

    private SpriteRenderer _weaponSpriteRenderer;

    [SerializeField] private bool _isWorking = true;

    private float _rotationSpeed = 1.0f;

    private float _stopwatch;
    private float _searchTime = 0.2f;

    private Transform _shootingSpot;
    private float _shootingOffset = 0.5f;
    private float _timer;
    private float _shootingInterval = 0.2f;

    private Transform _target;
    private Weapon _selectedWeapon;

    private FiringTrapState _trapState = FiringTrapState.Search;

    private void Awake()
    {
        _weaponSpriteRenderer = transform.Find("WeaponHolder").GetComponent<SpriteRenderer>();
        _shootingSpot = transform.Find("ShootingSpot");
    }

    private void Start()
    {
        if (_spawnRandomly)
        {
            if (0 == UnityEngine.Random.Range(0, 2))
                Destroy(gameObject);
        }

        WeaponItem randomWeapon = _weaponList[UnityEngine.Random.Range(0, _weaponList.Count)];
        _weaponSpriteRenderer.sprite = randomWeapon.ItemSprite;

        _selectedWeapon = new Weapon(randomWeapon, 0);
    }

    private void Update()
    {
        if (!_isWorking)
            return;

        switch (_trapState)
        {
            case FiringTrapState.Search:
                rotation2DTransform();
                searchForPlayer();
                break;
            case FiringTrapState.Attack:
                trackTarget();
                attackTarget();
                break;
            case FiringTrapState.GoBack:
                _trapState = FiringTrapState.Search;
                break;
        }        
    }

    public void DamageObject(float amount)
    {
        //non damagable object, though hittable
        //Particle System and/or Light up object
    }

    private void rotation2DTransform()
    {
        Vector3 rotationIncrement = Vector3.forward * _rotationSpeed * Mathf.Rad2Deg * Time.deltaTime;
        Vector3 finalRotation = transform.rotation.eulerAngles + rotationIncrement;
        transform.rotation = Quaternion.Euler(finalRotation);
    }

    private void searchForPlayer()
    {
        _stopwatch += Time.deltaTime;

        if (_stopwatch >= _searchTime)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _searchRadius);
            foreach (Collider2D collider in colliders)
            {
                PlayerStats playerStats = collider.gameObject.GetComponent<PlayerStats>();

                if (playerStats == null)
                    continue;

                if (playerStats.IsAlive())
                {
                    _target = collider.transform;
                    _trapState = FiringTrapState.Attack;
                }
            }

            _stopwatch = 0.0f;
        }
    }

    private void trackTarget()
    {
        if (_target != null)
        {
            rotate2DTransformTowards(_target);

            float distance = Vector2.Distance(transform.position, _target.position);
            
            if (distance >= _trackingRadius)
            {
                _target = null;
                _trapState = FiringTrapState.GoBack;
            }
        }
    }

    private void attackTarget()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0.0f)
        {
            shoot();

            if (_selectedWeapon.WeaponItem.ShootInterval == 0.0f)
                _timer = _shootingInterval;
            else
                _timer = _selectedWeapon.WeaponItem.ShootInterval;
        }
    }

    private void shoot()
    {
        if (_target == null)
            return;

        int bulletAmount = 1;
        if (_selectedWeapon.WeaponItem.Trigger.Equals(AnimationType.Shotgun))
            bulletAmount = 5;

        for (int i = 0; i < bulletAmount; i++)
        {
            Vector2 direction = shootingDirection(_target.position);
            Transform bulletTransform = Instantiate(GameAssets.Instance.BulletPrefab, _shootingSpot.position, Quaternion.identity, null);
            Bullet bullet = bulletTransform.GetComponent<Bullet>();

            bullet.SetupBullet(direction, UnityEngine.Random.Range(_selectedWeapon.WeaponItem.WeaponDamage.x, _selectedWeapon.WeaponItem.WeaponDamage.y), gameObject.tag);
        }

        generateShootingParticleSystem();

        OnWeaponShootingAudio?.Invoke(_selectedWeapon.WeaponItem.WeaponShootAudio);
    }

    private Vector2 shootingDirection(Vector3 target)
    {
        if (target == null)
            return Vector2.zero;

        return (target + getRandomOffset(_shootingOffset, 1.0f) - _shootingSpot.position).normalized;
    }

    private Vector3 getRandomOffset(float shootingOffset, float accuracy)
    {
        Vector2 right = Vector2.right * UnityEngine.Random.Range(-1.0f, 1.0f);
        Vector2 up = Vector2.up * UnityEngine.Random.Range(-1.0f, 1.0f);

        Vector2 randomOffset = (right + up) * shootingOffset / accuracy;

        return randomOffset;
    }

    private void rotate2DTransformTowards(Transform target)
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    private void generateShootingParticleSystem()
    {
        Vector3 rotation = new Vector3(-1.0f * _shootingSpot.rotation.eulerAngles.z, 0.0f, 0.0f);

        ParticleSystem shootingPS = Instantiate(GameAssets.Instance.ShootingPS);
        shootingPS.transform.position = _shootingSpot.position;
        shootingPS.transform.Rotate(rotation);

        shootingPS.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
        Gizmos.DrawWireSphere(transform.position, _trackingRadius);
    }
}
