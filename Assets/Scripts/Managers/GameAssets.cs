using System.Collections;
using System.Collections.Generic;
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
    public Transform NPCBossPrefab;
    public Transform ItemTemplate;
    public Transform BulletPrefab;
    public Transform SpawnPortal;
    public Transform ExitPortal;
    public Transform FiringTrap;
    public Transform Blood;

    [Header("Particle Systems")]
    public ParticleSystem ObjectDestroyPS;
    public ParticleSystem BloodPS;
    public ParticleSystem ShootingPS;

    [Header("Available Artefacts")]
    public List<ArtefactItem> AvailableArtefacts;

    [Header("Available Weapons")]
    public List<WeaponItem> AvailableWeaponsList;

    [Header("Character Base List")]
    public List<CharacterBase> CharacterBaseList;
    public List<CharacterBaseScriptable> CharacterBaseScriptableList;

    private void Awake()
    {
        _instance = this;
    }
}
