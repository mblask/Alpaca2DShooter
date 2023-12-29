using AlpacaMyGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStats : MonoBehaviour, IDamagable
{
    [Header("Enemy stats")]
    public Stat EnemyHealth;
    public Stat EnemySpeed;
    public Stat EnemyAccuracy;
    public Stat EnemyDefense;
    public Stat EnemyLimbToughness;

    private List<StatModifyingData> _wounds = new List<StatModifyingData>();
    private float _healthRegeneration;

    private GameAssets _gameAssets;
    private GameManager _gameManager;
    private ItemSpawner _itemSpawner;
    private PlayerWeapons _playerWeapons;
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
        _npcBase = GetComponent<NPCBase>();
        _enemyAI = GetComponent<NPC_AI>();
    }

    private void Update()
    {
        processWounds();
        regenerateHealth(_healthRegeneration);
    }

    private void LateUpdate()
    {
        _enemyHealthCanvas.SetPosition(this.transform);
    }

    public void InitializeStats(CharacterBaseScriptable baseScriptable)
    {
        float npcHealthModifier = 0.3f;
        EnemySpeed.SetBaseValue(baseScriptable.MovementSpeed);
        EnemyAccuracy.SetBaseValue(baseScriptable.Accuracy);
        EnemyHealth.SetBaseValue(baseScriptable.Health * npcHealthModifier);
        EnemyDefense.SetBaseValue(baseScriptable.Defense);
        EnemyLimbToughness.SetBaseValue(baseScriptable.LimbToughness);
    }

    private void processWounds()
    {
        if (!_npcBase.EnemyType.Equals(NPCEnemyType.Boss))
            return;

        if (_wounds.Count == 0)
            return;

        for (int i = _wounds.Count - 1; i >= 0; i--)
        {
            StatModifyingData wound = _wounds[i];
            wound.Duration -= Time.deltaTime;
            if (wound.Duration > 0)
                continue;

            Stat enemyStat = getStatByType(wound.StatAffected);
            enemyStat.RemoveBaseMultiplier(wound.StatMultiplier);
            enemyStat.RemoveModifier(wound.StatModifier);
            _wounds.Remove(wound);
        }
    }

    private void rollWoundsDice()
    {
        List<StatModifyingData> injuryPool = new List<StatModifyingData>();
        rollLegsInjury(injuryPool);
        //add other rolls - arms, head, etc.

        if (injuryPool.Count == 0)
            return;

        StatModifyingData randomPick = injuryPool.GetRandomElement();
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Critical hit", Color.red);

        _wounds.Add(randomPick);
        updateStatsWithWounds();
    }

    private void rollLegsInjury(List<StatModifyingData> injuryPool)
    {
        if (EnemySpeed.IsHandicaped())
            return;

        float legsWoundedChance = 2.0f;
        if (!Utilities.ChanceFunc(legsWoundedChance))
            return;

        float legsWoundedDuration = 10.0f;
        StatModifyingData crippleLegs = new StatModifyingData
        {
            StatAffected = StatType.Speed,
            Duration = legsWoundedDuration,
            IsInjury = true,
            StatHandicaped = true,
            StatMultiplier = 0.5f,
        };

        injuryPool.Add(crippleLegs);
    }

    private void updateStatsWithWounds()
    {
        foreach (StatModifyingData wound in _wounds)
        {
            Stat enemyStat = getStatByType(wound.StatAffected);
            if (enemyStat.IsHandicaped())
                continue;

            enemyStat.SetHandicaped(wound.StatHandicaped);
            enemyStat.AddBaseMultiplier(wound.StatMultiplier);
            enemyStat.AddModifier(wound.StatModifier);
        }
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

        if (_npcBase.EnemyType.Equals(NPCEnemyType.Boss))
            rollWoundsDice();

        _enemyHealthCanvas.ActivateHealthSlider();

        _enemyAI.ExtendAwareness();
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
        if (_isDead)
            return;
        
        _isDead = true;
        _gameManager.IncrementEnemiesKilled();
        _itemSpawner.SpawnRandomItemAt(transform.position);
        Destroy(gameObject);
    }

    private Stat getStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.Health:
                return EnemyHealth;
            case StatType.Accuracy:
                return EnemyAccuracy;
            case StatType.Speed:
                return EnemySpeed;
            case StatType.LimbToughness:
                return EnemyLimbToughness;
            default:
                return null;
        }
    }
}
