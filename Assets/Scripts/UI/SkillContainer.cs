using System.Collections.Generic;
using UnityEngine;

public class SkillContainer : MonoBehaviour
{
    private Transform _container;

    private List<SkillSO> _availableSkills;
    
    private GameAssets _gameAssets;
    private SkillManager _skillManager;

    private void Awake()
    {
        _container = transform.Find("Container");
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;
        _skillManager = SkillManager.Instance;
        _availableSkills = _skillManager.GetUnlockedSkills();

        _container.gameObject.SetActive(_availableSkills.Count > 0);
        foreach (SkillSO skill in _availableSkills)
        {
            SkillButton button = Instantiate(_gameAssets.SkillButton, _container).GetComponent<SkillButton>();
            button.SetupButton(skill);
        }
    }
}
