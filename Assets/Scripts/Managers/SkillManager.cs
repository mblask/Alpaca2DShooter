using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static SkillManager _instance;
    public static SkillManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] private List<SkillSO> _unlockedSkills = new List<SkillSO>();

    private GameAssets _gameAssets;

    private void Awake()
    {
        if (_instance != null)
            Destroy(_instance.gameObject);

        _instance = this;
        DontDestroyOnLoad(this);
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

    public List<SkillSO> GetUnlockedSkills()
    {
        return _unlockedSkills;
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
                return _gameAssets.GetSkillByStatType(StatType.Hacking);
            default:
                return null;
        }
    }
}
