using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class PlayerWeapons : MonoBehaviour
{
    private static PlayerWeapons _instance;
    public static PlayerWeapons Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("Shooting settings - Read-only")]
    [SerializeField] private float _nonShootingDistance = 1.00f;
    [SerializeField] private float _closeQuarterShooting = 0.75f;
    [SerializeField] private float _shootingOffset = 0.5f;

    [Header("Camera shake - Read-only")]
    [SerializeField] private float _cameraShakeDuration = 0.05f;
    [SerializeField] private float _cameraShakeMagnitude = 0.1f;

    [Header("Items")]
    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();
    [SerializeField] private List<ThrowableWeapon> _throwables = new List<ThrowableWeapon>();

    private Transform _shootingSpot;
    private Camera _camera;
    private GameManager _gameManager;
    private PlayerAnimations _playerAnimations;
    private PlayerStats _playerStats;
    private CameraController _cameraController;
    private IPointerOver _pointerOver;

    private Vector2 _mousePosition;

    private Weapon _currentWeapon;
    public Weapon CurrentWeapon
    {
        get
        {
            return _currentWeapon;
        }
    }
    private ThrowableWeapon _currentThrowable;

    private bool _weaponEquipped = false;
    private bool _canShoot = true;
    private bool _isShooting = false;
    private bool _canSwitchWeapons = true;
    private bool _canPutWeaponAway = true;

    private float _shootingInterval;
    private bool _intervalWeaponActivated = false;
    private bool _isReloading = false;

    private int _currentAmmo;
    private int _shotsFired;
    private int _shotsHit;

    private WeaponImage _weaponImage;
    private AmmoPanel _ammoPanel;
    private AccuracyPanel _accuracyPanel;
    private MouseCursor _mouseCursor;
    private AudioManager _audioManager;

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
        _pointerOver = new PointerOver();

        _weaponImage = WeaponImage.Instance;
        _ammoPanel = AmmoPanel.Instance;
        _accuracyPanel = AccuracyPanel.Instance;
        _mouseCursor = MouseCursor.Instance;
        _audioManager = AudioManager.Instance;

        weaponsStartSetup();
    }

    private void Update()
    {
        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        keyboardInput();
        intervalWeaponProcedure();
    }

    private void weaponsStartSetup()
    {
        if (_weapons.Count > 0)
            SetWeapon(0);

        if (_throwables.Count > 0)
            _currentThrowable = _throwables[0];
        
        ThrowableImage.UpdateThrowableUIStatic(_currentThrowable);
    }

    private void keyboardInput()
    {
        if (_playerStats == null || _gameManager == null)
            return;

        if (!_playerStats.IsAlive() || !_gameManager.IsGameRunning() || _gameManager.IsPaused())
            return;

        if (Input.GetKeyDown(KeyCode.R) && _weaponEquipped)
            reloadWeapon();

        if (Input.GetKeyDown(KeyCode.E) && _canSwitchWeapons)
            switchWeapon(1);

        if (Input.GetKeyDown(KeyCode.Q) && _canSwitchWeapons)
            switchWeapon(-1);

        if (Input.GetKeyDown(KeyCode.Tab) && _canSwitchWeapons)
            switchThrowables();

        if (Input.GetKeyDown(KeyCode.F) && _canPutWeaponAway)
        {
            if (_weapons.Count == 0)
                return;

            _weaponEquipped = !_weaponEquipped;
            presentWeapon();
        }

        if (Input.GetKeyDown(KeyCode.G))
            useThrowable();
    }

    public void LeftClickDown()
    {
        if (_pointerOver.OverUI())
            return;

        if (_currentWeapon == null)
            return;

        if (!_weaponEquipped)
            return;

        if (_isReloading)
            return;

        if (!_canShoot)
            return;

        _isShooting = true;
        _canSwitchWeapons = false;
        _canPutWeaponAway = false;

        evaluateWeaponTypesAndShoot();
    }

    public void LeftClickUp()
    {
        _isShooting = false;
        _canSwitchWeapons = true;
        _canPutWeaponAway = true;
        StopCoroutine(autoShootingCoroutine());
    }

    private void intervalWeaponProcedure()
    {
        if (!_intervalWeaponActivated)
            return;

        _shootingInterval += Time.deltaTime;

        if (_shootingInterval >= _currentWeapon.WeaponItem.ShootInterval)
        {
            _intervalWeaponActivated = false;
            _shootingInterval = 0.0f;
        }
    }

    private void evaluateWeaponTypesAndShoot()
    {
        if (_currentWeapon.WeaponItem.Automatic)
        {
            StartCoroutine(autoShootingCoroutine());
            return;
        }

        if (_currentWeapon.WeaponItem.ShootInterval <= 0.0f)
        {
            shootOnce();
            return;
        }

        if (_intervalWeaponActivated)
            return;

        _intervalWeaponActivated = shootOnce();
        _shootingInterval = 0.0f;
    }

    private void useThrowable()
    {
        if (_throwables.Count == 0)
        {
            string message = "I have nothing to throw!";
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, message, Color.white);
            return;
        }

        Vector3 vectorFromViewAngle = (Vector3)Utilities.GetVectorFromAngle(transform.rotation.eulerAngles.z);

        Vector3 forceVector = _currentThrowable.ThrowableItem.ThrowForce * vectorFromViewAngle;
        Vector3 throwPosition = transform.position + 1.5f * vectorFromViewAngle;

        Transform throwableTransform;
        switch (_currentThrowable.ThrowableItem.Type)
        {
            case ThrowableWeaponType.Mine:
                throwableTransform = Instantiate(GameAssets.Instance.Mine, throwPosition, Quaternion.identity, null);
                Mine mine = throwableTransform.GetComponent<Mine>();
                mine.ArmMine();
                mine.ThrowMine(forceVector);
                break;
            case ThrowableWeaponType.Grenade:
                throwableTransform = 
                    Instantiate(GameAssets.Instance.Grenade, transform.position, Quaternion.identity, null);
                Grenade grenade = throwableTransform.GetComponent<Grenade>();
                grenade.ArmGrenade();
                grenade.ThrowGrenade(forceVector);
                break;
            default:
                return;
        }

        _currentThrowable.TotalAmmo--;

        if (_currentThrowable.TotalAmmo == 0)
        {
            _throwables.Remove(_currentThrowable);

            if (_throwables.Count > 0)
            {
                _currentThrowable = _throwables[0];
                ThrowableImage.UpdateThrowableUIStatic(_currentThrowable);
            }
            else
            {
                _currentThrowable = null;
                ThrowableImage.UpdateThrowableUIStatic(_currentThrowable);
            }
        }
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
        while (_isShooting && !_isReloading)
        {
            shootOnce();
            yield return new WaitForSeconds(_shootingInterval);
        }
    }

    public void EnableShooting(bool value)
    {
        _canShoot = value;
    }

    public void SetReloading(bool value)
    {
        _isReloading = value;
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
        _weaponImage.SetWeaponImage(_currentWeapon);
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

        _currentWeapon = _weapons[index];
        setCurrentWeaponAmmo(_currentWeapon.WeaponItem.MagazineBullets);

        _playerStats.PlayerDamage = new Vector2(
            _currentWeapon.WeaponItem.WeaponDamage.x + (_playerStats.PlayerStrength.GetFinalValue() - 2.0f) / 10.0f,
            _currentWeapon.WeaponItem.WeaponDamage.y + (_playerStats.PlayerStrength.GetFinalValue() - 2.0f) / 10.0f
            );

        _shootingInterval = _currentWeapon.WeaponItem.ShootInterval;

        _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

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

    public void IncrementShotsHit()
    {
        _shotsHit++;
        _accuracyPanel?.SetupAccuracyText((float)_shotsHit / _shotsFired);
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

        int bulletAmount = _currentWeapon.WeaponItem.Trigger.Equals(AnimationType.Shotgun) ? 5 : 1;

        for (int i = 0; i < bulletAmount; i++)
        {
            Vector2 direction = shootingDirection(_mousePosition);

            if (direction == Vector2.zero)
                return false;

            Transform bulletTransform = Instantiate(GameAssets.Instance.BulletPrefab, _shootingSpot.position, Quaternion.identity, null);

            Bullet bullet = bulletTransform.GetComponent<Bullet>();
            bullet.SetupBullet(direction, _playerStats.PlayerDamage.GetRandom(), gameObject.tag);
        }
        
        _shotsFired++;

        _cameraController?.ShakeCamera(_cameraShakeDuration, _cameraShakeMagnitude);

        generateShootingParticleSystem();
        alertNearbyEnemies();

        _currentAmmo--;

        if (_currentAmmo < 0)
            _currentAmmo = 0;

        _mouseCursor.OnShooting(_currentWeapon);
        _audioManager.PlayClip(_currentWeapon.WeaponItem.WeaponShootAudio);

        _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

        return true;
    }

    private void alertNearbyEnemies()
    {
        float soundRadius = 10.0f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soundRadius);
        foreach (Collider2D collider in colliders)
        {
            NPC_AI npc = collider.GetComponent<NPC_AI>();
            if (npc != null)
                npc.AlertNPC(this.transform);
        }
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

        _audioManager.PlayClip(_currentWeapon.WeaponItem.WeaponReloadAudio);
        _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

        _playerAnimations.PlayAnimation(AnimationType.Reload);
        presentWeapon();
    }

    public static bool AddAmmoStatic(AmmoItem ammo)
    {
        return _instance.addAmmo(ammo);
    }

    private bool addAmmo(AmmoItem ammo)
    {
        foreach (Weapon weapon in _weapons)
        {
            if (weapon.WeaponItem.AmmoType != ammo.AmmoType)
                continue;

            weapon.TotalAmmo += ammo.Amount;
            if (_currentWeapon == weapon)
                _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

            PlayerInventory.DeleteItemFromInventoryStatic(ammo);
            return true;
        }

        return false;
    }

    private void generateShootingParticleSystem()
    {
        Vector3 rotation = new Vector3(-1.0f * _shootingSpot.rotation.eulerAngles.z, 0.0f, 0.0f);

        ParticleSystem shootingPS = Instantiate(GameAssets.Instance.ShootingPS);
        shootingPS.transform.position = _shootingSpot.position;
        shootingPS.transform.Rotate(rotation);

        shootingPS.Play();
    }

    public bool AddThrowable(ThrowableWeapon weapon)
    {
        if (weapon == null)
            return false;

        for (int i = 0; i < _throwables.Count; i++)
        {
            if (weapon.ThrowableItem == _throwables[i].ThrowableItem)
            {
                _throwables[i].TotalAmmo++;
                _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

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
                Vector2Int ammoDropRange = new Vector2Int(10, 30);
                _weapons[i].TotalAmmo += UnityEngine.Random.Range(ammoDropRange.x, ammoDropRange.y);

                _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

                return true;
            }
        }

        _weapons.Add(weapon);

        if (_currentWeapon == null)
        {
            _currentWeapon = weapon;
            _weaponImage.SetWeaponImage(_currentWeapon);
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

    public static ThrowableWeapon GetCurrentThrowableStatic()
    {
        return _instance?.getCurrentThrowable();
    }

    public bool IsArmed()
    {
        return _weaponEquipped;
    }

    private ThrowableWeapon getCurrentThrowable()
    {
        return _currentThrowable;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nonShootingDistance);
    }
}
