using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;
    public static GameAssets Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("UI Prefabs")]
    public Transform WoundsTextObject;
    public Transform HeadshotPostProcessing;
    public Transform PossibleCraftUI;
    public Transform ItemSlotUIPrefab;
    public Transform CollectibleItemSlotUI;
    public Transform SkillButton;
    public Transform ItemSelectorButton;
    public Transform SingleAchievementUI;

    [Header("Object Prefabs")]
    public Transform DestructionArea;
    public Transform ItemTemplate;
    public Transform BulletPrefab;
    public Transform SpawnPortal;
    public Transform ExitPortal;
    public Transform FiringTrap;
    public Transform Blood;

    [Header("Character Prefabs")]
    public Transform Player;
    public Transform NPCPrefab;
    public Transform NPCBossPrefab;

    [Header("Throwable Prefabs")]
    public Transform Mine;
    public Transform Grenade;
    public Transform FlashGrenade;

    [Header("Particle Systems")]
    public ParticleSystem ObjectDestroyPS;
    public ParticleSystem BloodPS;
    public ParticleSystem ShootingPS;

    [Header("Consumables")]
    public List<ConsumableItem> Consumables;

    [Header("Available Weapons")]
    public List<WeaponItem> WeaponsList;

    [Header("Available Throwable Weapons")]
    public List<ThrowableItem> ThrowablesList;

    [Header("Available Ammo")]
    public List<AmmoItem> AvailableAmmoItems;

    [Header("Available Skills")]
    public List<SkillSO> AvailableSkills;

    [Header("Character Base List")]
    public List<CharacterBaseScriptable> CharacterBaseScriptableList;

    [Header("Boss Base List")]
    public List<BossScriptable> BossScriptableList;

    [Header("Mouse cursor")]
    public Transform MouseCursorObject;

    [Header("Controls")]
    public ControlKeyListScriptable ControlList;

    private void Awake()
    {
        if (_instance != null)
            Destroy(_instance.gameObject);

        _instance = this;
        DontDestroyOnLoad(this);
    }

    public CharacterBaseScriptable GetBaseScriptabeByType(CharacterBaseType type)
    {
        return CharacterBaseScriptableList.Find(baseType => baseType.CharacterType.Equals(type));
    }

    public SkillSO GetSkillByStatType(StatType type)
    {
        return AvailableSkills.Find(skill => skill.Stat.Equals(type));
    }

    public ConsumableItem GetConsumableByType(ConsumableType type)
    {
        return Consumables.Find(consumable => consumable.ConsumableType.Equals(type));
    }

    public ThrowableItem GetThrowableByType(ThrowableWeaponType type)
    {
        return ThrowablesList.Find(throwable => throwable.Type.Equals(type));
    }
}
