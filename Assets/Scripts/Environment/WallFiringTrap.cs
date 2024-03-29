using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class WallFiringTrap : Hackable
{
    private List<ShootingSpot> _shootingSpots;

    private NPCAllegiance _allegiance;

    private Vector2 _damageInterval = new Vector2(3.0f, 6.0f);

    private bool _electricityAvailable = true;
    private bool _turnedOn = true;
    private bool _isShooting = false;

    private float _shootingInterval = 0.3f;
    private float _defaultStartTime = 0.2f;
    private float _timer = 0.2f;

    private GameManager _gameManager;
    private AudioManager _audioManager;

    private void Awake()
    {
        _shootingSpots = Utilities.GetListOfObjectsFromContainer<ShootingSpot>(transform);
        _allegiance = NPCAllegiance.Enemy;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    private void Update()
    {
        if (!_electricityAvailable)
            return;

        if (!_turnedOn)
            return;

        shoot();
    }

    public override void SwitchAllegiance()
    {
        _allegiance = _allegiance.Equals(NPCAllegiance.Enemy) ?
            NPCAllegiance.Ally : NPCAllegiance.Enemy;
    }

    public override void ElectricityAvailable(bool value)
    {
        _electricityAvailable = value;
    }

    public override void TurnOn()
    {
        _turnedOn = true;
    }

    public override void TurnOff()
    {
        _turnedOn = false;
    }

    public override bool Toggle()
    {
        _turnedOn = !_turnedOn;
        return _turnedOn;
    }

    private void shoot()
    {
        if (!_gameManager.IsGameRunning())
            return;

        if (_shootingSpots == null)
            return;

        if (_shootingSpots.Count == 0)
            return;

        if (!_isShooting)
            return;

        _timer -= Time.deltaTime;

        if (_timer > 0.0f)
            return;

        foreach (ShootingSpot shootingSpot in _shootingSpots)
            shootOnceFrom(shootingSpot.transform.position);

        _timer = _shootingInterval;
    }

    private void shootOnceFrom(Vector2 position)
    {
        Bullet bullet = Instantiate(GameAssets.Instance.BulletPrefab, position, Quaternion.identity, null)
            .GetComponent<Bullet>();
        Vector2 directionVector = Utilities.GetVectorFromAngle(transform.rotation.eulerAngles.z + 90);
        DamageData damageData = new DamageData { Damage = _damageInterval.GetRandom() };
        bullet.SetupBullet(directionVector, damageData, gameObject.tag);

        generateShootingParticles(position);
        _audioManager.PlayClip(SFXClip.GunShot);
    }

    private void generateShootingParticles(Vector2 position)
    {
        if (GameAssets.Instance.ShootingPS == null)
            return;

        ParticleSystem particleSystem = 
            Instantiate(GameAssets.Instance.ShootingPS, position, Quaternion.identity, null);
        particleSystem.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_allegiance.Equals(NPCAllegiance.Enemy))
            if (!searchPlayer(collision))
                return;

        if (_allegiance.Equals(NPCAllegiance.Ally))
            if (!searchEnemy(collision))
                return;

        _isShooting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_allegiance.Equals(NPCAllegiance.Enemy))
            if (!searchPlayer(collision))
                return;

        if (_allegiance.Equals(NPCAllegiance.Ally))
            if (!searchEnemy(collision))
                return;

        _timer = _defaultStartTime;
        _isShooting = false;
    }

    private bool searchPlayer(Collider2D collider)
    {
        PlayerController playerController = collider.GetComponent<PlayerController>();
        if (playerController == null)
            return false;

        return true;
    }

    private bool searchEnemy(Collider2D collider)
    {
        NPCStats npcStats = collider.GetComponent<NPCStats>();
        if (npcStats == null)
            return false;

        return true;
    }
}
