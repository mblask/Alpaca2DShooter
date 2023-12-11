using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    private static PlayerSelector _instance;
    public static PlayerSelector Instance
    {
        get
        {
            return _instance;
        }
    }

    private CharacterBaseType _selectedPlayerBase;
    [SerializeField] private List<SkillSO> _selectedSkills = new List<SkillSO>();

    private void Awake()
    {
        if (_instance != null)
            Destroy(_instance.gameObject);

        _instance = this;

        DontDestroyOnLoad(this);
    }

    public void SelectPlayerBase(CharacterBaseType baseType)
    {
        _selectedPlayerBase = baseType;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public CharacterBaseType GetSelectedBase()
    {
        return _selectedPlayerBase;
    }

    public void AddSkill(SkillSO skill)
    {
        if (!_selectedSkills.Contains(skill))
            _selectedSkills.Add(skill);
    }

    public void RemoveSkill(SkillSO skill)
    {
        if (_selectedSkills.Contains(skill))
            _selectedSkills.Remove(skill);
    }

    public List<SkillSO> GetSelectedSkills()
    {
        return _selectedSkills;
    }
}
