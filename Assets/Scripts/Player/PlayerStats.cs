using AlpacaMyGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamagable
{
    private static PlayerStats _instance;
    public static PlayerStats Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("For testing")]
    [SerializeField] private bool _invincible = false;

    [Header("Main Player Stats")]
    public Stat Health;
    public Stat Stamina;
    public Stat Speed;
    public Stat Accuracy;
    public Stat Defense;
    public Stat Strength;
    public Stat LimbToughness;
    public Stat Hacking;
    public Stat BodyArmor;
    public Vector2 PlayerDamage = new Vector2();
    
    private List<StatModifyingData> _injuries = new List<StatModifyingData>();

    private float _accumulatedHealthLoss = 0.0f;
    private float _staminaTrigger = 0.0f;

    [Header("Movement Characteristics")]
    [SerializeField] private const float _staminaDrainConst = 2.5f;
    [SerializeField] private const float _staminaHealConst = 4.0f;
    [SerializeField] private float _staminaTriggerThreshold = 0.1f;

    private GameAssets _gameAssets;
    private CameraController _cameraController;
    
    private PlayerBase _playerBase;
    private PlayerController _playerController;
    private PlayerHitManager _playerHitManager;
    private PlayerArmorSlider _playerArmorSlider;
    private PostProcessingManager _postProcessingManager;
    private GamePlayCanvas _uiCanvas;

    public void Awake()
    {
        _instance = this;

        _playerBase = GetComponent<PlayerBase>();
        _playerController = GetComponent<PlayerController>();
        _playerHitManager = GetComponent<PlayerHitManager>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _cameraController = CameraController.Instance;
        _postProcessingManager = PostProcessingManager.Instance;
        _playerArmorSlider = PlayerArmorSlider.Instance;
        _uiCanvas = GamePlayCanvas.Instance;
    }

    private void Update()
    {
        staminaManager();
    }

    public void DamageObject(DamageData damageData)
    {
        if (_invincible)
            return;

        if (damageData.Damage == 0)
            return;

        if (BodyArmor.GetCurrentValue() > 0.0f)
        {
            BodyArmor.UpdateCurrentValue(-damageData.Damage);
            _playerArmorSlider.UpdatePlayerArmorSlider(BodyArmor.GetCurrentValue());
            return;
        }

        float modifiedValue = -damageData.Damage * (1.0f - Defense.GetFinalValue() / 100.0f);
        Health.UpdateCurrentValue(modifiedValue);
        _accumulatedHealthLoss += MathF.Abs(modifiedValue);
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, damageData.Damage.ToString("F0"), new Color(1.0f, 0.5f, 0.0f));

        ParticleSystem bloodPSObject = Instantiate(_gameAssets.BloodPS, transform.position, Quaternion.identity, null);
        Transform bloodTransform = Instantiate(_gameAssets.Blood, transform.position, Quaternion.identity, null);

        if (_playerBase.GetCharacterBaseScriptable().CharacterType.Equals(CharacterBaseType.Robot))
        {
            ParticleSystem.MainModule bloodPSMain = bloodPSObject.main;
            SpriteRenderer bloodSR = bloodTransform.GetComponent<SpriteRenderer>();

            Color robotBloodColor = Color.black;

            bloodPSMain.startColor = robotBloodColor;
            bloodSR.color = robotBloodColor;
        }

        hitShading();
        _playerHitManager?.CheckHit();

        if (Health.GetCurrentValue() <= 0.0f)
            die();

        _cameraController?.ShakeCamera(0.05f, 0.1f);

        _uiCanvas.UpdatePlayerHealthSlider(Health.GetCurrentValue());
    }

    public float GetTotalHealthLoss()
    {
        return _accumulatedHealthLoss;
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

    public void HealCharacter(float value)
    {
        if (value == 0)
            return;

        Health.UpdateCurrentValue(value);

        if (Health.GetCurrentValue() >= Health.GetFinalValue())
            Health.SetCurrentToFinalValue();

        _uiCanvas.UpdatePlayerHealthSlider(Health.GetCurrentValue());
    }

    public static void TemporarilyModifyStat(StatModifyingData injuryData)
    {
        _instance?.temporarilyModifyStat(injuryData);
    }

    private void temporarilyModifyStat(StatModifyingData injuryData)
    {
        Stat stat = getStatByType(injuryData.StatAffected);

        if (stat.IsHandicaped())
            return;

        if (injuryData.IsInjury)
            _injuries.Add(injuryData);

        stat.AddModifier(injuryData.StatModifier);
        stat.AddBaseMultiplier(injuryData.StatMultiplier);

        if (injuryData.StatHandicaped)
            stat.SetHandicaped(true);

        if (injuryData.StatAffected.Equals(StatType.Speed))
            _playerController.SetLegsInjured(true);

        StartCoroutine(removeStatModifierCoroutine(injuryData.StatAffected, injuryData.Duration, injuryData.StatModifier, injuryData.StatMultiplier));
    }

    public void RemoveAllInjuries()
    {
        foreach (StatModifyingData data in _injuries)
        {
            removeStatModifier(data.StatAffected, data.StatModifier, data.StatMultiplier);
        }
    }

    public static void RemoveStatModifierStatic(StatType statType, float modifier, float multiplier)
    {
        _instance?.removeStatModifier(statType, modifier, multiplier);
    }

    private void removeStatModifier(StatType statType, float modifier, float multiplier)
    {
        Stat stat = getStatByType(statType);
        stat.RemoveModifier(modifier);
        stat.RemoveBaseMultiplier(multiplier);
        stat.SetHandicaped(false);

        if (statType.Equals(StatType.Speed))
            _playerController.SetLegsInjured(false);
    }

    private IEnumerator removeStatModifierCoroutine(StatType statType, float duration, float modifier, float multiplier)
    {
        yield return new WaitForSeconds(duration);

        removeStatModifier(statType, modifier, multiplier);

        StopCoroutine(nameof(removeStatModifierCoroutine));
    }

    public bool UseItem(Item item)
    {
        if (item == null)
            return false;

        if (item is InstantaneousItem)
        {
            InstantaneousItem instantaneous = item as InstantaneousItem;
            healing(instantaneous.LifeRestored, instantaneous.StaminaRestored, Vector2.zero, false);
            addBodyArmor(instantaneous.Armor);

            return true;
        }

        if (item is ConsumableItem)
        {
            ConsumableItem consumable = item as ConsumableItem;
            healing(consumable.LifeRestored, consumable.StaminaRestored, consumable.LimbToughnessDuration, consumable.LimbPatcher);

            return true;
        }

        return false;
    }

    private void addBodyArmor(float bodyArmor)
    {
        float maxArmor = 100.0f;

        BodyArmor.UpdateCurrentValue(bodyArmor);
        if (BodyArmor.GetCurrentValue() > maxArmor)
            BodyArmor.SetCurrentValue(maxArmor);

        _playerArmorSlider.UpdatePlayerArmorSlider(BodyArmor.GetCurrentValue());
    }

    private void healing(Vector2 lifeRestoration, Vector2 staminaRestoration, Vector2 limbEnforcement, bool limbPatcher)
    {
        if (lifeRestoration.magnitude > 0.0f)
        {
            Health.UpdateCurrentValue(lifeRestoration.GetRandom());

            if (Health.GetCurrentValue() > Health.GetFinalValue())
                Health.SetCurrentToFinalValue();

            _uiCanvas.UpdatePlayerHealthSlider(Health.GetCurrentValue());
        }

        if (staminaRestoration.magnitude > 0.0f)
        {
            Stamina.UpdateCurrentValue(staminaRestoration.GetRandom());

            if (Stamina.GetCurrentValue() > Stamina.GetFinalValue())
                Stamina.SetCurrentToFinalValue();

            _uiCanvas.UpdatePlayerStaminaSlider(Stamina.GetCurrentValue());
        }

        if (limbEnforcement.x > 0.0f)
        {
            temporarilyModifyStat(new StatModifyingData
            {
                StatAffected = StatType.LimbToughness,
                Duration = limbEnforcement.y,
                StatHandicaped = false,
                StatModifier = limbEnforcement.x,
                IsInjury = false
            });
        }

        if (limbPatcher)
        {
            RemoveAllInjuries();
            _postProcessingManager.ResetPostProcessing();
            _cameraController.StopCameraWobble();
            _playerHitManager.RemoveAllWounds();
        }
    }

    private void die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<SpriteRenderer>().enabled = false;

        Destroy(gameObject);
    }

    private void staminaManager()
    {
        if (_playerController == null)
            return;

        if (_playerController.IsMoving() && _playerController.IsRunning())
        {
            Stamina.UpdateCurrentValue(-Time.deltaTime * _staminaDrainConst);
            _staminaTrigger -= Time.deltaTime * _staminaDrainConst;

            if (Stamina.GetCurrentValue() <= 0.0f)
                Stamina.SetCurrentValue(0.0f);
        }
        else
        {
            if (Stamina.GetCurrentValue() < Stamina.GetFinalValue())
            {
                Stamina.UpdateCurrentValue(Time.deltaTime * _staminaHealConst);
                _staminaTrigger += Time.deltaTime * _staminaHealConst;
            }
        }

        if (Mathf.Abs(_staminaTrigger) >= _staminaTriggerThreshold)
        {
            _uiCanvas.UpdatePlayerStaminaSlider(Stamina.GetCurrentValue());
            _staminaTrigger = 0.0f;
        }
    }

    public bool IsAlive()
    {
        return Health.GetCurrentValue() > 0.0f;
    }

    public static Stat GetStatByTypeStatic(StatType statType)
    {
        return _instance?.getStatByType(statType);
    }

    private Stat getStatByType(StatType statType)
    {
        switch (statType)
        {
            case StatType.Health:
                return Health;
            case StatType.Stamina:
                return Stamina;
            case StatType.Accuracy:
                return Accuracy;
            case StatType.Speed:
                return Speed;
            case StatType.Strength:
                return Strength;
            case StatType.Hacking:
                return Hacking;
            case StatType.LimbToughness:
                return LimbToughness;
            default:
                return null;
        }
    }
}
