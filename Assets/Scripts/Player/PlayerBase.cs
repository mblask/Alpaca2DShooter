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

    private CharacterBaseScriptable _playerCharacterBaseScriptable;

    private GameAssets _gameAssets;
    private PlayerSelector _playerSelector;
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
        _playerSelector = PlayerSelector.Instance;
        _gameAssets = GameAssets.Instance;
        selectCharacter();
    }

    private void selectCharacter()
    {
        try
        {
            CharacterBaseType baseType = _playerSelector.GetSelected();
            _playerSelector.DestroySelf();
            if (baseType.Equals(CharacterBaseType.None))
                setupCharacter(_gameAssets.CharacterBaseScriptableList[0]);

            setupCharacter(_gameAssets.GetBaseScriptabeByType(baseType));
        }
        catch
        {
            setupCharacter(_gameAssets.CharacterBaseScriptableList[0]);
        }
    }

    private void setupCharacter(CharacterBaseScriptable characterBase)
    {
        _playerCharacterBaseScriptable = characterBase;
        _playerAnimations.SetPlayerAOC(characterBase.CharacterAOC);

        _playerStats.PlayerAccuracy.SetBaseValue(characterBase.Accuracy);
        _playerStats.PlayerSpeed.SetBaseValue(characterBase.MovementSpeed * 1.2f);
        _playerStats.PlayerHealth.SetBaseValue(characterBase.Health);
        _playerStats.PlayerStamina.SetBaseValue(characterBase.Stamina);
        _playerStats.PlayerDefense.SetBaseValue(characterBase.Defense);
        _playerStats.PlayerStrength.SetBaseValue(characterBase.Strength);
        _playerStats.HackingSpeed.SetBaseValue(characterBase.HackingSpeed);
        _playerStats.LimbToughness.SetBaseValue(characterBase.LimbToughness);

        foreach (BaseStat stat in characterBase.BaseStats)
        {
            float value = stat.Value;
            if (stat.Type.Equals(StatType.Speed))
                value *= 1.2f;

            if (stat.Type.Equals(StatType.Health))
                value *= characterBase.HealthModifier;

            _playerStats.Stats.Add(new Stat(stat.Type, value));
        }
    }

    public CharacterBaseScriptable GetCharacterBaseScriptable()
    {
        return _playerCharacterBaseScriptable;
    }
}
