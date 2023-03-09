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

    public event Action<float> OnHealthUIUpdate;
    public event Action<float> OnStaminaUIUpdate;
    public event Action OnPlayerDeath;

    public List<Stat> Stats;

    [Header("Main Player Stats")]
    public Stat PlayerHealth;
    public Stat PlayerStamina;
    public Stat PlayerSpeed;
    public Stat PlayerAccuracy;
    public Stat PlayerDefense;
    public Stat PlayerStrength;
    public Vector2 PlayerDamage = new Vector2();

    public float CurrentHealth { get; private set; }
    [Space]
    public float CurrentStamina;

    private float _staminaTrigger = 0.0f;

    [Header("Movement Characteristics")]
    [SerializeField] private const float _staminaDrainConst = 2.5f;
    [SerializeField] private const float _staminaHealConst = 4.0f;
    [SerializeField] private float _staminaTriggerThreshold = 0.1f;

    private float _playerFinalSpeed;

    private GameAssets _gameAssets;
    private CameraController _cameraController;
    
    private PlayerBase _playerBase;
    private PlayerController _playerController;
    private PlayerHitManager _playerHitManager;

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

        CurrentHealth = PlayerHealth.GetFinalValue();
        CurrentStamina = PlayerStamina.GetFinalValue();
        _playerFinalSpeed = PlayerSpeed.GetFinalValue();
    }

    private void Update()
    {
        staminaManager();
    }

    public void DamageObject(float value)
    {
        if (value == 0)
            return;

        CurrentHealth -= value * (1.0f - PlayerDefense.GetFinalValue() / 100.0f);
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, value.ToString("F0"), new Color(1.0f, 0.5f, 0.0f));

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

        if (CurrentHealth <= 0.0f)
            die();

        _cameraController?.ShakeCamera(0.05f, 0.1f);
        OnHealthUIUpdate?.Invoke(CurrentHealth);
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

        CurrentHealth += value;

        if (CurrentHealth >= PlayerHealth.GetFinalValue())
        {
            CurrentHealth = PlayerHealth.GetFinalValue();
        }

        OnHealthUIUpdate?.Invoke(CurrentHealth);
    }

    public static void TemporarilyModifyStat(StatType statType, float duration, bool handicapStat = true, float modifier = 0.0f, float multiplier = 1.0f)
    {
        _instance?.temporarilyModifyStat(statType, duration, handicapStat, modifier, multiplier);
    }

    private void temporarilyModifyStat(StatType statType, float duration, bool handicapStat, float modifier, float multiplier)
    {
        Stat stat = getStatByType(statType);

        if (stat.IsHandicaped())
            return;

        stat.AddModifier(modifier);
        stat.AddBaseMultiplier(multiplier);

        if (handicapStat)
            stat.SetHandicaped(true);

        if (statType.Equals(StatType.Speed))
        {
            _playerFinalSpeed = PlayerSpeed.GetFinalValue();
            _playerController.SetLegsInjured(true);
        }

        StartCoroutine(removeStatModifierCoroutine(statType, duration, modifier, multiplier));
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
        {
            _playerFinalSpeed = PlayerSpeed.GetFinalValue();
            _playerController.SetLegsInjured(false);
        }
    }

    private IEnumerator removeStatModifierCoroutine(StatType statType, float duration, float modifier, float multiplier)
    {
        yield return new WaitForSeconds(duration);

        removeStatModifier(statType, modifier, multiplier);

        StopCoroutine(nameof(removeStatModifierCoroutine));
    }

    public bool HealCharacter(Item item)
    {
        if (item == null)
            return false;

        if (item is InstantaneousItem)
        {
            Debug.Log("Use instantaneous item!");

            InstantaneousItem instantaneous = item as InstantaneousItem;

            healing(instantaneous.LifeRestored, instantaneous.StaminaRestored, Vector2.zero, false);

            return true;
        }

        if (item is ConsumableItem)
        {
            Debug.Log("Use consumable item!");

            ConsumableItem consumable = item as ConsumableItem;

            healing(consumable.LifeRestored, consumable.StaminaRestored, consumable.LimbToughnessDuration, consumable.LimbPatcher);

            return true;
        }

        return false;
    }

    private void healing(Vector2 lifeRestoration, Vector2 staminaRestoration, Vector2 limbEnforcement, bool limbPatcher)
    {
        if (lifeRestoration.magnitude > 0.0f)
        {
            CurrentHealth += UnityEngine.Random.Range(lifeRestoration.x, lifeRestoration.y);

            if (CurrentHealth > PlayerHealth.GetFinalValue())
                CurrentHealth = PlayerHealth.GetFinalValue();

            OnHealthUIUpdate?.Invoke(CurrentHealth);
        }

        if (staminaRestoration.magnitude > 0.0f)
        {
            CurrentStamina += UnityEngine.Random.Range(staminaRestoration.x, staminaRestoration.y);

            if (CurrentStamina > PlayerStamina.GetFinalValue())
                CurrentStamina = PlayerStamina.GetFinalValue();

            OnStaminaUIUpdate?.Invoke(CurrentStamina);
        }

        if (limbEnforcement.x > 0.0f)
        {
            Debug.Log("Enforce limbs!");
        }

        if (limbPatcher)
        {
            Debug.Log("Patch limbs!");
        }
    }

    private void die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<SpriteRenderer>().enabled = false;

        OnPlayerDeath?.Invoke();

        Destroy(gameObject);
    }

    private void staminaManager()
    {
        if (_playerController == null)
            return;

        if (_playerController.IsMoving() && _playerController.IsRunning())
        {
            CurrentStamina -= Time.deltaTime * _staminaDrainConst;
            _staminaTrigger -= Time.deltaTime * _staminaDrainConst;

            if (CurrentStamina <= 0.0f)
                CurrentStamina = 0.0f;
        }
        else
        {
            if (CurrentStamina < PlayerStamina.GetFinalValue())
            {
                CurrentStamina += Time.deltaTime * _staminaHealConst;
                _staminaTrigger += Time.deltaTime * _staminaHealConst;
            }
        }

        if (Mathf.Abs(_staminaTrigger) >= _staminaTriggerThreshold)
        {
            OnStaminaUIUpdate?.Invoke(CurrentStamina);
            _staminaTrigger = 0.0f;
        }
    }

    public float GetFinalSpeed()
    {
        return _playerFinalSpeed;
    }

    public bool IsAlive()
    {
        return CurrentHealth > 0.0f;
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
                return PlayerHealth;
            case StatType.Stamina:
                return PlayerStamina;
            case StatType.Accuracy:
                return PlayerAccuracy;
            case StatType.Speed:
                return PlayerSpeed;
            case StatType.Strength:
                return PlayerStrength;
            default:
                return null;
        }
    }
}
