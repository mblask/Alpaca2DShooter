using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    public event Action<int, int> OnAmmoPanelUIChanged;
    public event Action<PlayerWeapons> OnWeaponChanged;
    public event Action<PlayerWeapons> OnShooting;
    public event Action<SFXClip> OnShootingAudio;
    public event Action<SFXClip> OnReloadingAudio;
    public event Action<float> OnEnemyHit;

    private static PlayerWeapons _instance;
    public static PlayerWeapons Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("Items")]
    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();

    [Header("Shooting settings - Read-only")]
    [SerializeField] private float _nonShootingDistance = 1.25f;
    [SerializeField] private float _closeQuarterShooting = 0.75f;
    [SerializeField] private float _shootingOffset = 0.5f;

    [Header("Camera shake - Read-only")]
    [SerializeField] private float _cameraShakeDuration = 0.05f;
    [SerializeField] private float _cameraShakeMagnitude = 0.1f;

    private Transform _shootingSpot;
    private Camera _camera;
    private ChatBubbleSpawner _chatBubbleSpawner;
    private GameManager _gameManager;
    private PlayerAnimations _playerAnimations;
    private PlayerStats _playerStats;
    private CameraController _cameraController;

    private Vector2 _mousePosition;

    private Weapon _currentWeapon;
    public Weapon CurrentWeapon
    {
        get
        {
            return _currentWeapon;
        }
    }

    private bool _weaponEquipped = false;
    private bool _canShoot = true;
    private bool _isShooting = false;
    private bool _canSwitchWeapons = true;
    private bool _canPutWeaponAway = true;
    private bool _firstTimeSetting = true;

    private float _shootingInterval;
    private bool _intervalWeaponActivated = true;

    private int _currentAmmo;
    private int _shotsFired;
    private int _shotsHit;

    public void Awake()
    {
        _instance = this;

        _shootingSpot = transform.Find("ShootingSpot");
        _playerAnimations = GetComponent<PlayerAnimations>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        _camera = Camera.main;
        _chatBubbleSpawner = ChatBubbleSpawner.Instance;
        _gameManager = GameManager.Instance;
        _cameraController = CameraController.Instance;

        NPCStats.OnHit += IncrementShotsHit;

        if (_weapons.Count > 0)
            SetWeapon(0);
    }

    private void OnDisable()
    {
        NPCStats.OnHit -= IncrementShotsHit;
    }

    private void Update()
    {
        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        weaponHandling();
    }

    private void weaponHandling()
    {
        if (_playerStats == null || _gameManager == null)
            return;

        if (!_playerStats.IsAlive() || !_gameManager.IsGameRunning() || _gameManager.IsPaused())
            return;

        if (Input.GetKeyDown(KeyCode.R) && _weaponEquipped)
        {
            ReloadWeapon();
        }

        if (Input.GetKeyDown(KeyCode.E) && _canSwitchWeapons)
        {
            switchWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Q) && _canSwitchWeapons)
        {
            switchWeapon(-1);
        }

        if (Input.GetKeyDown(KeyCode.F) && _canPutWeaponAway)
        {
            if (_weapons.Count > 0)
            {
                _weaponEquipped = !_weaponEquipped;
                presentWeapon();
            }
        }

        triggerShooting();
    }

    private void triggerShooting()
    {
        if (_currentWeapon == null)
            return;

        if (!_weaponEquipped)
            return;

        if (Input.GetMouseButtonDown(0) && _canShoot)
        {
            _isShooting = true;
            _canSwitchWeapons = false;
            _canPutWeaponAway = false;

            if (_currentWeapon.WeaponItem.Automatic)
                StartCoroutine(AutoShooting());
            else
            {
                if (_currentWeapon.WeaponItem.ShootInterval > 0.0f)
                {
                    if (!_intervalWeaponActivated)
                    {
                        _intervalWeaponActivated = ShootWeapon();
                        _shootingInterval = 0.0f;
                    }
                } 
                else
                    ShootWeapon();
            }
        }

        if (_intervalWeaponActivated)
        {
            _shootingInterval += Time.deltaTime;

            if (_shootingInterval >= _currentWeapon.WeaponItem.ShootInterval)
            {
                _intervalWeaponActivated = false;
                _shootingInterval = 0.0f;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isShooting = false;
            _canSwitchWeapons = true;
            _canPutWeaponAway = true;
            StopCoroutine(AutoShooting());
        }
    }

    public IEnumerator AutoShooting()
    {
        while (_isShooting)
        {
            ShootWeapon();

            yield return new WaitForSeconds(_shootingInterval);
        }
    }

    public static void EnableShootingStatic(bool value = true)
    {
        _instance.enableShooting(value);
    }

    private void enableShooting(bool value = true)
    {
        _canShoot = value;
    }

    private void presentWeapon()
    {
        if (_weapons.Count == 0)
            return;

        if (_weaponEquipped)
            _playerAnimations.PlayAnimation(_currentWeapon.WeaponItem.Trigger);
        else
            _playerAnimations.PlayAnimation(AnimationType.RemoveWeapon);
    }

    private void switchWeapon(int leftRight)
    {
        if (_weapons.Count <= 1)
            return;

        int currentIndex = _weapons.IndexOf(_currentWeapon);

        if (leftRight == -1)
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = _weapons.Count - 1;
        }

        if (leftRight == 1)
        {
            currentIndex++;

            if (currentIndex == _weapons.Count)
                currentIndex = 0;
        }

        SetWeapon(currentIndex);

        OnWeaponChanged?.Invoke(this);
    }

    public void SetWeapon(int index = 0)
    {
        if (index < 0)
            return;

        if (_currentWeapon != null)
            _currentWeapon.TotalAmmo += _currentAmmo;

        if (_playerStats.PlayerDamage.GetFinalValue() != 0)
            _playerStats.PlayerDamage.RemoveModifier((_currentWeapon.WeaponItem.WeaponDamage.x + _currentWeapon.WeaponItem.WeaponDamage.y) / 2.0f);

        _currentWeapon = _weapons[index];

        int ammoDelta = _currentWeapon.TotalAmmo - _currentWeapon.WeaponItem.MagazineBullets;

        if (ammoDelta >= 0)
        {
            _currentAmmo = _currentWeapon.WeaponItem.MagazineBullets;
            _currentWeapon.TotalAmmo = ammoDelta;
        }
        else
        {
            _currentAmmo = _currentWeapon.TotalAmmo;
            _currentWeapon.TotalAmmo = 0;
        }

        if (_firstTimeSetting)
        {
            _currentWeapon.Durability = _currentWeapon.WeaponItem.MaxDurability;
            _firstTimeSetting = false;
        }

        _shootingInterval = _currentWeapon.WeaponItem.ShootInterval;

        _playerStats.PlayerDamage.AddModifier((_currentWeapon.WeaponItem.WeaponDamage.x + _currentWeapon.WeaponItem.WeaponDamage.y) / 2.0f);

        OnAmmoPanelUIChanged?.Invoke(_currentAmmo, _currentWeapon.TotalAmmo);

        if (_weaponEquipped)
            presentWeapon();
    }

    public void IncrementShotsHit()
    {
        _shotsHit++;

        OnEnemyHit?.Invoke((float)_shotsHit / _shotsFired);
    }

    public int GetShotsHit()
    {
        return _shotsHit;
    }

    public float GetAccuracy()
    {
        if (_shotsFired == 0)
            return 0.0f;
        else
            return (float)_shotsHit / _shotsFired;
    }

    public bool ShootWeapon()
    {
        if (_currentAmmo == 0)
        {
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Out of ammo!", Color.white);
            return false;
        }

        int bulletAmount = 1;
        if (_currentWeapon.WeaponItem.Trigger.Equals(AnimationType.Shotgun))
            bulletAmount = 5;

        for (int i = 0; i < bulletAmount; i++)
        {
            Vector2 direction = shootingDirection(_mousePosition);

            if (direction == Vector2.zero)
                return false;

            Transform bulletObject = Instantiate(GameAssets.Instance.BulletPrefab, _shootingSpot.position, Quaternion.identity, null);

            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.SetupBullet(direction, UnityEngine.Random.Range(_currentWeapon.WeaponItem.WeaponDamage.x, _currentWeapon.WeaponItem.WeaponDamage.y), gameObject.tag);

            _shotsFired++;
        }

        _cameraController?.ShakeCamera(_cameraShakeDuration, _cameraShakeMagnitude);

        generateShootingParticleSystem();

        _currentAmmo--;

        if (_currentAmmo < 0)
            _currentAmmo = 0;

        OnShooting?.Invoke(this);
        OnShootingAudio?.Invoke(_currentWeapon.WeaponItem.WeaponShootAudio);

        OnAmmoPanelUIChanged?.Invoke(_currentAmmo, _currentWeapon.TotalAmmo);

        return true;
    }

    private Vector2 shootingDirection(Vector3 targetPosition)
    {
        float distanceFromShootingSpot = ((Vector2)targetPosition - (Vector2)_shootingSpot.position).magnitude;

        float distanceFromGameObject = ((Vector2)targetPosition - (Vector2)transform.position).magnitude;

        if (distanceFromGameObject < _nonShootingDistance)
            return Vector2.zero;

        Vector2 direction = new Vector2();

        if (distanceFromShootingSpot < _closeQuarterShooting)
            direction = (targetPosition + getRandomOffset(_shootingOffset, _playerStats.PlayerAccuracy.GetFinalValue()) - transform.position).normalized;
        else
            direction = (targetPosition + getRandomOffset(_shootingOffset, _playerStats.PlayerAccuracy.GetFinalValue()) - _shootingSpot.position).normalized;

        return direction;
    }

    private Vector3 getRandomOffset(float shootingOffset, float accuracy)
    {
        Vector2 right = Vector2.right * UnityEngine.Random.Range(-1.0f, 1.0f);

        Vector2 up = Vector2.up * UnityEngine.Random.Range(-1.0f, 1.0f);

        Vector2 randomOffset = (right + up) * shootingOffset / accuracy;

        return randomOffset;
    }

    public void ReloadWeapon()
    {
        if (_currentWeapon == null)
            return;

        string weaponLoadedString = "Weapon fully loaded!";
        string noMoreAmmoString = "No more ammo!";


        if (_currentAmmo == _currentWeapon.WeaponItem.MagazineBullets)
        {
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, weaponLoadedString, Color.white);
            return;
        }

        if (_currentWeapon.TotalAmmo == 0)
        {
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, noMoreAmmoString, Color.white);
            return;
        }

        if (_currentAmmo == 0)
        {
            int ammoDelta = _currentWeapon.TotalAmmo - _currentWeapon.WeaponItem.MagazineBullets;

            if (ammoDelta >= 0)
            {
                _currentWeapon.TotalAmmo -= _currentWeapon.WeaponItem.MagazineBullets;
                _currentAmmo = _currentWeapon.WeaponItem.MagazineBullets;
            }
            else
            {
                _currentAmmo = _currentWeapon.TotalAmmo;
                _currentWeapon.TotalAmmo = 0;
            }
        }

        if (_currentAmmo > 0 && _currentAmmo < _currentWeapon.WeaponItem.MagazineBullets)
        {
            int ammoNeeded = _currentWeapon.WeaponItem.MagazineBullets - _currentAmmo;

            int ammoDelta = _currentWeapon.TotalAmmo - ammoNeeded;

            if (ammoDelta >= 0)
            {
                _currentAmmo = _currentWeapon.WeaponItem.MagazineBullets;
                _currentWeapon.TotalAmmo -= ammoNeeded;
            }
            else
            {
                _currentAmmo += _currentWeapon.TotalAmmo;
                _currentWeapon.TotalAmmo = 0;
            }
        }

        OnReloadingAudio?.Invoke(_currentWeapon.WeaponItem.WeaponReloadAudio);
        OnAmmoPanelUIChanged?.Invoke(_currentAmmo, _currentWeapon.TotalAmmo);

        _playerAnimations.PlayAnimation(AnimationType.Reload);
        presentWeapon();
    }

    private void generateShootingParticleSystem()
    {
        Vector3 rotation = new Vector3(-1.0f * _shootingSpot.rotation.eulerAngles.z, 0.0f, 0.0f);

        ParticleSystem shootingPS = Instantiate(GameAssets.Instance.ShootingPS);
        shootingPS.transform.position = _shootingSpot.position;
        shootingPS.transform.Rotate(rotation);

        shootingPS.Play();
    }

    public void AddWeapon(Weapon weapon)
    {
        if (weapon == null)
            return;

        for (int i = 0; i < _weapons.Count; i++)
        {
            if (weapon.WeaponItem == _weapons[i].WeaponItem)
            {
                _weapons[i].TotalAmmo += UnityEngine.Random.Range(0, weapon.TotalAmmo);

                OnAmmoPanelUIChanged?.Invoke(_currentAmmo, _currentWeapon.TotalAmmo);

                return;
            }
        }

        _weapons.Add(weapon);

        if (_currentWeapon == null)
        {
            _currentWeapon = weapon;
            OnWeaponChanged?.Invoke(this);
            SetWeapon(0);
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    public int GetCurrentAmmo()
    {
        return _currentAmmo;
    }

    public bool IsWeaponEquipped()
    {
        return _weaponEquipped;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nonShootingDistance);
    }
}
