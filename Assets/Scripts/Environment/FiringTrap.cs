using UnityEngine;
using AlpacaMyGames;
using UnityEngine.Tilemaps;

public class FiringTrap : Hackable, IDamagable
{
    [SerializeField] private float _searchRadius;
    [SerializeField] private float _trackingRadius;

    private SpriteRenderer _weaponSpriteRenderer;

    [SerializeField] private bool _isWorking = true;
    private bool _turnedOn = true;
    private NPCAllegiance _allegiance;

    private float _rotationSpeed = 1.0f;

    private float _stopwatch;
    private float _searchTime = 0.2f;

    private Transform _shootingSpot;
    private float _shootingOffset = 0.5f;
    private float _timer;
    private float _shootingInterval = 0.2f;

    private Transform _target;
    private Weapon _selectedWeapon;

    private GameManager _gameManager;
    private AudioManager _audioManager;

    private FiringTrapState _trapState = FiringTrapState.Search;

    private void Awake()
    {
        _weaponSpriteRenderer = transform.Find("WeaponHolder").GetComponent<SpriteRenderer>();
        _shootingSpot = transform.Find("ShootingSpot");
        _allegiance = NPCAllegiance.Enemy;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        WeaponItem randomWeapon = GameAssets.Instance.WeaponsList.GetRandomElement();
        _weaponSpriteRenderer.sprite = randomWeapon.ItemSprite;

        _selectedWeapon = new Weapon(randomWeapon, 0);
    }

    private void Update()
    {
        if (!_gameManager.IsGameRunning())
            return;

        if (!_isWorking)
            return;

        if (!_turnedOn)
            return;

        if (_target == null)
            _trapState = FiringTrapState.Search;

        switch (_trapState)
        {
            case FiringTrapState.Search:
                rotation2DTransform();
                searchForTarget();
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

    public void DamageObject(DamageData damageData)
    {
        //non damagable object, though hittable
        //Particle System and/or Light up object
    }

    public override void Hack()
    {
        _allegiance = _allegiance.Equals(NPCAllegiance.Enemy) ? 
            NPCAllegiance.Ally : NPCAllegiance.Enemy;

        _trapState = FiringTrapState.Search;
    }

    public override void TurnOnOff()
    {
        _turnedOn = !_turnedOn;
    }

    private void rotation2DTransform()
    {
        Vector3 rotationIncrement = Vector3.forward * _rotationSpeed * Mathf.Rad2Deg * Time.deltaTime;
        Vector3 finalRotation = transform.rotation.eulerAngles + rotationIncrement;
        transform.rotation = Quaternion.Euler(finalRotation);
    }

    private void searchForTarget()
    {
        _stopwatch += Time.deltaTime;

        if (_stopwatch < _searchTime)
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _searchRadius);
        foreach (Collider2D collider in colliders)
        {
            if (_allegiance.Equals(NPCAllegiance.Enemy) && !grabPlayer(collider))
                continue;

            if (_allegiance.Equals(NPCAllegiance.Ally) && !grabEnemy(collider))
                continue;
        }

        _stopwatch = 0.0f;
    }

    private bool grabPlayer(Collider2D collider)
    {
        PlayerStats playerStats = collider.gameObject.GetComponent<PlayerStats>();

        if (playerStats == null)
            return false;

        if (!playerStats.IsAlive())
            return false;

        if (obstacleInTheWay(playerStats.transform))
            return false;

        _target = collider.transform;
        _trapState = FiringTrapState.Attack;

        return true;
    }

    private bool grabEnemy(Collider2D collider)
    {
        NPCStats npcStats = collider.gameObject.GetComponent<NPCStats>();

        if (npcStats == null)
            return false;

        if (!npcStats.IsAlive())
            return false;

        if (obstacleInTheWay(npcStats.transform))
            return false;

        _target = collider.transform;
        _trapState = FiringTrapState.Attack;

        return true;
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
        if (obstacleInTheWay(_target))
            return;

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

    private bool obstacleInTheWay(Transform target)
    {
        if (target == null)
            return false;

        Vector3 direction = (target.position - transform.position).normalized;
        float offsetFromOrigin = 1.0f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + direction * offsetFromOrigin, direction, 999.0f);

        if (hits == null)
            return false;

        if (hits.Length == 0)
            return false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;
            
            if (hit.collider.GetComponent<TilemapCollider2D>() != null)
                return true;

            if (_allegiance.Equals(NPCAllegiance.Enemy) && hit.collider.GetComponent<PlayerController>() != null)
                return false;

            if (_allegiance.Equals(NPCAllegiance.Ally) && hit.collider.GetComponent<NPCBase>() != null)
                return false;

            if (hit.collider.GetComponent<BaseCollider>() != null)
                return true;
        }

        return false;
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

            DamageData damageData = 
                new DamageData { Damage = _selectedWeapon.WeaponItem.WeaponDamage.GetRandom() };
            bullet.SetupBullet(direction, damageData, gameObject.tag);
        }

        generateShootingParticleSystem();

        _audioManager.PlayClip(_selectedWeapon.WeaponItem.WeaponShootAudio);
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

        if (GameAssets.Instance.ShootingPS == null)
            return;

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
