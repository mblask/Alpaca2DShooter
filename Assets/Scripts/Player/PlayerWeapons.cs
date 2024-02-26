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
    private bool _isAutoShooting = false;
    private bool _canSwitchWeapons = true;
    private bool _canPutWeaponAway = true;

    private float _shootingInterval;
    private bool _intervalWeaponActivated = false;
    private bool _isReloading = false;

    private int _currentAmmo;
    private int _shotsFired;
    private int _shotsHit;

    private AmmoPanel _ammoPanel;
    private AccuracyPanel _accuracyPanel;
    private MouseCursor _mouseCursor;
    private AudioManager _audioManager;
    private GamePlayCanvas _uiCanvas;

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
        _pointerOver = PointerOver.GetInstance();
        _ammoPanel = AmmoPanel.Instance;

        _accuracyPanel = AccuracyPanel.Instance;
        _mouseCursor = MouseCursor.Instance;
        _audioManager = AudioManager.Instance;
        _uiCanvas = GamePlayCanvas.Instance;

        weaponsStartSetup();
    }

    private void Update()
    {
        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        keyboardInput();
        autoShootingProcedure();
        intervalWeaponProcedure();
    }

    private void weaponsStartSetup()
    {
        if (_weapons.Count > 0)
            SetWeapon(0);

        if (_throwables.Count > 0)
            _currentThrowable = _throwables[0];

        _uiCanvas.SetThrowableImage(_currentThrowable);
        if (_currentThrowable != null)
            _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);
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

        _canSwitchWeapons = false;
        _canPutWeaponAway = false;

        evaluateWeaponAndShoot();
    }

    private void evaluateWeaponAndShoot()
    {
        if (_currentWeapon.WeaponItem.ShootInterval == 0)
        {
            shootOnce();
            return;
        }

        if (_intervalWeaponActivated)
            return;

        if (_currentWeapon.WeaponItem.Automatic)
        {
            _isAutoShooting = true;
            return;
        }

        if (!_currentWeapon.WeaponItem.Automatic)
        {
            _intervalWeaponActivated = shootOnce();
            _shootingInterval = 0.0f;
            return;
        }
    }

    private void autoShootingProcedure()
    {
        if (!_isAutoShooting || _isReloading)
            return;

        _shootingInterval += Time.deltaTime;

        if (_shootingInterval >= _currentWeapon.WeaponItem.ShootInterval)
        {
            shootOnce();
            _intervalWeaponActivated = false;
            _shootingInterval = 0.0f;
        }
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

    public void LeftClickUp()
    {
        _isAutoShooting = false;
        _canSwitchWeapons = true;
        _canPutWeaponAway = true;
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
                throwableTransform = 
                    Instantiate(GameAssets.Instance.Mine, throwPosition, Quaternion.identity, null);
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
            case ThrowableWeaponType.FlashGrenade:
                throwableTransform =
                    Instantiate(GameAssets.Instance.FlashGrenade, transform.position, Quaternion.identity, null);
                FlashGrenade flashGrenade = throwableTransform.GetComponent<FlashGrenade>();
                flashGrenade.ArmGrenade();
                flashGrenade.ThrowGrenade(forceVector);
                break;
            default:
                return;
        }

        _currentThrowable.TotalAmmo--;
        _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);

        if (_currentThrowable.TotalAmmo == 0)
        {
            _throwables.Remove(_currentThrowable);

            if (_throwables.Count > 0)
            {
                _currentThrowable = _throwables[0];
                _uiCanvas.SetThrowableImage(_currentThrowable);
            }
            else
            {
                _currentThrowable = null;
                _uiCanvas.SetThrowableImage(_currentThrowable);
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

        _uiCanvas.SetThrowableImage(_currentThrowable);
        _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);
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
        _uiCanvas.SetWeaponImage(_currentWeapon);
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
            _currentWeapon.WeaponItem.WeaponDamage.x + (_playerStats.Strength.GetFinalValue() - 2.0f) / 10.0f,
            _currentWeapon.WeaponItem.WeaponDamage.y + (_playerStats.Strength.GetFinalValue() - 2.0f) / 10.0f
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

            Transform bulletTransform = 
                Instantiate(GameAssets.Instance.BulletPrefab, _shootingSpot.position, Quaternion.identity, null);

            Bullet bullet = bulletTransform.GetComponent<Bullet>();
            DamageData damageData = new DamageData { Damage = _playerStats.PlayerDamage.GetRandom() };
            bullet.SetupBullet(direction, damageData, gameObject.tag);
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
        float soundRadius = 8.0f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soundRadius);
        foreach (Collider2D collider in colliders)
        {
            NpcAi npc2 = collider.GetComponent<NpcAi>();
            if (npc2 != null)
                npc2.AlertNPC();
        }
    }

    private Vector2 shootingDirection(Vector3 targetPosition)
    {
        float distanceFromShootingSpot = ((Vector2)targetPosition - (Vector2)_shootingSpot.position).magnitude;
        float distanceFromGameObject = ((Vector2)targetPosition - (Vector2)transform.position).magnitude;

        if (distanceFromGameObject < _nonShootingDistance)
            return Vector2.zero;

        if (distanceFromShootingSpot < _closeQuarterShooting)
            return (targetPosition + getRandomOffset(_shootingOffset, _playerStats.Accuracy.GetFinalValue()) - transform.position).normalized;
        else
            return (targetPosition + getRandomOffset(_shootingOffset, _playerStats.Accuracy.GetFinalValue()) - _shootingSpot.position).normalized;
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

            AudioManager.Instance.PlayClip(ammo.LoadAudio);
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

    public bool AddThrowable(ThrowableWeapon throwable)
    {
        if (throwable == null)
            return false;

        for (int i = 0; i < _throwables.Count; i++)
        {
            if (throwable.ThrowableItem != _throwables[i].ThrowableItem)
                continue;

            _throwables[i].TotalAmmo++;
            _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);

            return true;
        }

        _throwables.Add(throwable);

        if (_currentThrowable == null)
            _currentThrowable = throwable;

        _uiCanvas.SetThrowableImage(_currentThrowable);
        _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);

        return true;
    }

    public bool AddWeapon(Weapon weapon)
    {
        if (weapon == null)
            return false;

        //check strength requirement
        if (_playerStats.Strength.GetFinalValue() < weapon.WeaponItem.StrengthRequired)
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
            _uiCanvas.SetWeaponImage(_currentWeapon);
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
