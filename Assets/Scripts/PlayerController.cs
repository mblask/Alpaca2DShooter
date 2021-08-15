using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("References")]
    public Camera Camera;
    public GameObject BulletPrefab;
    public Transform ShootingSpot;
    public ParticleSystem ShootingParticleSystem;

    [Header("Player Stats")]
    public Stat PlayerSpeed;
    public Stat PlayerDamage;

    [Header("Movement Characteristics")]
    public float SprintBoost;

    [Header("Items")]
    public List<WeaponItem> AvailableWeapons = new List<WeaponItem>();
    public List<ArtefactItem> ArtefactsCollected = new List<ArtefactItem>();

    [Header("Read-only")]
    [SerializeField] private float _playerFinalSpeed;
    [SerializeField] private float _nonShootingDistance = 1.25f;
    [SerializeField] private float _closeQuarterShooting = 0.75f;
    [SerializeField] private float _shootingOffset = 0.5f;
    [SerializeField] private float _cameraShakeDuration = 0.05f;
    [SerializeField] private float _cameraShakeMagnitude = 0.1f;
    
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private Vector2 _movement;
    private Vector2 _mousePosition;

    private WeaponItem _currentWeapon = null;
    public WeaponItem CurrentWeapon
    {
        get
        {
            return _currentWeapon;
        }
    }

    private bool _weaponEquipped = false;
    private bool _canShoot = true;
    private bool _canSwitchWeapons = true;

    private float _shootingInterval;

    private void Awake()
    {
        #region Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        #endregion

        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _playerFinalSpeed = PlayerSpeed.GetValue();
    }

    private void Start()
    {
        SetWeapon(0);
        GamePlayCanvas.Instance.UpdateAmmoText(_currentWeapon.CurrentMagazineBullets, _currentWeapon.TotalAvailableBullets);
    }

    private void Update()
    {
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.y = Input.GetAxisRaw("Vertical");

        _mousePosition = Camera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _playerFinalSpeed = PlayerSpeed.GetValue() * SprintBoost;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _playerFinalSpeed = PlayerSpeed.GetValue();
        }

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

        if (Input.GetKeyDown(KeyCode.F))
        {
            _weaponEquipped = !_weaponEquipped;
            presentWeapon();
        }

        TriggerShooting();
    }

    private void FixedUpdate()
    {
        rotatePlayer(Camera.ScreenToWorldPoint(Input.mousePosition));

        movePlayer(_movement.normalized);
    }

    public void TriggerShooting()
    {
        if (Input.GetMouseButtonDown(0) && _weaponEquipped && _canShoot)
        {
            if (_currentWeapon.Automatic)
            {
                StartCoroutine("AutoShooting");
                _canSwitchWeapons = false;
            }
            else
            {
                ShootWeapon();
            }
        }

        if (Input.GetMouseButtonUp(0) && _currentWeapon.Automatic)
        {
            StopCoroutine("AutoShooting");
            _canSwitchWeapons = true;
        }
    }

    public IEnumerator AutoShooting()
    {
        while (true)
        {
            ShootWeapon();

            yield return new WaitForSeconds(_shootingInterval);
        }
    }

    public void EnableShooting(bool value = true)
    {
        _canShoot = value;
    }

    private void movePlayer(Vector2 axisMovement)
    {
        if (axisMovement == null)
            return;

        if (axisMovement.sqrMagnitude > 1.0f)
            axisMovement.Normalize();

        Vector2 currentPosition = _rigidBody.position;
        float currentAngle = _rigidBody.rotation;

        Vector2 movementDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle));

        Vector2 strafeDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (currentAngle - 90.0f)), Mathf.Sin(Mathf.Deg2Rad * (currentAngle - 90.0f)));

        Vector2 finalMovementPosition = currentPosition + (axisMovement.x * strafeDirection + axisMovement.y * movementDirection) * _playerFinalSpeed * Time.deltaTime;
        _rigidBody.MovePosition(finalMovementPosition);
    }

    private void rotatePlayer(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - _rigidBody.position;

        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _rigidBody.rotation = rotationAngle;
    }

    private void presentWeapon()
    {
        _animator.SetBool("WeaponEquipped", _weaponEquipped);

        if (_weaponEquipped)
            _animator.SetTrigger(_currentWeapon.WeaponTrigger);
        else
            _animator.SetTrigger("RemoveWeapon");
    }

    private void switchWeapon(int leftRight)
    {
        int currentIndex = AvailableWeapons.IndexOf(_currentWeapon);

        //Left shifting
        if (leftRight == -1)
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = AvailableWeapons.Count - 1;
        }

        //Right shifting
        if (leftRight == 1)
        {
            currentIndex++;

            if (currentIndex == AvailableWeapons.Count)
                currentIndex = 0;
        }

        SetWeapon(currentIndex);
        
        GamePlayCanvas.Instance.UpdateAmmoText(_currentWeapon.CurrentMagazineBullets, _currentWeapon.TotalAvailableBullets);
    }

    public void SetWeapon(int index)
    {
        if (index < 0)
            return;

        if (PlayerDamage.GetValue() != 0)
            PlayerDamage.RemoveModifier(_currentWeapon.WeaponDamage);

        _currentWeapon = AvailableWeapons[index];
        _shootingInterval = _currentWeapon.ShootInterval;

        PlayerDamage.AddModifier(_currentWeapon.WeaponDamage);

        if (_weaponEquipped)
            presentWeapon();
    }

    public void SetWeapon(WeaponItem weapon)
    {
        if (weapon == null)
            return;

        if (PlayerDamage.GetValue() != 0)
            PlayerDamage.RemoveModifier(_currentWeapon.WeaponDamage);

        _currentWeapon = weapon;
        _shootingInterval = _currentWeapon.ShootInterval;

        PlayerDamage.AddModifier(_currentWeapon.WeaponDamage);

        if (_weaponEquipped)
            presentWeapon();
    }

    public void ShootWeapon()
    {
        if (_currentWeapon.CurrentMagazineBullets == 0)
        {
            GamePlayCanvas.Instance.FillSpeechBubbleText(ConstsEnums.OutOfAmmo);
            return;
        }

        Vector3 mouseClickedOn = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float distanceFromShootingSpot = ((Vector2)mouseClickedOn - (Vector2)ShootingSpot.position).magnitude;

        float distanceFromGameObject = ((Vector2)mouseClickedOn - (Vector2)transform.position).magnitude;

        if (distanceFromGameObject < _nonShootingDistance)
            return;

        Vector2 randomOffset = (Vector2.right * Random.Range(-1.0f, 1.0f) * _shootingOffset + Vector2.up * Random.Range(-1.0f, 1.0f) * _shootingOffset);

        Vector2 direction = new Vector2();
        if (distanceFromShootingSpot < _closeQuarterShooting)
            direction = (mouseClickedOn + (Vector3)randomOffset - transform.position).normalized;
        else
            direction = (mouseClickedOn + (Vector3)randomOffset - ShootingSpot.position).normalized;

        GameObject bullet = Instantiate(BulletPrefab);
        bullet.transform.position = ShootingSpot.position;
        bullet.GetComponent<Bullet>().SetDirection(direction);
        bullet.transform.parent = null;

        StartCoroutine(CameraController.Instance.CameraShaker(_cameraShakeDuration, _cameraShakeMagnitude));

        GenerateShootingParticleSystem();

        _currentWeapon.CurrentMagazineBullets -= 1;
        if (_currentWeapon.CurrentMagazineBullets < 0)
            _currentWeapon.CurrentMagazineBullets = 0;

        GamePlayCanvas.Instance.UpdateAmmoText(_currentWeapon.CurrentMagazineBullets, _currentWeapon.TotalAvailableBullets);
    }

    public void ReloadWeapon()
    {
        if (_currentWeapon.CurrentMagazineBullets == _currentWeapon.MagazineBullets)
        {
            Debug.Log("Weapon fully loaded. No reload needed.");
            GamePlayCanvas.Instance.FillSpeechBubbleText(ConstsEnums.WeaponFullyLoaded);
            return;
        }

        if (_currentWeapon.TotalAvailableBullets == 0)
        {
            Debug.Log("You have no more ammo!");
            GamePlayCanvas.Instance.FillSpeechBubbleText(ConstsEnums.NoMoreAmmo);
            //Trigger audio
            return;
        }

        if (_currentWeapon.CurrentMagazineBullets > 0 && _currentWeapon.CurrentMagazineBullets < _currentWeapon.MagazineBullets)
        {
            int ammoNeeded = _currentWeapon.MagazineBullets - _currentWeapon.CurrentMagazineBullets;
            _currentWeapon.CurrentMagazineBullets = _currentWeapon.MagazineBullets;
            _currentWeapon.TotalAvailableBullets -= ammoNeeded;
        }
        else
        {
            _currentWeapon.TotalAvailableBullets -= _currentWeapon.MagazineBullets;
            _currentWeapon.CurrentMagazineBullets = _currentWeapon.MagazineBullets;
        }

        GamePlayCanvas.Instance.UpdateAmmoText(_currentWeapon.CurrentMagazineBullets, _currentWeapon.TotalAvailableBullets);

        _animator.SetTrigger("Reload");
        presentWeapon();
    }

    private void GenerateShootingParticleSystem()
    {
        Vector3 rotation = new Vector3(-1.0f * ShootingSpot.rotation.eulerAngles.z, 0.0f, 0.0f);

        ParticleSystem shootingPS = Instantiate(ShootingParticleSystem);
        shootingPS.transform.position = ShootingSpot.position;
        shootingPS.transform.Rotate(rotation);

        shootingPS.Play();
    }

    public void AddWeapon(WeaponItem weapon)
    {
        if (weapon == null)
            return;

        for (int i = 0; i < AvailableWeapons.Count; i++)
        {
            if (weapon.ItemID == AvailableWeapons[i].ItemID)
            {
                AvailableWeapons[i].AddToTotalAmmo();

                if (_currentWeapon == weapon)
                    GamePlayCanvas.Instance.UpdateAmmoText(_currentWeapon.CurrentMagazineBullets, _currentWeapon.TotalAvailableBullets);

                return;
            }
        }

        AvailableWeapons.Add(weapon);
    }

    public void AddArtefact(ArtefactItem artefact)
    {
        if (artefact == null)
            return;

        ArtefactsCollected.Add(artefact);

        GameManager.Instance.UpdateCollectedArtefacts(ArtefactsCollected.Count);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nonShootingDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ShootingSpot.position, _closeQuarterShooting);
    }
}