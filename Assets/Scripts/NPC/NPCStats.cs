using System.Collections;
using UnityEngine;

public class NPCStats : MonoBehaviour, IDamagable
{
    [SerializeField] private NPCEnemyType _enemyType;
    [SerializeField] private int _bossId;
    public int BossId => _bossId;

    [Header("Enemy stats")]
    public Stat EnemyHealth;
    public Stat EnemySpeed;
    public Stat EnemyAccuracy;
    public Stat EnemyDefense;

    private float _healthRegeneration;

    private GameAssets _gameAssets;
    private GameManager _gameManager;
    private ItemSpawner _itemSpawner;
    private PlayerWeapons _playerWeapons;
    private ConversationSystem _conversationSystem;
    private NPCHealthCanvas _enemyHealthCanvas;

    private NPCBase _npcBase;
    private NPC_AI _enemyAI;

    private bool _isDead = false;

    private void Awake()
    {
        _enemyHealthCanvas = transform.Find("CharacterHealthCanvas").GetComponent<NPCHealthCanvas>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _itemSpawner = ItemSpawner.Instance;
        _gameManager = GameManager.Instance;
        _playerWeapons = PlayerWeapons.Instance;
        _conversationSystem = GetComponent<ConversationSystem>();
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
        float accuracyMultiplier = 3.0f;

        EnemyHealth.AddBaseMultiplier(healthMultiplier);
        EnemyHealth.SetCurrentToFinalValue();
        _healthRegeneration = 1.0f;
        EnemyAccuracy.AddBaseMultiplier(accuracyMultiplier);
        EnemyAccuracy.SetCurrentToFinalValue();
    }

    private void regenerateHealth(float value)
    {
        if (value == 0)
            return;

        if (EnemyHealth.GetCurrentValue() <= EnemyHealth.GetFinalValue())
            return;

        EnemyHealth.UpdateCurrentValue(value * Time.deltaTime);
    }

    public void DamageObject(float value)
    {
        if (value == 0.0f)
            return;

        EnemyHealth.UpdateCurrentValue(-value * (1.0f - EnemyDefense.GetFinalValue() / 100.0f));
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, value.ToString("F0"), new Color(1.0f, 0.5f, 0.0f));

        _enemyHealthCanvas.UpdateHealthSlider(EnemyHealth.GetCurrentValue());

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
        _playerWeapons.IncrementShotsHit();

        hitShading();

        if (EnemyHealth.GetCurrentValue() <= 0.0f)
            die();
    }

    public bool IsAlive()
    {
        return EnemyHealth.GetCurrentValue() > 0.0f;
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
            _gameManager.IncrementEnemiesKilled();
            _itemSpawner.SpawnRandomItemAt(transform.position);
            _conversationSystem.DeactivateSpeechBoxOnNPCDeath();
            Destroy(gameObject);
        }
    }
}
