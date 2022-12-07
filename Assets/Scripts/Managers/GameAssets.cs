using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterBaseType
{
    Black,
    Blue,
    Brown,
    Old,
    Robot,
    Soldier,
    Survivor,
    Woman,
}

public class GameAssets : Singleton<GameAssets>
{
    public const string CHARACTER_TYPE_STRING = "CharacterType";

    [Header("UI Prefabs")]
    public Transform HeadshotPostProcessing;
    public Transform PossibleCraftUI;
    public Transform ItemSlotUIPrefab;

    [Header("Object Prefabs")]
    public Transform NPCPrefab;
    public Transform ItemTemplate;
    public Transform BulletPrefab;
    public Transform SpawnPortal;
    public Transform ExitPortal;
    public Transform ChatBubble;
    public Transform Blood;

    [Header("Particle Systems")]
    public GameObject ObjectDestroyPS;
    public GameObject BloodPS;
    public ParticleSystem ShootingPS;

    [Header("Available Weapons")]
    public List<WeaponItem> AvailableWeaponsList;

    [Header("Character Base List")]
    public List<CharacterBase> CharacterBaseList;
    public List<CharacterBaseScriptable> CharacterBaseScriptableList;

    //public CharacterBase GetCharacterBase(CharacterBaseType characterBaseType)
    //{
    //    foreach (CharacterBase characterBase in CharacterBaseList)
    //    {
    //        if (characterBase.Equals(characterBaseType))
    //            return characterBase;
    //    }
    //
    //    return null;
    //}
}
