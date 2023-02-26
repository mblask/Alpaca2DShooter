using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

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

        _animator.runtimeAnimatorController = _npcCharacterBase.CharacterAOC;
        _enemyStats.EnemySpeed.SetBaseValue(_npcCharacterBase.MovementSpeed);
        _enemyStats.EnemyAccuracy.SetBaseValue(_npcCharacterBase.Accuracy);
        _enemyStats.EnemyHealth.SetBaseValue(_enemyStats.EnemyHealth.GetBaseValue() * _npcCharacterBase.HealthModifier);
        _enemyStats.ModifyStats();
    }

    private CharacterBaseScriptable getRandomCharacterBaseScriptable()
    {
        CharacterBaseScriptable characterBase;

        do
        {
            characterBase = _gameAssets.CharacterBaseScriptableList.GetRandomElement();
        } while (characterBase.Equals(PlayerBase.Instance.GetCharacterBaseScriptable()));

        return characterBase;
    }

    public CharacterBaseScriptable GetCharacterBaseScriptable()
    {
        return _npcCharacterBase;
    }
}
