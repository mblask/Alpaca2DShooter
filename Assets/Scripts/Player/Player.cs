using AlpacaMyGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance
    {
        get
        {
            return _instance;
        }
    }

    private GameAssets _gameAssets;
    private PlayerSelector _playerSelector;
    private Camera _camera;
    private CameraController _cameraController;
    private PostProcessingManager _postProcessingManager;
    private WoundedUI _woundedUI;
    private ItemSpawner _itemSpawner;
    private AchievementManager _achievementManager;
    private PlayerArmorSlider _playerArmorSlider;
    private GameManager _gameManager;
    private IPointerOver _pointerOver;
    private WeaponImage _weaponImage;
    private ThrowableImage _throwableImage;
    private AmmoPanel _ammoPanel;
    private ThrowableAmmoPanel _throwableAmmoPanel;
    private AccuracyPanel _accuracyPanel;
    private MouseCursor _mouseCursor;
    private AudioManager _audioManager;
    private GamePlayCanvas _uiCanvas;

    private void Awake()
    {
        _instance = this;

        _animator = GetComponent<Animator>();
        _currentAnimationType = AnimationType.Idle;

        _rigidBody = GetComponent<Rigidbody2D>();

        _shootingSpot = transform.Find("ShootingSpot");
    }

    private void Start()
    {
        if (_animator.runtimeAnimatorController == null)
            _animator.runtimeAnimatorController = GameAssets.Instance.CharacterBaseScriptableList[0].CharacterAOC;

        _playerSelector = PlayerSelector.Instance;
        _gameAssets = GameAssets.Instance;
        selectCharacter();

        _cameraController = CameraController.Instance;
        _postProcessingManager = PostProcessingManager.Instance;
        _woundedUI = WoundedUI.Instance;

        _itemSpawner = ItemSpawner.Instance;
        _achievementManager = AchievementManager.Instance;

        _playerArmorSlider = PlayerArmorSlider.Instance;

        _camera = Camera.main;
        _gameManager = GameManager.Instance;
        _pointerOver = PointerOver.GetInstance();
        _weaponImage = WeaponImage.Instance;
        _throwableImage = ThrowableImage.Instance;
        _ammoPanel = AmmoPanel.Instance;
        _throwableAmmoPanel = ThrowableAmmoPanel.Instance;

        _accuracyPanel = AccuracyPanel.Instance;
        _mouseCursor = MouseCursor.Instance;
        _audioManager = AudioManager.Instance;

        weaponsStartSetup();

        _uiCanvas = GamePlayCanvas.Instance;
    }

    private void Update()
    {
        getInput();
        triggerRunning();
        toggleInventoryUI();
        toggleCraftingUI();
        staminaManager();

        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        keyboardInput();
        autoShootingProcedure();
        intervalWeaponProcedure();
    }

    private void FixedUpdate()
    {
        rotatePlayer(_mousePosition);
        movePlayer(_movement.normalized);
    }

    //**********
    //Animations
    //**********

    private const string EQUIP_GUN = "EquipGun";
    private const string EQUIP_SILENCER = "EquipSilencer";
    private const string EQUIP_MACHINE = "EquipMachine";
    private const string EQUIP_SHOTGUN = "EquipShotgun";
    private const string RELOAD = "Reload";
    private const string REMOVE_WEAPON = "RemoveWeapon";

    private Animator _animator;

    private AnimationType _currentAnimationType;

    private void setPlayerAOC(AnimatorOverrideController aoc)
    {
        _animator.runtimeAnimatorController = aoc;
    }

    private void playAnimation(AnimationType animation)
    {
        if (isCurrentAnimationWeapon() && isAnimationWeapon(animation))
            _animator.SetTrigger(REMOVE_WEAPON);

        _currentAnimationType = animation;

        switch (animation)
        {
            case AnimationType.Idle:
                break;
            case AnimationType.Gun:
                _animator.SetTrigger(EQUIP_GUN);
                break;
            case AnimationType.Silencer:
                _animator.SetTrigger(EQUIP_SILENCER);
                break;
            case AnimationType.Machine:
                _animator.SetTrigger(EQUIP_MACHINE);
                break;
            case AnimationType.Shotgun:
                _animator.SetTrigger(EQUIP_SHOTGUN);
                break;
            case AnimationType.Reload:
                _animator.SetTrigger(RELOAD);
                break;
            case AnimationType.RemoveWeapon:
                _animator.SetTrigger(REMOVE_WEAPON);
                break;
        }
    }

    private bool isAnimationWeapon(AnimationType currentAnimationType)
    {
        switch (currentAnimationType)
        {
            case AnimationType.Gun:
            case AnimationType.Silencer:
            case AnimationType.Machine:
            case AnimationType.Shotgun:
                return true;
            default:
                return false;
        }
    }

    private bool isCurrentAnimationWeapon()
    {
        return isAnimationWeapon(_currentAnimationType);
    }

    //****
    //Base
    //****

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

    private CharacterBaseScriptable _playerCharacterBaseScriptable;

    private void selectCharacter()
    {
        try
        {
            CharacterBaseType baseType = _playerSelector.GetSelectedBase();
            List<SkillSO> selectedSkills = _playerSelector.GetSelectedSkills();
            _playerSelector.DestroySelf();
            if (baseType.Equals(CharacterBaseType.None))
                setupCharacter(_gameAssets.CharacterBaseScriptableList[0]);

            setupCharacter(_gameAssets.GetBaseScriptabeByType(baseType));
            setupPlayerSkills(selectedSkills);
        }
        catch
        {
            setupCharacter(_gameAssets.CharacterBaseScriptableList[0]);
        }
    }

    private void setupCharacter(CharacterBaseScriptable characterBase)
    {
        _playerCharacterBaseScriptable = characterBase;
        setPlayerAOC(characterBase.CharacterAOC);

        Accuracy.SetBaseValue(characterBase.Accuracy);
        Speed.SetBaseValue(characterBase.MovementSpeed * 1.2f);
        Health.SetBaseValue(characterBase.Health);
        Stamina.SetBaseValue(characterBase.Stamina);
        Defense.SetBaseValue(characterBase.Defense);
        Strength.SetBaseValue(characterBase.Strength);
        Hacking.SetBaseValue(characterBase.Hacking);
        LimbToughness.SetBaseValue(characterBase.LimbToughness);
        BodyArmor.SetBaseValue(0.0f);
    }

    private void setupPlayerSkills(List<SkillSO> skills)
    {
        foreach (SkillSO skill in skills)
        {
            switch (skill.Stat)
            {
                case StatType.Health:
                    updateStat(Health, skill);
                    break;
                case StatType.Stamina:
                    updateStat(Stamina, skill);
                    break;
                case StatType.Accuracy:
                    updateStat(Accuracy, skill);
                    break;
                case StatType.Damage:
                    PlayerDamage += Vector2.one * skill.Modifier;
                    PlayerDamage *= Vector2.one * skill.Multiplier;
                    break;
                case StatType.Defense:
                    updateStat(Defense, skill);
                    break;
                case StatType.Speed:
                    updateStat(Speed, skill);
                    break;
                case StatType.Strength:
                    updateStat(Strength, skill);
                    break;
                case StatType.LimbToughness:
                    updateStat(LimbToughness, skill);
                    break;
                case StatType.Hacking:
                    updateStat(Hacking, skill);
                    break;
                default:
                    break;
            }
        }
    }

    private void updateStat(Stat stat, SkillSO skill)
    {
        if (skill.Modifier > 0.0f)
            stat.AddModifier(skill.Modifier);

        if (skill.Multiplier > 0.0f)
            stat.AddBaseMultiplier(skill.Multiplier);
    }

    public CharacterBaseScriptable GetCharacterBaseScriptable()
    {
        return _playerCharacterBaseScriptable;
    }

    //**********
    //Controller
    //**********

    private Rigidbody2D _rigidBody;

    private bool _legsInjured = false;

    [Header("Movement Characteristics")]
    [SerializeField] private const float _staminaDrainConst = 2.5f;
    [SerializeField] private const float _staminaHealConst = 4.0f;
    [SerializeField] private float _staminaTriggerThreshold = 0.1f;
    private Vector2 _movement;
    private Vector2 _mousePosition;
    private bool _isRunning = false;
    private bool _canRun = true;
    private float _accumulatedHealthLoss = 0.0f;
    private float _staminaTrigger = 0.0f;

    private bool _inputActive = true;

    private void getInput()
    {
        if (_inputActive)
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PlayerInventory.Instance.UseConsumable(ConsumableType.HerbalBooster);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlayerInventory.Instance.UseConsumable(ConsumableType.LimbPatcher);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayerInventory.Instance.UseConsumable(ConsumableType.LimbProtector);
            }
        }

        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void deactivateInput()
    {
        _inputActive = false;
        _movement = Vector2.zero;
    }

    private void movePlayer(Vector2 axisMovement)
    {
        if (!IsAlive())
            return;

        if (axisMovement == null)
            return;

        if (axisMovement.sqrMagnitude > 1.0f)
            axisMovement.Normalize();

        Vector2 currentPosition = _rigidBody.position;
        float currentAngle = _rigidBody.rotation;

        Vector2 movementDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle));
        Vector2 strafeDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (currentAngle - 90.0f)), Mathf.Sin(Mathf.Deg2Rad * (currentAngle - 90.0f)));

        Vector2 finalMovementPosition = currentPosition + (axisMovement.x * strafeDirection + axisMovement.y * movementDirection) * Speed.GetFinalValue() * Time.fixedDeltaTime;

        _rigidBody.MovePosition(finalMovementPosition);
    }

    private void setLegsInjured(bool value)
    {
        _legsInjured = value;
    }

    private void triggerRunning()
    {
        if (_legsInjured)
            return;

        float runningSpeedMultiplier = 1.5f;

        _canRun = Stamina.GetCurrentValue() > 0.0f;

        if (!_canRun && _isRunning)
        {
            _isRunning = false;
            Speed.RemoveBaseMultiplier(runningSpeedMultiplier);
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!_isRunning)
            {
                if (_movement.magnitude > 0.0f)
                {
                    _isRunning = true;
                    Speed.AddBaseMultiplier(runningSpeedMultiplier);
                }
            }
            else
            {
                _isRunning = false;
                Speed.RemoveBaseMultiplier(runningSpeedMultiplier);
            }
        }
    }

    private void rotatePlayer(Vector2 targetPosition)
    {
        if (!IsAlive())
            return;

        Vector2 direction = targetPosition - _rigidBody.position;

        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _rigidBody.rotation = rotationAngle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDisturbable disturbable = collision.GetComponent<IDisturbable>();

        if (disturbable != null)
            disturbable.DisturbAnimation();
    }

    private bool isRunning()
    {
        return _isRunning;
    }

    private bool isMoving()
    {
        return _movement.magnitude > 0.0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 40.0f);
    }

    //***********
    //Hit manager
    //***********

    private List<WoundType> _woundsList = new List<WoundType>();

    private void checkHit()
    {
        Color hitTextColor = Color.red;

        float headShotChance = 1.0f * (1 - LimbToughness.GetFinalValue());
        float legShotChance = 3.0f * (1 - LimbToughness.GetFinalValue());
        float armShotChance = 5.0f * (1 - LimbToughness.GetFinalValue());

        if (Utilities.ChanceFunc(headShotChance))
        {
            //head hit, activate postprocessing and wobbling
            float headInjuryDuration = 10.0f;
            _cameraController.WobbleCamera(true, headInjuryDuration);
            _postProcessingManager.ActivatePostProcessing(headInjuryDuration);

            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Head shot!", hitTextColor, 0.5f, 8.0f, 2.0f, true, FloatDirection.Up);

            _woundsList.AddIfNone(WoundType.Head);

            StartCoroutine(removeWoundTypeCoroutine(WoundType.Head, headInjuryDuration));
            //Audio trigger
        }
        else if (Utilities.ChanceFunc(legShotChance))
        {
            //leg hit, decrease movement speed
            float legInjuryDuration = 8.0f;
            float speedBaseMultiplier = 0.6f;
            TemporarilyModifyStat(new StatModifyingData
            {
                StatAffected = StatType.Speed,
                Duration = legInjuryDuration,
                StatHandicaped = true,
                StatModifier = 0.0f,
                StatMultiplier = speedBaseMultiplier
            });
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Leg Hit!", hitTextColor, 0.5f, 8.0f, 2.0f, true, FloatDirection.Down);

            _woundsList.AddIfNone(WoundType.Legs);

            StartCoroutine(removeWoundTypeCoroutine(WoundType.Legs, legInjuryDuration));
            //Audio trigger
        }
        else if (Utilities.ChanceFunc(armShotChance))
        {
            //arm hit, decrease accuracy
            float armInjuryDuration = 10.0f;
            float accuracyBaseMultiplier = 0.3f;
            TemporarilyModifyStat(new StatModifyingData
            {
                StatAffected = StatType.Accuracy,
                Duration = armInjuryDuration,
                StatHandicaped = true,
                StatModifier = 0.0f,
                StatMultiplier = accuracyBaseMultiplier
            });
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Arm Hit!", hitTextColor, 0.5f, 8.0f, 2.0f, true, FloatDirection.Any);

            _woundsList.AddIfNone(WoundType.Arms);

            StartCoroutine(removeWoundTypeCoroutine(WoundType.Arms, armInjuryDuration));
            //Audio trigger
        }
        else
            return;

        _uiCanvas.ActivateWoundedUI();
    }

    private IEnumerator removeWoundTypeCoroutine(WoundType woundType, float after)
    {
        float timer = 0.0f;

        while (timer < after)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        _woundsList.Remove(woundType);
        _uiCanvas.ActivateWoundedUI();
    }

    private void removeAllWounds()
    {
        _woundsList.Clear();
        _uiCanvas.ActivateWoundedUI();
    }

    public static List<WoundType> GetWoundsListStatic()
    {
        return _instance.getWoundList();
    }

    private List<WoundType> getWoundList()
    {
        return _woundsList;
    }

    //*********
    //Inventory
    //*********

    private const int MAX_NUMBER_OF_ITEMS = 12;

    public Action OnSuccessfulAdd;
    public Action OnItemRemovedFromInventory;
    public Action OnToggleInventoryUI;
    public Action OnToggleCraftingUI;

    [SerializeField] private List<Item> _items = new List<Item>();
    private bool _craftingPossible = false;

    private void toggleInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.I))
            OnToggleInventoryUI?.Invoke();
    }

    private void toggleCraftingUI()
    {
        if (!_craftingPossible)
            return;

        if (Input.GetKeyDown(KeyCode.C))
            OnToggleCraftingUI?.Invoke();
    }

    public static List<Item> GetItemsStatic()
    {
        return _instance.getItems();
    }

    private void useConsumable(ConsumableType type)
    {
        ConsumableItem consumable = GameAssets.Instance.GetConsumableByType(type);
        if (!_items.Contains(consumable))
            return;

        if (consumable.UseItem())
            _achievementManager.CheckOnItemUsed(consumable);
    }

    public static bool AddToInventoryStatic(Item item)
    {
        return _instance.addToInventory(item);
    }

    private bool addToInventory(Item item)
    {
        if (_items.Count < MAX_NUMBER_OF_ITEMS)
        {
            _items.Add(item);
            OnSuccessfulAdd?.Invoke();

            return true;
        }

        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Invenory full", Color.white);
        return false;
    }

    public static bool ThrowFromInventoryStatic(Item item)
    {
        return _instance.throwFromInventory(item);
    }

    private bool throwFromInventory(Item item)
    {
        if (item == null)
            return false;

        if (_items.Remove(item))
        {
            float randomAngleRad = UnityEngine.Random.Range(0.0f, 6.28f);
            Vector3 position = transform.position + 2.0f * (Vector3.up * Mathf.Sin(randomAngleRad) + Vector3.right * Mathf.Cos(randomAngleRad));
            _itemSpawner.SpawnItem(position, item);

            OnItemRemovedFromInventory?.Invoke();

            return true;
        }

        return false;
    }

    public static bool DeleteItemFromInventoryStatic(Item item)
    {
        return _instance.deleteItemFromInventory(item);
    }

    private bool deleteItemFromInventory(Item item)
    {
        if (item == null)
            return false;

        if (_items.Remove(item))
        {
            OnItemRemovedFromInventory?.Invoke();
            return true;
        }
        else
            return false;
    }

    private List<Item> getItems()
    {
        return _items;
    }

    public void EnableCrafting(bool value)
    {
        _craftingPossible = value;
    }

    //*****
    //Stats
    //*****

    public event Action<float> OnHealthUIUpdate;
    public event Action<float> OnStaminaUIUpdate;

    [Header("For testing")]
    [SerializeField] private bool _invincible = false;

    private List<StatModifyingData> _injuries = new List<StatModifyingData>();

    public void DamagePlayer(float value)
    {
        if (_invincible)
            return;

        if (value == 0)
            return;

        if (BodyArmor.GetCurrentValue() > 0.0f)
        {
            BodyArmor.UpdateCurrentValue(-value);
            _playerArmorSlider.UpdatePlayerArmorSlider(BodyArmor.GetCurrentValue());
            return;
        }

        float modifiedValue = -value * (1.0f - Defense.GetFinalValue() / 100.0f);
        Health.UpdateCurrentValue(modifiedValue);
        _accumulatedHealthLoss += MathF.Abs(modifiedValue);
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, value.ToString("F0"), new Color(1.0f, 0.5f, 0.0f));

        ParticleSystem bloodPSObject = Instantiate(_gameAssets.BloodPS, transform.position, Quaternion.identity, null);
        Transform bloodTransform = Instantiate(_gameAssets.Blood, transform.position, Quaternion.identity, null);

        if (GetCharacterBaseScriptable().CharacterType.Equals(CharacterBaseType.Robot))
        {
            ParticleSystem.MainModule bloodPSMain = bloodPSObject.main;
            SpriteRenderer bloodSR = bloodTransform.GetComponent<SpriteRenderer>();

            Color robotBloodColor = Color.black;

            bloodPSMain.startColor = robotBloodColor;
            bloodSR.color = robotBloodColor;
        }

        hitShading();
        checkHit();

        if (Health.GetCurrentValue() <= 0.0f)
            die();

        _cameraController?.ShakeCamera(0.05f, 0.1f);
        OnHealthUIUpdate?.Invoke(Health.GetCurrentValue());
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

        OnHealthUIUpdate?.Invoke(Health.GetCurrentValue());
    }

    public static void TemporarilyModifyStat(StatModifyingData injuryData)
    {
        _instance.temporarilyModifyStat(injuryData);
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
            setLegsInjured(true);

        StartCoroutine(removeStatModifierCoroutine(injuryData.StatAffected, injuryData.Duration, injuryData.StatModifier, injuryData.StatMultiplier));
    }

    private void removeAllInjuries()
    {
        foreach (StatModifyingData data in _injuries)
        {
            removeStatModifier(data.StatAffected, data.StatModifier, data.StatMultiplier);
        }
    }

    public static void RemoveStatModifierStatic(StatType statType, float modifier, float multiplier)
    {
        _instance.removeStatModifier(statType, modifier, multiplier);
    }

    private void removeStatModifier(StatType statType, float modifier, float multiplier)
    {
        Stat stat = getStatByType(statType);
        stat.RemoveModifier(modifier);
        stat.RemoveBaseMultiplier(multiplier);
        stat.SetHandicaped(false);

        if (statType.Equals(StatType.Speed))
            setLegsInjured(false);
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
            BodyArmor.UpdateCurrentValue(maxArmor);

        _playerArmorSlider.UpdatePlayerArmorSlider(BodyArmor.GetCurrentValue());
    }

    private void healing(Vector2 lifeRestoration, Vector2 staminaRestoration, Vector2 limbEnforcement, bool limbPatcher)
    {
        if (lifeRestoration.magnitude > 0.0f)
        {
            Health.UpdateCurrentValue(lifeRestoration.GetRandom());

            if (Health.GetCurrentValue() > Health.GetFinalValue())
                Health.SetCurrentToFinalValue();

            OnHealthUIUpdate?.Invoke(Health.GetCurrentValue());
        }

        if (staminaRestoration.magnitude > 0.0f)
        {
            Stamina.UpdateCurrentValue(staminaRestoration.GetRandom());

            if (Stamina.GetCurrentValue() > Stamina.GetFinalValue())
                Stamina.SetCurrentToFinalValue();

            OnStaminaUIUpdate?.Invoke(Stamina.GetCurrentValue());
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
            removeAllInjuries();
            _postProcessingManager.ResetPostProcessing();
            _cameraController.StopCameraWobble();
            removeAllWounds();
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
        if (isMoving() && isRunning())
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
            OnStaminaUIUpdate?.Invoke(Stamina.GetCurrentValue());
            _staminaTrigger = 0.0f;
        }
    }

    public bool IsAlive()
    {
        return Health.GetCurrentValue() > 0.0f;
    }

    public static Stat GetStatByTypeStatic(StatType statType)
    {
        return _instance.getStatByType(statType);
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

    //*******
    //Weapons
    //*******

    private Transform _shootingSpot;

    [Header("Shooting settings - Read-only")]
    [SerializeField] private float _nonShootingDistance = 1.00f;
    [SerializeField] private float _closeQuarterShooting = 0.75f;
    [SerializeField] private float _shootingOffset = 0.5f;
    private bool _weaponEquipped = false;
    private bool _canShoot = true;
    private bool _isAutoShooting = false;
    private bool _canSwitchWeapons = true;
    private bool _canPutWeaponAway = true;
    private float _shootingInterval;
    private bool _intervalWeaponActivated = false;
    private bool _isReloading = false;
    private int _currentAmmo;
    private int _shotsFired;
    private int _shotsHit;

    [Header("Camera shake - Read-only")]
    [SerializeField] private float _cameraShakeDuration = 0.05f;
    [SerializeField] private float _cameraShakeMagnitude = 0.1f;

    [Header("Items")]
    [SerializeField] private List<Weapon> _weapons = new List<Weapon>();
    [SerializeField] private List<ThrowableWeapon> _throwables = new List<ThrowableWeapon>();

    private Weapon _currentWeapon;
    public Weapon CurrentWeapon => _currentWeapon;
    private ThrowableWeapon _currentThrowable;

    private void weaponsStartSetup()
    {
        if (_weapons.Count > 0)
            setWeapon(0);

        if (_throwables.Count > 0)
            _currentThrowable = _throwables[0];

        _uiCanvas.SetThrowableImage(_currentThrowable);
        if (_currentThrowable != null)
            _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);
    }

    private void keyboardInput()
    {
        if (_gameManager == null)
            return;

        if (!IsAlive() || !_gameManager.IsGameRunning() || _gameManager.IsPaused())
            return;

        if (Input.GetKeyDown(KeyCode.R) && _weaponEquipped)
            reloadWeapon();

        if (Input.GetKeyDown(KeyCode.E) && _canSwitchWeapons)
            switchWeapon(1);

        if (Input.GetKeyDown(KeyCode.Q) && _canSwitchWeapons)
            switchWeapon(-1);

        if (Input.GetKeyDown(KeyCode.Tab) && _canSwitchWeapons)
            switchThrowables();

        if (Input.GetKeyDown(KeyCode.F) && _canPutWeaponAway)
        {
            if (_weapons.Count == 0)
                return;

            _weaponEquipped = !_weaponEquipped;
            presentWeapon();
        }

        if (Input.GetKeyDown(KeyCode.G))
            useThrowable();
    }

    public void LeftClickDown()
    {
        if (_pointerOver.OverUI())
            return;

        if (_currentWeapon == null)
            return;

        if (!_weaponEquipped)
            return;

        if (_isReloading)
            return;

        if (!_canShoot)
            return;

        _canSwitchWeapons = false;
        _canPutWeaponAway = false;

        evaluateWeaponAndShoot();
    }

    private void evaluateWeaponAndShoot()
    {
        if (_currentWeapon.WeaponItem.ShootInterval == 0)
        {
            shootOnce();
            return;
        }

        if (_intervalWeaponActivated)
            return;

        if (_currentWeapon.WeaponItem.Automatic)
        {
            _isAutoShooting = true;
            return;
        }

        if (!_currentWeapon.WeaponItem.Automatic)
        {
            _intervalWeaponActivated = shootOnce();
            _shootingInterval = 0.0f;
            return;
        }
    }

    private void autoShootingProcedure()
    {
        if (!_isAutoShooting || _isReloading)
            return;

        _shootingInterval += Time.deltaTime;

        if (_shootingInterval >= _currentWeapon.WeaponItem.ShootInterval)
        {
            shootOnce();
            _intervalWeaponActivated = false;
            _shootingInterval = 0.0f;
        }
    }

    private void intervalWeaponProcedure()
    {
        if (!_intervalWeaponActivated)
            return;

        _shootingInterval += Time.deltaTime;

        if (_shootingInterval >= _currentWeapon.WeaponItem.ShootInterval)
        {
            _intervalWeaponActivated = false;
            _shootingInterval = 0.0f;
        }
    }

    public void LeftClickUp()
    {
        _isAutoShooting = false;
        _canSwitchWeapons = true;
        _canPutWeaponAway = true;
    }

    private void useThrowable()
    {
        if (_throwables.Count == 0)
        {
            string message = "I have nothing to throw!";
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, message, Color.white);
            return;
        }

        Vector3 vectorFromViewAngle = (Vector3)Utilities.GetVectorFromAngle(transform.rotation.eulerAngles.z);

        Vector3 forceVector = _currentThrowable.ThrowableItem.ThrowForce * vectorFromViewAngle;
        Vector3 throwPosition = transform.position + 1.5f * vectorFromViewAngle;

        Transform throwableTransform;
        switch (_currentThrowable.ThrowableItem.Type)
        {
            case ThrowableWeaponType.Mine:
                throwableTransform =
                    Instantiate(GameAssets.Instance.Mine, throwPosition, Quaternion.identity, null);
                Mine mine = throwableTransform.GetComponent<Mine>();
                mine.ArmMine();
                mine.ThrowMine(forceVector);
                break;
            case ThrowableWeaponType.Grenade:
                throwableTransform =
                    Instantiate(GameAssets.Instance.Grenade, transform.position, Quaternion.identity, null);
                Grenade grenade = throwableTransform.GetComponent<Grenade>();
                grenade.ArmGrenade();
                grenade.ThrowGrenade(forceVector);
                break;
            case ThrowableWeaponType.FlashGrenade:
                throwableTransform =
                    Instantiate(GameAssets.Instance.FlashGrenade, transform.position, Quaternion.identity, null);
                FlashGrenade flashGrenade = throwableTransform.GetComponent<FlashGrenade>();
                flashGrenade.ArmGrenade();
                flashGrenade.ThrowGrenade(forceVector);
                break;
            default:
                return;
        }

        _currentThrowable.TotalAmmo--;
        _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);

        if (_currentThrowable.TotalAmmo == 0)
        {
            _throwables.Remove(_currentThrowable);

            if (_throwables.Count > 0)
            {
                _currentThrowable = _throwables[0];
                _uiCanvas.SetThrowableImage(_currentThrowable);
            }
            else
            {
                _currentThrowable = null;
                _uiCanvas.SetThrowableImage(_currentThrowable);
            }
        }
    }

    private void switchThrowables()
    {
        if (_throwables.Count == 0)
            return;

        if (_currentThrowable == null)
            return;

        int currentIndex = _throwables.IndexOf(_currentThrowable);

        currentIndex++;

        if (currentIndex >= _throwables.Count)
            currentIndex = 0;

        _currentThrowable = _throwables[currentIndex];

        _uiCanvas.SetThrowableImage(_currentThrowable);
        _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);
    }

    public void EnableShooting(bool value)
    {
        _canShoot = value;
    }

    public void SetReloading(bool value)
    {
        _isReloading = value;
    }

    private void presentWeapon()
    {
        if (_weapons.Count == 0)
            return;

        if (_weaponEquipped)
            playAnimation(_currentWeapon.WeaponItem.Trigger);
        else
            playAnimation(AnimationType.RemoveWeapon);
    }

    private void switchWeapon(int leftRight)
    {
        if (_weapons.Count <= 1)
            return;

        int currentIndex = _weapons.IndexOf(_currentWeapon);

        if (leftRight == -1)
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = _weapons.Count - 1;
        }

        if (leftRight == 1)
        {
            currentIndex++;

            if (currentIndex == _weapons.Count)
                currentIndex = 0;
        }

        setWeapon(currentIndex);
        _uiCanvas.SetWeaponImage(_currentWeapon);
    }

    public void setWeapon(int index = 0)
    {
        if (index < 0)
            return;

        if (_currentWeapon != null)
        {
            _currentWeapon.TotalAmmo += _currentAmmo;
            _currentAmmo = 0;
        }

        _currentWeapon = _weapons[index];
        setCurrentWeaponAmmo(_currentWeapon.WeaponItem.MagazineBullets);

        PlayerDamage = new Vector2(
            _currentWeapon.WeaponItem.WeaponDamage.x + (Strength.GetFinalValue() - 2.0f) / 10.0f,
            _currentWeapon.WeaponItem.WeaponDamage.y + (Strength.GetFinalValue() - 2.0f) / 10.0f
            );

        _shootingInterval = _currentWeapon.WeaponItem.ShootInterval;

        _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

        if (_weaponEquipped)
            presentWeapon();
    }

    private void setCurrentWeaponAmmo(int ammoNeeded)
    {
        int ammoDelta = _currentWeapon.TotalAmmo - ammoNeeded;

        if (ammoDelta > 0)
        {
            _currentWeapon.TotalAmmo = ammoDelta;
            _currentAmmo = _currentWeapon.WeaponItem.MagazineBullets;
        }
        else
        {
            _currentAmmo += _currentWeapon.TotalAmmo;
            _currentWeapon.TotalAmmo = 0;
        }
    }

    public void IncrementShotsHit()
    {
        _shotsHit++;
        _accuracyPanel?.SetupAccuracyText((float)_shotsHit / _shotsFired);
    }

    public float GetAccuracy()
    {
        if (_shotsFired == 0)
            return 0.0f;
        else
            return (float)_shotsHit / _shotsFired;
    }

    private bool shootOnce()
    {
        if (_currentAmmo == 0)
        {
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Out of ammo!", Color.white);
            return false;
        }

        int bulletAmount = _currentWeapon.WeaponItem.Trigger.Equals(AnimationType.Shotgun) ? 5 : 1;

        for (int i = 0; i < bulletAmount; i++)
        {
            Vector2 direction = shootingDirection(_mousePosition);

            if (direction == Vector2.zero)
                return false;

            Transform bulletTransform =
                Instantiate(GameAssets.Instance.BulletPrefab, _shootingSpot.position, Quaternion.identity, null);

            Bullet bullet = bulletTransform.GetComponent<Bullet>();
            DamageData damageData = new DamageData { Damage = PlayerDamage.GetRandom() };
            bullet.SetupBullet(direction, damageData, gameObject.tag);
        }

        _shotsFired++;

        _cameraController?.ShakeCamera(_cameraShakeDuration, _cameraShakeMagnitude);

        generateShootingParticleSystem();
        alertNearbyEnemies();

        _currentAmmo--;

        if (_currentAmmo < 0)
            _currentAmmo = 0;

        _mouseCursor.OnShooting(_currentWeapon);
        _audioManager.PlayClip(_currentWeapon.WeaponItem.WeaponShootAudio);

        _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

        return true;
    }

    private void alertNearbyEnemies()
    {
        float soundRadius = 8.0f;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, soundRadius);
        foreach (Collider2D collider in colliders)
        {
            NpcAi npc2 = collider.GetComponent<NpcAi>();
            if (npc2 != null)
                npc2.AlertNPC();
        }
    }

    private Vector2 shootingDirection(Vector3 targetPosition)
    {
        float distanceFromShootingSpot = ((Vector2)targetPosition - (Vector2)_shootingSpot.position).magnitude;
        float distanceFromGameObject = ((Vector2)targetPosition - (Vector2)transform.position).magnitude;

        if (distanceFromGameObject < _nonShootingDistance)
            return Vector2.zero;

        if (distanceFromShootingSpot < _closeQuarterShooting)
            return (targetPosition + getRandomOffset(_shootingOffset, Accuracy.GetFinalValue()) - transform.position).normalized;
        else
            return (targetPosition + getRandomOffset(_shootingOffset, Accuracy.GetFinalValue()) - _shootingSpot.position).normalized;
    }

    private Vector3 getRandomOffset(float shootingOffset, float accuracy)
    {
        Vector2 right = Vector2.right * UnityEngine.Random.Range(-1.0f, 1.0f);
        Vector2 up = Vector2.up * UnityEngine.Random.Range(-1.0f, 1.0f);
        Vector2 randomOffset = (right + up) * shootingOffset / accuracy;

        return randomOffset;
    }

    private void reloadWeapon()
    {
        if (_currentWeapon == null)
            return;

        string weaponLoadedString = "Weapon fully loaded!";
        string noMoreAmmoString = "No more ammo!";

        if (_currentAmmo == _currentWeapon.WeaponItem.MagazineBullets)
        {
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, weaponLoadedString, Color.white);
            return;
        }

        if (_currentWeapon.TotalAmmo == 0)
        {
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, noMoreAmmoString, Color.white);
            return;
        }

        if (_currentAmmo == 0)
            setCurrentWeaponAmmo(_currentWeapon.WeaponItem.MagazineBullets);

        if (_currentAmmo > 0 && _currentAmmo < _currentWeapon.WeaponItem.MagazineBullets)
        {
            int ammoNeeded = _currentWeapon.WeaponItem.MagazineBullets - _currentAmmo;
            setCurrentWeaponAmmo(ammoNeeded);
        }

        _audioManager.PlayClip(_currentWeapon.WeaponItem.WeaponReloadAudio);
        _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

        playAnimation(AnimationType.Reload);
        presentWeapon();
    }

    public static bool AddAmmoStatic(AmmoItem ammo)
    {
        return _instance.addAmmo(ammo);
    }

    private bool addAmmo(AmmoItem ammo)
    {
        foreach (Weapon weapon in _weapons)
        {
            if (weapon.WeaponItem.AmmoType != ammo.AmmoType)
                continue;

            weapon.TotalAmmo += ammo.Amount;
            if (_currentWeapon == weapon)
                _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

            PlayerInventory.DeleteItemFromInventoryStatic(ammo);
            return true;
        }

        return false;
    }

    private void generateShootingParticleSystem()
    {
        Vector3 rotation = new Vector3(-1.0f * _shootingSpot.rotation.eulerAngles.z, 0.0f, 0.0f);

        ParticleSystem shootingPS = Instantiate(GameAssets.Instance.ShootingPS);
        shootingPS.transform.position = _shootingSpot.position;
        shootingPS.transform.Rotate(rotation);

        shootingPS.Play();
    }

    public bool AddThrowable(ThrowableWeapon throwable)
    {
        if (throwable == null)
            return false;

        for (int i = 0; i < _throwables.Count; i++)
        {
            if (throwable.ThrowableItem == _throwables[i].ThrowableItem)
            {
                _throwables[i].TotalAmmo++;
                _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

                return true;
            }
        }

        _throwables.Add(throwable);

        if (_currentThrowable == null)
            _currentThrowable = throwable;

        _uiCanvas.SetThrowableImage(_currentThrowable);
        _uiCanvas.UpdateThrowableAmmoText(_currentThrowable.TotalAmmo);

        return true;
    }

    public bool AddWeapon(Weapon weapon)
    {
        if (weapon == null)
            return false;

        //check strength requirement
        if (Strength.GetFinalValue() < weapon.WeaponItem.StrengthRequired)
        {
            List<string> stringList = new List<string> { "Not enough strength!", "I cannot use this!", "Too big piece..." };
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, stringList.GetRandomElement(), Color.white);
            return false;
        }

        for (int i = 0; i < _weapons.Count; i++)
        {
            if (weapon.WeaponItem == _weapons[i].WeaponItem)
            {
                Vector2Int ammoDropRange = new Vector2Int(10, 30);
                _weapons[i].TotalAmmo += UnityEngine.Random.Range(ammoDropRange.x, ammoDropRange.y);

                _ammoPanel.UpdateAmmoText(_currentAmmo, _currentWeapon.TotalAmmo);

                return true;
            }
        }

        _weapons.Add(weapon);

        if (_currentWeapon == null)
        {
            _currentWeapon = weapon;
            _uiCanvas.SetWeaponImage(_currentWeapon);
            setWeapon(0);
        }

        return true;
    }

    public List<Weapon> GetWeapons()
    {
        return _weapons;
    }

    public Weapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    private ThrowableWeapon getCurrentThrowable()
    {
        return _currentThrowable;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nonShootingDistance);
    }
}
