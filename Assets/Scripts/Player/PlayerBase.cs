using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    private static PlayerBase _instance;
    public static PlayerBase Instance
    {
        get
        {
            return _instance;
        }
    }

    private CharacterBase _playerCharacterBase;
    private CharacterBaseScriptable _playerCharacterBaseScriptable;

    private PlayerAnimations _playerAnimations;
    private PlayerStats _playerStats;

    public void Awake()
    {
        _instance = this;

        _playerAnimations = GetComponent<PlayerAnimations>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        setupCharacter(GameAssets.Instance?.CharacterBaseScriptableList[0]);
    }

    private void setupCharacter(CharacterBaseScriptable characterBase)
    {
        _playerCharacterBaseScriptable = characterBase;

        _playerAnimations.SetPlayerAOC(characterBase.CharacterAOC);
        _playerStats.PlayerAccuracy.SetBaseValue(characterBase.Accuracy);
        _playerStats.PlayerSpeed
            .SetBaseValue(_playerStats.PlayerSpeed.GetBaseValue() + characterBase.MovementSpeed / 5.0f);
        _playerStats.PlayerHealth
            .SetBaseValue(_playerStats.PlayerHealth.GetBaseValue() * characterBase.HealthModifier);
        _playerStats.PlayerDefense.SetBaseValue(characterBase.Defense);
        _playerStats.PlayerStrength.SetBaseValue(characterBase.Strength);
        _playerStats.HackingSpeed = characterBase.HackingSpeed;

        _playerStats.Stats = new List<Stat>()
        {
            new Stat(StatType.Health, characterBase.Health),
            new Stat(StatType.Stamina, characterBase.Stamina),
            new Stat(StatType.Strength, characterBase.Strength),
            new Stat(StatType.Speed, characterBase.MovementSpeed),
            new Stat(StatType.Accuracy, characterBase.Accuracy),
            new Stat(StatType.Defense, characterBase.Defense)
        };
    }

    private void setCharacterBase(string characterBaseType)
    {
        if (GameAssets.Instance == null)
            return;

        bool characterSet = false;
        foreach (CharacterBaseScriptable characterBase in GameAssets.Instance.CharacterBaseScriptableList)
        {
            if (characterBase.CharacterType.ToString().Equals(characterBaseType))
            {
                setupCharacter(characterBase);
                characterSet = true;
                break;
            }
        }

        if (!characterSet)
            setupCharacter(GameAssets.Instance.CharacterBaseScriptableList[1]);
    }

    public CharacterBase GetCharacterBase()
    {
        return _playerCharacterBase;
    }

    public CharacterBaseScriptable GetCharacterBaseScriptable()
    {
        return _playerCharacterBaseScriptable;
    }
}
