using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;
using System;

public class FiringTrap2 : MonoBehaviour
{
    public static Action<SFXClip> OnShooting;

    private List<ShootingSpot> _shootingSpots;

    private Vector2 _damageInterval = new Vector2(3.0f, 6.0f);
    private string _trapTag = "Enemy";

    private bool _isShooting = false;

    private float _shootingInterval = 0.3f;
    private float _defaultStartTime = 0.2f;
    private float _timer = 0.2f;

    private GameManager _gameManager;

    private void Awake()
    {
        _shootingSpots = Utilities.GetListOfObjectsFromContainer<ShootingSpot>(transform);
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        shoot();
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
        Bullet bullet = Instantiate(GameAssets.Instance.BulletPrefab, position, Quaternion.identity, null).GetComponent<Bullet>();
        bullet.SetupBullet(Utilities.GetVectorFromAngle(transform.rotation.eulerAngles.z + 90), UnityEngine.Random.Range(_damageInterval.x, _damageInterval.y), _trapTag);
        generateShootingParticles(position);
        OnShooting?.Invoke(SFXClip.GunShot);
    }

    private void generateShootingParticles(Vector2 position)
    {
        if (GameAssets.Instance.ShootingPS == null)
            return;

        ParticleSystem particleSystem = Instantiate(GameAssets.Instance.ShootingPS, position, Quaternion.identity, null);
        particleSystem.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController == null)
            return;

        _isShooting = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController == null)
            return;

        _timer = _defaultStartTime;
        _isShooting = false;
    }
}
