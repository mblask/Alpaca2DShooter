using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : Singleton<PlayerBase>
{
    private CharacterBase _playerCharacterBase;
    private CharacterBaseScriptable _playerCharacterBaseScriptable;

    private PlayerAnimations _playerAnimations;
    private PlayerStats _playerStats;

    public override void Awake()
    {
        base.Awake();

        _playerAnimations = GetComponent<PlayerAnimations>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        string characterType = PlayerPrefs.GetString(GameAssets.CHARACTER_TYPE_STRING);

        //if (characterType == null)
        //    SetCharacterBase(GameAssets.Instance?.CharacterBaseList[1]);
        //else
        //{
        //    SetCharacterBase(characterType);
        //}

        if (characterType == null)
            setupCharacter(GameAssets.Instance?.CharacterBaseScriptableList[1]);
        else
            setCharacterBase2(characterType);
    }

    private void setupCharacter(CharacterBaseScriptable characterBase)
    {
        _playerCharacterBaseScriptable = characterBase;

        _playerAnimations.SetPlayerAOC(characterBase.CharacterAOC);
        _playerStats.PlayerAccuracy.SetBaseValue(characterBase.Accuracy);
        _playerStats.PlayerSpeed.SetBaseValue(_playerStats.PlayerSpeed.GetBaseValue() + characterBase.MovementSpeed / 5.0f);
        _playerStats.PlayerHealth.SetBaseValue(_playerStats.PlayerHealth.GetBaseValue() * characterBase.HealthModifier);
        _playerStats.PlayerStrength.SetBaseValue(characterBase.Strength);
    }

    private void setupCharacter(CharacterBase characterBase)
    {
        _playerCharacterBase = characterBase;

        _playerAnimations.SetPlayerAOC(characterBase.CharacterAOC);
        _playerStats.PlayerAccuracy.SetBaseValue(characterBase.Accuracy);
        _playerStats.PlayerSpeed.SetBaseValue(_playerStats.PlayerSpeed.GetBaseValue() + characterBase.MovementSpeed / 5);
        _playerStats.PlayerHealth.SetBaseValue(_playerStats.PlayerHealth.GetBaseValue() * characterBase.HealthModifier);
    }

    private void setCharacterBase2(string characterBaseType)
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

    private void setCharacterBase(string characterBaseType)
    {
        if (GameAssets.Instance == null)
            return;

        bool characterSet = false;
        foreach (CharacterBase characterBase in GameAssets.Instance.CharacterBaseList)
        {
            if (characterBase.CharacterType.ToString().Equals(characterBaseType))
            {
                setupCharacter(characterBase);
                characterSet = true;
                break;
            }
        }

        if (!characterSet)
            setupCharacter(GameAssets.Instance.CharacterBaseList[1]);
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
