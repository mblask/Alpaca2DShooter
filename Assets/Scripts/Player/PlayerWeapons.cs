using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

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
    [SerializeField] private List<Weapon> _throwables = new List<Weapon>();

    [Header("Shooting settings - Read-only")]
    [SerializeField] private float _nonShootingDistance = 1.00f;
    [SerializeField] private float _closeQuarterShooting = 0.75f;
    [SerializeField] private float _shootingOffset = 0.5f;

    [Header("Camera shake - Read-only")]
    [SerializeField] private float _cameraShakeDuration = 0.05f;
    [SerializeField] private float _cameraShakeMagnitude = 0.1f;

    private Transform _shootingSpot;
    private Camera _camera;
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
    private Weapon _currentThrowable;

    private bool _weaponEquipped = false;
    private bool _canShoot = true;
    private bool _isShooting = false;
    private bool _canSwitchWeapons = true;
    private bool _canPutWeaponAway = true;

    private float _shootingInterval;
    private bool _intervalWeaponActivated = false;

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
        _gameManager = GameManager.Instance;
        _cameraController = CameraController.Instance;

        NPCStats.OnHit += incrementShotsHit;

        weaponsStartSetup();
    }

    private void OnDisable()
    {
        NPCStats.OnHit -= incrementShotsHit;
    }

    private void Update()
    {
        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        weaponHandling();
        triggerShooting();
    }

    private void weaponsStartSetup()
    {
        if (_weapons.Count > 0)
            SetWeapon(0);

        if (_throwables.Count > 0)
            _currentThrowable = _throwables[0];
        
        ThrowableImage.UpdateThrowableUIStatic(_currentThrowable);
    }

    private void weaponHandling()
    {
        if (_playerStats == null || _gameManager == null)
            return;

        if (!_playerStats.IsAlive() || !_gameManager.IsGameRunning() || _gameManager.IsPaused())
            return;

        if (Input.GetKeyDown(KeyCode.R) && _weaponEquipped)
        {
            reloadWeapon();
        }

        if (Input.GetKeyDown(KeyCode.E) && _canSwitchWeapons)
        {
            switchWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Q) && _canSwitchWeapons)
        {
            switchWeapon(-1);
        }

        if (Input.GetKeyDown(KeyCode.Tab) && _canSwitchWeapons)
        {
            switchThrowables();
        }

        if (Input.GetKeyDown(KeyCode.F) && _canPutWeaponAway)
        {
            if (_weapons.Count == 0)
                return;

            _weaponEquipped = !_weaponEquipped;
            presentWeapon();
        }
    }

    private void triggerShooting()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            useThrowable();
        }

        if (_currentWeapon == null)
            return;

        if (!_weaponEquipped)
            return;

        if (Input.GetMouseButtonDown(0) && _canShoot)
        {
            _isShooting = true;
            _canSwitchWeapons = false;
            _canPutWeaponAway = false;

            evaluateWeaponTypesAndShoot();
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
            StopCoroutine(autoShootingCoroutine());
        }
    }

    private void evaluateWeaponTypesAndShoot()
    {
        if (_currentWeapon.WeaponItem.Automatic)
            StartCoroutine(autoShootingCoroutine());
        else
        {
            if (_currentWeapon.WeaponItem.ShootInterval > 0.0f)
            {
                if (!_intervalWeaponActivated)
                {
                    _intervalWeaponActivated = shootOnce();
                    _shootingInterval = 0.0f;
                }
            }
            else
                shootOnce();
        }
    }

    private void useThrowable()
    {
        if (_throwables.Count == 0)
        {
            string message = "I have nothing to throw!";
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, message, Color.white);
            return;
        }

        Vector3 throwPosition = transform.position + 2.0f * (Vector3)Utilities.GetVectorFromAngle(transform.rotation.eulerAngles.z);
        Mine mine = Instantiate(GameAssets.Instance.Mine, throwPosition, Quaternion.identity, null).GetComponent<Mine>();
        mine.ArmMine();

        _currentThrowable.TotalAmmo--;

        if (_currentThrowable.TotalAmmo == 0)
        {
            _throwables.Remove(_currentThrowable);
            _currentThrowable = null;
        }

        ThrowableImage.UpdateThrowableUIStatic(_currentThrowable);
    }

    private void switchThrowables()
    {
        if (_throwables.Count == 0)
            return;

        if (_currentThrowable == null)
            return;

        int currentIndex = _throwables.IndexOf(_currentThrowable);

        currentIndex++;

        if (currentIndex >= _throwables.Count)
            currentIndex = 0;

        _currentThrowable = _throwables[currentIndex];

        ThrowableImage.UpdateThrowableUIStatic(_currentThrowable);
    }

    private IEnumerator autoShootingCoroutine()
    {
        while (_isShooting)
        {
            shootOnce();
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
        {
            _currentWeapon.TotalAmmo += _currentAmmo;
            _currentAmmo = 0;
        }

        if (_playerStats.PlayerDamage.GetFinalValue() != 0)
            _playerStats.PlayerDamage.RemoveModifier(_currentWeapon.WeaponItem.WeaponDamage.Average());

        _currentWeapon = _weapons[index];
        setCurrentWeaponAmmo(_currentWeapon.WeaponItem.MagazineBullets);

        _shootingInterval = _currentWeapon.WeaponItem.ShootInterval;

        _playerStats.PlayerDamage.AddModifier(_currentWeapon.WeaponItem.WeaponDamage.Average());

        OnAmmoPanelUIChanged?.Invoke(_currentAmmo, _currentWeapon.TotalAmmo);

        if (_weaponEquipped)
            presentWeapon();
    }

    private void setCurrentWeaponAmmo(int ammoNeeded)
    {
        int ammoDelta = _currentWeapon.TotalAmmo - ammoNeeded;

        if (ammoDelta > 0)
        {
            _currentWeapon.TotalAmmo = ammoDelta;
            _currentAmmo = _currentWeapon.WeaponItem.MagazineBullets;
        }
        else
        {
            _currentAmmo += _currentWeapon.TotalAmmo;
            _currentWeapon.TotalAmmo = 0;
        }
    }

    private void incrementShotsHit()
    {
        _shotsHit++;

        OnEnemyHit?.Invoke((float)_shotsHit / _shotsFired);
    }

    public float GetAccuracy()
    {
        if (_shotsFired == 0)
            return 0.0f;
        else
            return (float)_shotsHit / _shotsFired;
    }

    private bool shootOnce()
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

            Transform bulletTransform = Instantiate(GameAssets.Instance.BulletPrefab, _shootingSpot.position, Quaternion.identity, null);

            Bullet bullet = bulletTransform.GetComponent<Bullet>();
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

    private void reloadWeapon()
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
            setCurrentWeaponAmmo(_currentWeapon.WeaponItem.MagazineBullets);

        if (_currentAmmo > 0 && _currentAmmo < _currentWeapon.WeaponItem.MagazineBullets)
        {
            int ammoNeeded = _currentWeapon.WeaponItem.MagazineBullets - _currentAmmo;
            setCurrentWeaponAmmo(ammoNeeded);
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

    public bool AddThrowable(Weapon weapon)
    {
        if (weapon == null)
            return false;

        for (int i = 0; i < _throwables.Count; i++)
        {
            if (weapon.WeaponItem == _throwables[i].WeaponItem)
            {
                _throwables[i].TotalAmmo++;

                OnAmmoPanelUIChanged?.Invoke(_currentAmmo, _currentWeapon.TotalAmmo);

                return true;
            }
        }

        _throwables.Add(weapon);

        if (_currentThrowable == null)
            _currentThrowable = weapon;

        ThrowableImage.UpdateThrowableUIStatic(_currentThrowable);

        return true;
    }

    public bool AddWeapon(Weapon weapon)
    {
        if (weapon == null)
            return false;

        //check strength requirement
        if (_playerStats.PlayerStrength.GetFinalValue() < weapon.WeaponItem.StrengthRequired)
        {
            List<string> stringList = new List<string> { "Not enough strength!", "I cannot use this!", "Too big piece..." };
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, stringList.GetRandomElement(), Color.white);
            return false;
        }

        for (int i = 0; i < _weapons.Count; i++)
        {
            if (weapon.WeaponItem == _weapons[i].WeaponItem)
            {
                _weapons[i].TotalAmmo += UnityEngine.Random.Range(0, weapon.TotalAmmo);

                OnAmmoPanelUIChanged?.Invoke(_currentAmmo, _currentWeapon.TotalAmmo);

                return true;
            }
        }

        _weapons.Add(weapon);

        if (_currentWeapon == null)
        {
            _currentWeapon = weapon;
            OnWeaponChanged?.Invoke(this);
            SetWeapon(0);
        }

        return true;
    }

    public List<Weapon> GetWeapons()
    {
        return _weapons;
    }

    public Weapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    public static Weapon GetCurrentThrowableStatic()
    {
        return _instance?.getCurrentThrowable();
    }

    private Weapon getCurrentThrowable()
    {
        return _currentThrowable;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nonShootingDistance);
    }
}
