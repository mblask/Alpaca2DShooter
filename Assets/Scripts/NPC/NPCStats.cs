using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStats : MonoBehaviour, IDamagable
{
    public static event Action OnHit;
    public static event Action<NPCStats> OnEnemyDeath;

    public float CurrentHealth { get; private set; }

    [Header("Enemy stats")]
    public Stat EnemyHealth;
    public Stat EnemySpeed;
    public Stat EnemyAccuracy;

    private GameAssets _gameAssets;
    private NPCHealthCanvas _enemyHealthCanvas;

    private NPC_AI _enemyAI;

    private bool _isDead;

    private void Awake()
    {
        _enemyHealthCanvas = transform.Find("CharacterHealthCanvas").GetComponent<NPCHealthCanvas>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _enemyAI = GetComponent<NPC_AI>();
        CurrentHealth = EnemyHealth.GetFinalValue();
    }

    private void LateUpdate()
    {
        _enemyHealthCanvas.SetPosition(this.transform);
    }

    public void DamageObject(float value)
    {
        if (value == 0.0f)
            return;

        CurrentHealth -= value;
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, value.ToString("F0"), new Color(1.0f, 0.5f, 0.0f));

        _enemyHealthCanvas.UpdateHealthSlider(CurrentHealth);

        Instantiate(_gameAssets.BloodPS, transform.position, Quaternion.identity, null);
        Instantiate(_gameAssets.Blood, transform.position, Quaternion.identity, null);

        _enemyHealthCanvas.ActivateHealthSlider();

        _enemyAI.ExtendViewAndAttackDistance();
        OnHit?.Invoke();

        hitShading();

        if (CurrentHealth <= 0.0f)
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
