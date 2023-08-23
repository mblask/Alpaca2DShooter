using System;
using System.Collections;
using UnityEngine;

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
    public Stat EnemyDefense;

    private float _healthRegeneration;

    private GameAssets _gameAssets;
    private NPCHealthCanvas _enemyHealthCanvas;

    private NPCBase _npcBase;
    private NPC_AI _enemyAI;

    private bool _isDead = false;

    private void Awake()
    {
        _enemyHealthCanvas = transform.Find("CharacterHealthCanvas").GetComponent<NPCHealthCanvas>();
        _currentHealth = EnemyHealth.GetFinalValue();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _npcBase = GetComponent<NPCBase>();
        _enemyAI = GetComponent<NPC_AI>();
    }

    private void Update()
    {
        regenerateHealth(_healthRegeneration);
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
        float healthMultiplier = 8.0f;
        float speedMultiplier = 2.0f;
        float accuracyMultiplier = 3.0f;

        EnemyHealth.AddBaseMultiplier(healthMultiplier);
        _currentHealth = EnemyHealth.GetFinalValue();
        _healthRegeneration = 1.0f;
        EnemySpeed.AddBaseMultiplier(speedMultiplier);
        EnemyAccuracy.AddBaseMultiplier(accuracyMultiplier);
    }

    private void regenerateHealth(float value)
    {
        if (value == 0)
            return;

        _currentHealth += value * Time.deltaTime;
    }

    public void DamageObject(float value)
    {
        if (value == 0.0f)
            return;

        _currentHealth -= value * (1.0f - EnemyDefense.GetFinalValue() / 100.0f);
        Debug.Log(value);
        Debug.Log(value * (1.0f - EnemyDefense.GetFinalValue() / 100.0f));
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, value.ToString("F0"), new Color(1.0f, 0.5f, 0.0f));

        _enemyHealthCanvas.UpdateHealthSlider(_currentHealth);

        ParticleSystem bloodPSObject = Instantiate(_gameAssets.BloodPS, transform.position, Quaternion.identity, null);
        Transform bloodTransform = Instantiate(_gameAssets.Blood, transform.position, Quaternion.identity, null);

        if (_npcBase.GetCharacterBaseScriptable().CharacterType.Equals(CharacterBaseType.Robot))
        {
            ParticleSystem.MainModule bloodPSMain = bloodPSObject.main;
            SpriteRenderer bloodSR = bloodTransform.GetComponent<SpriteRenderer>();

            Color robotBloodColor = Color.black;

            bloodPSMain.startColor = robotBloodColor;
            bloodSR.color = robotBloodColor;
        }

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
