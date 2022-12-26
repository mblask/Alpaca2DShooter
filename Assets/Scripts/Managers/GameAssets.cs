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
    public Transform WoundsTextObject;
    public Transform HeadshotPostProcessing;
    public Transform PossibleCraftUI;
    public Transform ItemSlotUIPrefab;

    [Header("Object Prefabs")]
    public Transform Player;
    public Transform Mine;
    public Transform DestructionArea;
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

    [Header("Available Artefacts")]
    public List<ArtefactItem> AvailableArtefacts;

    [Header("Available Weapons")]
    public List<WeaponItem> AvailableWeaponsList;

    [Header("Character Base List")]
    public List<CharacterBase> CharacterBaseList;
    public List<CharacterBaseScriptable> CharacterBaseScriptableList;
}
