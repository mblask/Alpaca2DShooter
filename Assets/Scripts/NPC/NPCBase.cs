using UnityEngine;
using AlpacaMyGames;
using System.Collections.Generic;

public class NPCBase : MonoBehaviour
{
    private Animator _animator;
    private NPCStats _enemyStats;

    private CharacterBaseScriptable _npcCharacterBase;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyStats = GetComponent<NPCStats>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;

        setCharacterBase();
    }

    private void setCharacterBase()
    {
        _npcCharacterBase = getRandomCharacterBaseScriptable();

        if (_npcCharacterBase == null)
        {
            Debug.LogError("There are no scriptable character bases!");
            return;
        }

        float npcHealthModifier = 0.3f;

        _animator.runtimeAnimatorController = _npcCharacterBase.CharacterAOC;
        _enemyStats.EnemySpeed.SetBaseValue(_npcCharacterBase.MovementSpeed);
        _enemyStats.EnemyAccuracy.SetBaseValue(_npcCharacterBase.Accuracy);
        _enemyStats.EnemyHealth.SetBaseValue(_npcCharacterBase.Health * npcHealthModifier);
        _enemyStats.EnemyDefense.SetBaseValue(_npcCharacterBase.Defense);
        _enemyStats.ModifyStats();
    }

    private CharacterBaseScriptable getRandomCharacterBaseScriptable()
    {
        CharacterBaseScriptable playerBase = PlayerBase.Instance.GetCharacterBaseScriptable();
        List<CharacterBaseScriptable> availableBases = new List<CharacterBaseScriptable>();
        foreach (CharacterBaseScriptable scriptable in _gameAssets.CharacterBaseScriptableList)
            if (scriptable.CharacterType != playerBase.CharacterType)
                availableBases.Add(scriptable);

        CharacterBaseScriptable characterBase = availableBases.GetRandomElement();
        return characterBase;
    }

    public CharacterBaseScriptable GetCharacterBaseScriptable()
    {
        return _npcCharacterBase;
    }
}
