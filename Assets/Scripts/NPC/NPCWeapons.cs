using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

public class NPCWeapons : MonoBehaviour
{
    private Transform _shootingSpot;
    private Animator _animator;
    private NpcAi _npcAI;
    private NPCBase _npcBase;

    private string _removeWeaponTriggerName = "RemoveWeapon";

    private float _closeQuarterShooting = 0.75f;
    private float _shootingOffset = 0.5f;
    private float _timer = 0.0f;
    private float _autoWeaponTimer = 0.0f;
    private Vector2 _enemyShootingInterval = new Vector2(0.2f, 0.6f);
    private bool _autoShooting = false;
    private float _autoShootingTimer = 0.0f;
    private float _damageMultiplierBase = 0.5f;
    private float _damageMultiplier;

    [SerializeField] private ThrowableItem _throwable;

    [Header("Weapons, read-only")]
    [SerializeField] private WeaponItem _selectedWeapon;
    private float _shootingIntervalMultiplier = 1.0f;
    private TimerObject _shootingIntervalMultiplierTimer = new TimerObject();
    private bool _reloadActivated = false;
    [SerializeField] private float _reloadTime = 0.0f;

    private Transform _shootTarget;
    public bool HasShootingTarget => _shootTarget != null;

    private LevelsManager _levelsManager;
    private GameAssets _gameAssets;
    private AudioManager _audioManager;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _shootingSpot = transform.Find("ShootingSpot");
        _npcAI = GetComponent<NpcAi>();
        _npcBase = GetComponent<NPCBase>();
    }

    private void Start()
    {
        _levelsManager = LevelsManager.Instance;
        _gameAssets = GameAssets.Instance;
        _audioManager = AudioManager.Instance;
        chooseWeapon();
        setDamageMultiplier();
    }

    private void Update()
    {
        autoShootingRoutine();
        shootingIntervalTimerProcess();
    }

    private void shootingIntervalTimerProcess()
    {
        if (_shootingIntervalMultiplierTimer.IsOver)
            return;

        if (_shootingIntervalMultiplierTimer.Update())
            _shootingIntervalMultiplier = 1.0f;
    }

    public void InitializeBossWeapon(WeaponItem weaponOfChoice)
    {
        int chanceForWeaponOfChoice = 75;
        bool randomChance = Utilities.ChanceFunc(chanceForWeaponOfChoice);

        if (randomChance)
            _selectedWeapon = weaponOfChoice;
        else
            chooseWeapon();
    }

    private void chooseWeapon()
    {
        if (_levelsManager == null)
            _levelsManager = LevelsManager.Instance;

        List<WeaponItem> availableWeapons = _levelsManager.GetAvailableNpcWeapons();

        if (availableWeapons.Count == 1)
        {
            _selectedWeapon = availableWeapons[0];
            return;
        }

        List<WeaponItem> itemPool = new List<WeaponItem>();
        for (int i = 0; i < availableWeapons.Count; i++)
        {
            float weaponChance = availableWeapons[i].UseChance;
            int sampleSize = 5;
            for (int j = 0; j < sampleSize; j++)
            {
                if (!Utilities.ChanceFunc(weaponChance))
                    continue;

                itemPool.Add(availableWeapons[i]);
            }
        }

        if (itemPool.Count == 0)
            itemPool = availableWeapons;

        _selectedWeapon = itemPool.GetRandomElement();
    }

    public void PresentWeapon(bool presentWeapon)
    {
        if (_selectedWeapon == null)
            return;

        string weaponTrigger = "Equip" + _selectedWeapon.Trigger.ToString();

        if (presentWeapon)
            _animator.SetTrigger(weaponTrigger);
        else
            _animator.SetTrigger(_removeWeaponTriggerName);
    }

    public void AttackTarget(Transform target)
    {
        _shootTarget = target;

        if (_shootTarget == null)
        {
            _autoShooting = false;
            return;
        }

        if (_npcAI.ObstaclesInRaycast(_shootTarget.position))
            return;

        _timer -= Time.deltaTime;
        _autoWeaponTimer -= Time.deltaTime;
        if (_timer < 0.0f)
        {
            if (_selectedWeapon.Automatic)
            {
                if (_autoWeaponTimer < 0.0f)
                {
                    if (!_autoShooting)
                    {
                        //Start automatic shooting
                        _autoShooting = true;
                    }
                    else
                    {
                        //Stop automatic shooting
                        _autoShooting = false;
                    }

                    //When either started or stopped auto shooting, reset the timer
                    _autoWeaponTimer = Random.Range(_enemyShootingInterval.x, _enemyShootingInterval.y);
                }
            }
            else
            {
                //If item has a reloading interval - e.g. shotgun
                if (_selectedWeapon.ShootInterval > 0.0f)
                {
                    if (!_reloadActivated)
                    {
                        _reloadActivated = true;
                        shoot();
                    }
                }
                else
                {
                    //Shoot non automatic
                    shoot();
                }
            }

            //Reset timer when a shot is made
            _timer = Random.Range(_enemyShootingInterval.x, _enemyShootingInterval.y);
        }

        //Reloading interval
        if (_reloadActivated)
        {
            _reloadTime += Time.deltaTime;

            if (_reloadTime >= getTotalShootingInterval())
            {
                _reloadTime = 0.0f;
                _reloadActivated = false;
            }
        }
    }

    public void SetTemporaryShootingMultiplier(float multiplier, float duration)
    {
        _shootingIntervalMultiplierTimer = new TimerObject(duration);
        _shootingIntervalMultiplier = multiplier;
    }

    private float getTotalShootingInterval()
    {
        return _selectedWeapon.ShootInterval * _shootingIntervalMultiplier;
    }

    public void StopAttack()
    {
        _shootTarget = null;
        _autoShooting = false;
    }

    public void ThrowThrowable(Transform target)
    {
        _shootTarget = target;

        if (_npcAI.ObstaclesInRaycast(_shootTarget.position))
            return;

        Grenade grenade =
            Instantiate(_gameAssets.Grenade, transform.position, Quaternion.identity, null)
            .GetComponent<Grenade>();

        grenade.SetItem(_throwable);
        grenade.ArmGrenade();

        Vector3 throwDirection = (Vector3)Utilities.GetVectorFromAngle(transform.rotation.eulerAngles.z);
        grenade.ThrowGrenade(_throwable.ThrowForce * throwDirection);
    }

    private void setDamageMultiplier()
    {
        if (_npcBase.EnemyType.Equals(NPCEnemyType.Boss))
        {
            _damageMultiplier = 0.75f;
            return;
        }

        int currentLevel = _levelsManager.GetLevelNumber();
        float damageMultiplierIncrement = 0.03f;
        _damageMultiplier = _damageMultiplierBase + damageMultiplierIncrement * (currentLevel - 1);
    }

    private void shoot()
    {
        if (_shootTarget == null)
            return;

        int bulletAmount = 1;
        if (_selectedWeapon.Trigger.Equals(AnimationType.Shotgun))
            bulletAmount = 5;

        for (int i = 0; i < bulletAmount; i++)
        {
            Vector2 direction = shootingDirection(_shootTarget.position);
            Transform bulletObject = Instantiate(_gameAssets.BulletPrefab, _shootingSpot.position, Quaternion.identity, null);
            Bullet bullet = bulletObject.GetComponent<Bullet>();

            DamageData damageData = new DamageData 
                { Damage = _selectedWeapon.WeaponDamage.GetRandom() * _damageMultiplier, BleedingChance = 0.0f };
            bullet.SetupBullet(direction, damageData, gameObject.tag);
        }

        generateShootingParticleSystem();

        _audioManager.PlayClip(_selectedWeapon.WeaponShootAudio);
    }

    private void autoShootingRoutine()
    {
        if (!_autoShooting)
            return;

        _autoShootingTimer -= Time.deltaTime;
        if (_autoShootingTimer > 0.0f)
            return;

        shoot();
        _autoShootingTimer = _selectedWeapon.ShootInterval;
    }

    private Vector2 shootingDirection(Vector3 target)
    {
        if (target == null)
            return Vector2.zero;

        float distanceFromShootingSpot = ((Vector2)target - (Vector2)_shootingSpot.position).magnitude;

        if (distanceFromShootingSpot < _closeQuarterShooting)
            return (target + getRandomOffset(_shootingOffset, 1.0f) - transform.position).normalized;
        else
            return (target + getRandomOffset(_shootingOffset, 1.0f) - _shootingSpot.position).normalized;
    }

    private Vector3 getRandomOffset(float shootingOffset, float accuracy)
    {
        Vector2 right = Vector2.right * UnityEngine.Random.Range(-1.0f, 1.0f);

        Vector2 up = Vector2.up * UnityEngine.Random.Range(-1.0f, 1.0f);

        Vector2 randomOffset = (right + up) * _shootingOffset / accuracy;

        return randomOffset;
    }

    private void generateShootingParticleSystem()
    {
        Vector3 rotation = new Vector3(-1.0f * _shootingSpot.rotation.eulerAngles.z, 0.0f, 0.0f);

        if (_gameAssets.ShootingPS == null)
            return;

        ParticleSystem shootingPS = Instantiate(_gameAssets.ShootingPS);
        shootingPS.transform.position = _shootingSpot.position;
        shootingPS.transform.Rotate(rotation);

        shootingPS.Play();
    }

    public WeaponItem GetSelectedWepon()
    {
        return _selectedWeapon;
    }
}
