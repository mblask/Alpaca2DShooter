using System.Collections.Generic;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    private List<SkillSO> _availableSkills;
    
    private GameAssets _gameAssets;
    private SkillManager _skillManager;

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _skillManager = SkillManager.Instance;
        _availableSkills = _skillManager.GetUnlockedSkills();

        foreach (SkillSO skill in _availableSkills)
        {
            SkillButton button = Instantiate(_gameAssets.SkillButton, transform).GetComponent<SkillButton>();
            button.SetupButton(skill);
        }
    }
}
