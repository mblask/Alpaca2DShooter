using AlpacaMyGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NPCStats : MonoBehaviour, IDamagable
{
    [Header("Enemy stats")]
    public Stat EnemyHealth;
    public Stat EnemySpeed;
    public Stat EnemyAccuracy;
    public Stat EnemyDefence;
    public Stat EnemyLimbToughness;

    private List<StatModifyingData> _statModifyingData = new List<StatModifyingData>();
    private float _healthRegeneration = 0.0f;
    private bool _isBleeding;
    private float _bleedingIntensity = 3.0f;
    private float _bloodSpawningTime = 0.5f;
    private float _bloodSpawningStopwatch = 0.0f;
    private float _bleedingStopwatch = 0.0f;
    private float _defaultBleedingTime = 15.0f;

    private bool _isEnraged = false;
    private bool _failedEnrage = false;
    private float _enragedDuration = 10.0f;
    private TimerObject _enragedTimer = new TimerObject();
    private bool _isHealing = false;
    private float _healAmount = 0.0f;
    private TimerObject _healingTimer = new TimerObject();

    private GameAssets _gameAssets;
    private GameManager _gameManager;
    private ItemSpawner _itemSpawner;
    private PlayerWeapons _playerWeapons;
    private NPCHealthCanvas _enemyHealthCanvas;

    private NPCBase _npcBase;
    private NpcAi _enemyAI;
    private NPCWeapons _npcWeapons;
    private Light2D _bossLight;
    private ParticleSystem _burstParticleSystem;

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
        _enemyAI = GetComponent<NpcAi>();
        _npcWeapons = GetComponent<NPCWeapons>();
    }

    private void Update()
    {
        processStatModifyingData();
        regenerateHealth(_healthRegeneration);
        bleedingProcess();
        healingProcess();
        enragedProcess();
    }

    private void LateUpdate()
    {
        _enemyHealthCanvas.SetPosition(this.transform);
    }

    public void InitializeStats(CharacterBaseScriptable baseScriptable)
    {
        float npcHealthMultiplier = 0.3f;
        if (baseScriptable is BossScriptable)
            npcHealthMultiplier = 1.0f;

        EnemySpeed.SetBaseValue(baseScriptable.MovementSpeed);
        EnemyAccuracy.SetBaseValue(baseScriptable.Accuracy);
        EnemyHealth.SetBaseValue(baseScriptable.Health * npcHealthMultiplier);
        EnemyDefence.SetBaseValue(baseScriptable.Defense);
        EnemyLimbToughness.SetBaseValue(baseScriptable.LimbToughness);
    }

    private void processStatModifyingData()
    {
        if (!_npcBase.EnemyType.Equals(NPCEnemyType.Boss))
            return;

        if (_statModifyingData.Count == 0)
            return;

        for (int i = _statModifyingData.Count - 1; i >= 0; i--)
        {
            StatModifyingData statModifier = _statModifyingData[i];
            statModifier.Duration -= Time.deltaTime;
            if (statModifier.Duration > 0)
                continue;

            Stat enemyStat = getStatByType(statModifier.StatAffected);
            enemyStat.RemoveBaseMultiplier(statModifier.StatMultiplier);
            enemyStat.RemoveModifier(statModifier.StatModifier);
            if (enemyStat.IsHandicaped())
                enemyStat.SetHandicaped(false);
            _statModifyingData.Remove(statModifier);

            enemyStat.SetCurrentToFinalValue();
        }
    }

    private void rollWounds()
    {
        List<StatModifyingData> injuryPool = new List<StatModifyingData>();
        rollLegsInjury(injuryPool);
        //add other rolls - arms, head, etc.

        if (injuryPool.Count == 0)
            return;

        StatModifyingData randomPick = injuryPool.GetRandomElement();
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Critical hit", Color.red);

        _statModifyingData.Add(randomPick);
        updateStatsWithWounds();
    }

    private void rollBleeding(float chance)
    {
        _isBleeding = Utilities.ChanceFunc(chance) && !_isBleeding;
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
        foreach (StatModifyingData wound in _statModifyingData)
        {
            if (!wound.IsInjury)
                continue;

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

    private void bleedingProcess()
    {
        if (!_isBleeding)
            return;

        EnemyHealth.UpdateCurrentValue(-1.0f * _bleedingIntensity * Time.deltaTime);
        if (EnemyHealth.GetCurrentValue() <= 0.0f)
        {
            die();
            return;
        }

        _bloodSpawningStopwatch += Time.deltaTime;
        if (_bloodSpawningStopwatch >= _bloodSpawningTime)
        {
            Vector3 position = transform.position + Utilities.GetRandomVector3(1.0f, false);
            Instantiate(_gameAssets.Blood, position, Quaternion.identity, null);
            FloatingTextSpawner.CreateFloatingTextStatic
                (transform.position, "Bleeding", Color.red, 0.7f, 6.0f, 2.0f, true, FloatDirection.Any);
            _bloodSpawningStopwatch = 0.0f;
        }

        _bleedingStopwatch += Time.deltaTime;
        if (_bleedingStopwatch >= _defaultBleedingTime)
        {
            _isBleeding = false;
            _bleedingStopwatch = 0.0f;
        }
    }

    private void enragedProcess()
    {
        if (!_isEnraged)
            return;

        if (_enragedTimer.IsOver)
        {
            _bossLight.color = Color.red;
            return;
        }

        _enragedTimer.Update(Time.deltaTime);
    }

    private void bossEnrage()
    {
        if (_failedEnrage)
            return;

        if (!_npcBase.EnemyType.Equals(NPCEnemyType.Boss))
            return;

            float rageThreshold = 0.05f;
        if (EnemyHealth.GetCurrentValue() / EnemyHealth.GetFinalValue() >= rageThreshold)
            return;

        if (Utilities.ChanceFunc(25))
        {
            _failedEnrage = true;
            return;
        }

        if (_isEnraged)
            return;

        _isEnraged = true;
        playBurstParticleSystem();
        _enragedTimer = new TimerObject(_enragedDuration);
        Vector2 healthPercentageToHeal = new Vector2(30.0f, 60.0f);
        float healAmount = EnemyHealth.GetFinalValue() * healthPercentageToHeal.GetRandom() / 100.0f;
        Debug.Log(healAmount);
        float healingDuration = 2.0f;
        healCharacter(healAmount, healingDuration);

        //Stat buffs
        float buffDuration = 10.0f;
        float speedMultiplier = 1.5f;
        float defenceModifier = 25.0f;
        float shootingTimerMultiplier = 0.75f;
        temporaryModifyStat(EnemySpeed, 0.0f, speedMultiplier, buffDuration, true);
        temporaryModifyStat(EnemyDefence, defenceModifier, 0.0f, buffDuration, true);
        _npcWeapons.SetTemporaryShootingMultiplier(shootingTimerMultiplier, buffDuration);
    }

    private void temporaryModifyStat(Stat stat, float modifier, float multiplier, float duration, bool statHandicaped = false)
    {
        if (stat.IsHandicaped())
            return;

        _statModifyingData.Add(new StatModifyingData
        {
            Duration = duration,
            IsInjury = false,
            StatAffected = stat.StatType,
            StatModifier = modifier,
            StatMultiplier = multiplier,
            StatHandicaped = statHandicaped
        });

        stat.AddModifier(modifier);
        stat.AddBaseMultiplier(multiplier);
    }

    public void TemporaryModifyStat(StatType statType, float modifier, float multiplier, float duration)
    {
        Stat stat = getStatByType(statType);
        temporaryModifyStat(stat, modifier, multiplier, duration);
    }

    private void playBurstParticleSystem()
    {
        if (_burstParticleSystem == null)
            _burstParticleSystem = transform.Find("BossParticleSystem").Find("BurstSystem").GetComponent<ParticleSystem>();
        
        if (_bossLight == null)
            _bossLight = transform.Find("BossLight").GetComponent<Light2D>();

        _bossLight.color = Color.green;
        _burstParticleSystem.Play();
    }

    private void healingProcess()
    {
        if (!_isHealing)
            return;

        if (_healingTimer.Duration == 0.0f)
        {
            EnemyHealth.UpdateCurrentValue(_healAmount);
            _enemyHealthCanvas.UpdateHealthSlider(EnemyHealth.GetCurrentValue());
            _isHealing = false;
            return;
        }

        if (_healingTimer.Update())
        {
            _isHealing = false;
            _enemyHealthCanvas.DeactivateHealthSlider();
            return;
        }

        float healthincrement = _healAmount / _healingTimer.Duration * Time.deltaTime;
        EnemyHealth.UpdateCurrentValue(healthincrement);
        _enemyHealthCanvas.UpdateHealthSlider(EnemyHealth.GetCurrentValue());
    }

    private void healCharacter(float amount, float duration)
    {
        _isHealing = true;
        _healAmount = amount;
        _healingTimer = new TimerObject(duration);
        _enemyHealthCanvas.ActivateHealthSlider();
    }

    public void DamageObject(DamageData damageData)
    {
        if (damageData.Damage == 0.0f)
            return;

        float totalDamage = damageData.Damage * (1.0f - EnemyDefence.GetFinalValue() / 100.0f);
        EnemyHealth.UpdateCurrentValue(-1.0f * totalDamage);
        FloatingTextSpawner.CreateFloatingTextStatic
            (transform.position, totalDamage.ToString("F0"), new Color(1.0f, 0.5f, 0.0f));

        if (!_isEnraged && !_failedEnrage)
            bossEnrage();

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
        {
            rollWounds();
        }

        rollBleeding(damageData.BleedingChance);
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

        if (_npcBase.EnemyType.Equals(NPCEnemyType.Boss))
            _npcBase.BossKilled();

        for (int i = 0; i < 5; i++)
        {
            Vector3 position = transform.position + Utilities.GetRandomVector3(1.0f, false);
            Instantiate(_gameAssets.Blood, position, Quaternion.identity, null);
        }
        
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
            case StatType.Speed:
                return EnemySpeed;
            case StatType.Defense:
                return EnemyDefence;
            case StatType.Accuracy:
                return EnemyAccuracy;
            case StatType.LimbToughness:
                return EnemyLimbToughness;
            default:
                return null;
        }
    }
}
