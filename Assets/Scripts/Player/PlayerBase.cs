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
        _playerAnimations.SetPlayerAOC(characterBase.CharacterAOC);

        _playerStats.PlayerAccuracy.SetBaseValue(characterBase.Accuracy);
        _playerStats.PlayerSpeed.SetBaseValue(characterBase.MovementSpeed * 1.2f);
        _playerStats.PlayerHealth.SetBaseValue(characterBase.Health);
        _playerStats.PlayerStamina.SetBaseValue(characterBase.Stamina);
        _playerStats.PlayerDefense.SetBaseValue(characterBase.Defense);
        _playerStats.PlayerStrength.SetBaseValue(characterBase.Strength);
        _playerStats.Hacking.SetBaseValue(characterBase.Hacking);
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

    private void setupPlayerSkills(List<SkillSO> skills)
    {
        foreach (SkillSO skill in skills)
        {
            switch (skill.Stat)
            {
                case StatType.Health:
                    UpdateStat(_playerStats.PlayerHealth, skill);
                    break;
                case StatType.Stamina:
                    UpdateStat(_playerStats.PlayerStamina, skill);
                    break;
                case StatType.Accuracy:
                    UpdateStat(_playerStats.PlayerAccuracy, skill);
                    break;
                case StatType.Damage:
                    _playerStats.PlayerDamage += Vector2.one * skill.Modifier;
                    _playerStats.PlayerDamage *= Vector2.one * skill.Multiplier;
                    break;
                case StatType.Defense:
                    UpdateStat(_playerStats.PlayerDefense, skill);
                    break;
                case StatType.Speed:
                    UpdateStat(_playerStats.PlayerSpeed, skill);
                    break;
                case StatType.Strength:
                    UpdateStat(_playerStats.PlayerStrength, skill);
                    break;
                case StatType.LimbToughness:
                    UpdateStat(_playerStats.LimbToughness, skill);
                    break;
                case StatType.Hacking:
                    UpdateStat(_playerStats.Hacking, skill);
                    break;
                default:
                    break;
            }
        }
    }

    private void UpdateStat(Stat stat, SkillSO skill)
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
}
