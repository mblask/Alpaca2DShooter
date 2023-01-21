using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCEnemyType
{
    Normal,
    Boss,
}

public class NPCStats : MonoBehaviour, IDamagable
{
    public static event Action OnHit;
    public static event Action<NPCStats> OnEnemyDeath;

    private float _currentHealth;

    [SerializeField] private NPCEnemyType _enemyType;

    [Header("Enemy stats")]
    public Stat EnemyHealth;
    public Stat EnemySpeed;
    public Stat EnemyAccuracy;

    private GameAssets _gameAssets;
    private NPCHealthCanvas _enemyHealthCanvas;

    private NPC_AI _enemyAI;

    private bool _isDead = false;

    private void Awake()
    {
        _enemyHealthCanvas = transform.Find("CharacterHealthCanvas").GetComponent<NPCHealthCanvas>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _enemyAI = GetComponent<NPC_AI>();
    }

    private void LateUpdate()
    {
        _enemyHealthCanvas.SetPosition(this.transform);
    }

    public void ModifyStats()
    {
        if (!_enemyType.Equals(NPCEnemyType.Boss))
            return;

        //health, speed, accuracy multipliers
        float[] multipliers = new float[3] { 8.0f, 2.0f, 3.0f };

        EnemyHealth.AddBaseMultiplier(multipliers[0]);
        _currentHealth = EnemyHealth.GetFinalValue();
        EnemySpeed.AddBaseMultiplier(multipliers[1]);
        EnemyAccuracy.AddBaseMultiplier(multipliers[2]);
    }

    public void DamageObject(float value)
    {
        if (value == 0.0f)
            return;

        _currentHealth -= value;
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, value.ToString("F0"), new Color(1.0f, 0.5f, 0.0f));

        _enemyHealthCanvas.UpdateHealthSlider(_currentHealth);

        Instantiate(_gameAssets.BloodPS, transform.position, Quaternion.identity, null);
        Instantiate(_gameAssets.Blood, transform.position, Quaternion.identity, null);

        _enemyHealthCanvas.ActivateHealthSlider();

        _enemyAI.ExtendViewAndAttackDistance();
        OnHit?.Invoke();

        hitShading();

        if (_currentHealth <= 0.0f)
            die();
    }

    private void hitShading()
    {
        StopCoroutine(hitShadingCoroutine());
        StartCoroutine(hitShadingCoroutine());
    }

    private IEnumerator hitShadingCoroutine()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;

        Vector2 colorVector = Vector2.zero;

        float colorChangeSpeed = 5.0f;

        while (colorVector.sqrMagnitude < 1.0f)
        {
            colorVector.x += Time.deltaTime * colorChangeSpeed;
            colorVector.y += Time.deltaTime * colorChangeSpeed;

            Color newColor = new Color(1.0f, colorVector.x, colorVector.y, 1.0f);
            spriteRenderer.color = newColor;

            yield return null;
        }

        spriteRenderer.color = Color.white;
    }

    private void die()
    {
        if (!_isDead)
        {
            _isDead = true;
            OnEnemyDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
