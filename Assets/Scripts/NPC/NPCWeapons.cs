using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class NPCWeapons : MonoBehaviour
{
    public static event Action<SFXClip> OnEnemyShootingAudio;

    private Transform _shootingSpot;
    private Animator _animator;
    private NPC_AI _npcAI;

    private string _removeWeaponTriggerName = "RemoveWeapon";

    private float _closeQuarterShooting = 0.75f;
    private float _shootingOffset = 0.5f;
    private float _timer = 0.0f;
    private float _autoWeaponTimer = 0.0f;
    private Vector2 _enemyShootingInterval = new Vector2(0.2f, 0.6f);
    private bool _autoShootingCoroutineRunning = false;

    [Header("Weapons, read-only")]
    [SerializeField] private WeaponItem _selectedWeapon;
    private bool _reloadActivated = false;
    [SerializeField] private float _reloadTime = 0.0f;

    private Transform _shootTarget;
    public bool HasShootingTarget => _shootTarget != null;
    private GameAssets _gameAssets;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _shootingSpot = transform.Find("ShootingSpot");
        _npcAI = GetComponent<NPC_AI>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        chooseEnemysWeapon();
    }

    private void chooseEnemysWeapon()
    {
        if (GameAssets.Instance == null)
            return;

        int randomWeaponIndex = UnityEngine.Random.Range(0, 12);

        if (randomWeaponIndex < 4)
        {
            //Gun - 4/12 chance to spawn (0, 1, 2, 3)
            _selectedWeapon = GameAssets.Instance.AvailableWeaponsList[0];
        }
        else if (randomWeaponIndex >= 4 && randomWeaponIndex < 8)
        {
            //Silencer - 4/12 chance to spawn (4, 5, 6, 7)
            _selectedWeapon = GameAssets.Instance.AvailableWeaponsList[1];
        }
        else if (randomWeaponIndex >= 8 && randomWeaponIndex < 10)
        {
            //Machine gun - 2/12 chance to spawn (8, 9)
            _selectedWeapon = GameAssets.Instance.AvailableWeaponsList[2];
        }
        else
        {
            //Shotgun - 2/12 chance to spawn (10, 11)
            _selectedWeapon = GameAssets.Instance.AvailableWeaponsList[3];
        }
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
            StopCoroutine("AutoShooting");
            return;
        }

        if (_npcAI.ObstaclesInTheWay(_shootTarget.position))
            return;

        _timer -= Time.deltaTime;
        _autoWeaponTimer -= Time.deltaTime;
        if (_timer < 0.0f)
        {
            if (_selectedWeapon.Automatic)
            {
                if (_autoWeaponTimer < 0.0f)
                {
                    if (!_autoShootingCoroutineRunning)
                    {
                        //Start automatic shooting
                        _autoShootingCoroutineRunning = true;
                        StartCoroutine(nameof(AutoShooting));
                    }
                    else
                    {
                        //Stop automatic shooting
                        StopCoroutine(nameof(AutoShooting));
                        _autoShootingCoroutineRunning = false;
                    }

                    //When either started or stopped auto shooting, reset the timer
                    _autoWeaponTimer = UnityEngine.Random.Range(_enemyShootingInterval.x, _enemyShootingInterval.y);
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
            _timer = UnityEngine.Random.Range(_enemyShootingInterval.x, _enemyShootingInterval.y);
        }

        //Reloading interval
        if (_reloadActivated)
        {
            _reloadTime += Time.deltaTime;

            if (_reloadTime >= _selectedWeapon.ShootInterval)
            {
                _reloadTime = 0.0f;
                _reloadActivated = false;
            }
        }
    }

    public void StopAttack()
    {
        _shootTarget = null;
        StopCoroutine(nameof(AutoShooting));
        _autoShootingCoroutineRunning = false;
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

            bullet.SetupBullet(direction, UnityEngine.Random.Range(_selectedWeapon.WeaponDamage.x, _selectedWeapon.WeaponDamage.y), gameObject.tag);
        }

        generateShootingParticleSystem();

        OnEnemyShootingAudio?.Invoke(_selectedWeapon.WeaponShootAudio);
    }

    public IEnumerator AutoShooting()
    {
        while (true)
        {
            shoot();

            yield return new WaitForSeconds(_selectedWeapon.ShootInterval);
        }
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
