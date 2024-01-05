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

        _playerStats.Accuracy.SetBaseValue(characterBase.Accuracy);
        _playerStats.Speed.SetBaseValue(characterBase.MovementSpeed * 1.2f);
        _playerStats.Health.SetBaseValue(characterBase.Health);
        _playerStats.Stamina.SetBaseValue(characterBase.Stamina);
        _playerStats.Defense.SetBaseValue(characterBase.Defense);
        _playerStats.Strength.SetBaseValue(characterBase.Strength);
        _playerStats.Hacking.SetBaseValue(characterBase.Hacking);
        _playerStats.LimbToughness.SetBaseValue(characterBase.LimbToughness);
        _playerStats.BodyArmor.SetBaseValue(0.0f);
    }

    private void setupPlayerSkills(List<SkillSO> skills)
    {
        foreach (SkillSO skill in skills)
        {
            switch (skill.Stat)
            {
                case StatType.Health:
                    UpdateStat(_playerStats.Health, skill);
                    break;
                case StatType.Stamina:
                    UpdateStat(_playerStats.Stamina, skill);
                    break;
                case StatType.Accuracy:
                    UpdateStat(_playerStats.Accuracy, skill);
                    break;
                case StatType.Damage:
                    _playerStats.PlayerDamage += Vector2.one * skill.Modifier;
                    _playerStats.PlayerDamage *= Vector2.one * skill.Multiplier;
                    break;
                case StatType.Defense:
                    UpdateStat(_playerStats.Defense, skill);
                    break;
                case StatType.Speed:
                    UpdateStat(_playerStats.Speed, skill);
                    break;
                case StatType.Strength:
                    UpdateStat(_playerStats.Strength, skill);
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
