using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static SkillManager _instance;
    public static SkillManager INstance
    {
        get
        {
            return _instance;
        }
    }

    private List<SkillSO> _unlockedSkills = new List<SkillSO>();

    private GameAssets _gameAssets;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
    }

    public void UnlockSkill(AchievementType achievement)
    {
        SkillSO skill = GetSkill(achievement);

        if (skill != null)
            _unlockedSkills.AddIfNone(skill);
    }

    private SkillSO GetSkill(AchievementType achievement)
    {
        switch (achievement)
        {
            case AchievementType.Lightning:
                return _gameAssets.GetSkillByStatType(StatType.Speed);
            case AchievementType.Bloodthirst:
                return _gameAssets.GetSkillByStatType(StatType.Damage);
            case AchievementType.Ironman:
                return _gameAssets.GetSkillByStatType(StatType.Defense);
            case AchievementType.Survivalist:
                return _gameAssets.GetSkillByStatType(StatType.LimbToughness);
            case AchievementType.Medic:
                return _gameAssets.GetSkillByStatType(StatType.Health);
            case AchievementType.Sniper:
                return _gameAssets.GetSkillByStatType(StatType.Accuracy);
            case AchievementType.Hacker:
                return _gameAssets.GetSkillByStatType(StatType.HackingSpeed);
            default:
                return null;
        }
    }
}
