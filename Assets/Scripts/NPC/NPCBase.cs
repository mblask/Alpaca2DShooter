using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class NPCBase : MonoBehaviour
{
    private Animator _animator;
    private NPCStats _enemyStats;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyStats = GetComponent<NPCStats>();
    }

    private void Start()
    {
        _gameAssets = GameAssets.Instance;

        //CharacterBase characterBase = generateRandomCharacterBase();
        //
        //if (characterBase != null)
        //{
        //    _animator.runtimeAnimatorController = characterBase.CharacterAOC;
        //    _enemyStats.EnemySpeed.SetBaseValue(characterBase.MovementSpeed);
        //    _enemyStats.EnemyAccuracy.SetBaseValue(characterBase.Accuracy);
        //    _enemyStats.EnemyHealth.SetBaseValue(_enemyStats.EnemyHealth.GetBaseValue() * characterBase.HealthModifier);
        //}

        CharacterBaseScriptable characterBaseScriptable = getRandomCharacterBaseScriptable();

        if (characterBaseScriptable != null)
        {
            _animator.runtimeAnimatorController = characterBaseScriptable.CharacterAOC;
            _enemyStats.EnemySpeed.SetBaseValue(characterBaseScriptable.MovementSpeed);
            _enemyStats.EnemyAccuracy.SetBaseValue(characterBaseScriptable.Accuracy);
            _enemyStats.EnemyHealth.SetBaseValue(_enemyStats.EnemyHealth.GetBaseValue() * characterBaseScriptable.HealthModifier);
        }
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

    private CharacterBase generateCharacterBase(int index)
    {
        if (index < 0)
            return null;

        if (index >= _gameAssets.CharacterBaseList.Count)
            return null;

        return _gameAssets.CharacterBaseList[index];
    }

    private CharacterBase generateRandomCharacterBase()
    {
        if (_gameAssets == null)
            return null;

        CharacterBase characterBase;

        do
        {
            int randomIndex = Random.Range(0, _gameAssets.CharacterBaseList.Count);
            characterBase = generateCharacterBase(randomIndex);

            //Debug.Log(characterBase.CharacterAOC.name);

        } while (characterBase == PlayerBase.Instance.GetCharacterBase());

        return characterBase;
    }
}
