using UnityEngine;
using AlpacaMyGames;
using System.Collections.Generic;
using System.Linq;

public class NPCBase : MonoBehaviour
{
    [SerializeField] private NPCEnemyType _enemyType;
    public NPCEnemyType EnemyType => _enemyType;
    [SerializeField] private int _bossLevel;
    public int BossLevel => _bossLevel;

    private Animator _animator;
    private NPCStats _npcStats;
    private NPCWeapons _npcWeapons;

    private CharacterBaseScriptable _npcCharacterBase;

    private GameAssets _gameAssets;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _npcStats = GetComponent<NPCStats>();
        _npcWeapons = GetComponent<NPCWeapons>();
    }

    private void Start()
    {
        if (_enemyType == NPCEnemyType.Boss)
            return;
        
        _gameAssets = GameAssets.Instance;
        setCharacterBase();
    }

    public void SetBossLevel(int level)
    {
        _bossLevel = level;
        _gameAssets = GameAssets.Instance;
        setCharacterBase();
    }

    private void setCharacterBase()
    {
        switch (_enemyType)
        {
            case NPCEnemyType.Normal:
                _npcCharacterBase = getRandomCharacterBaseScriptable();
                if (_npcCharacterBase == null)
                    Debug.LogError("There are no scriptable character bases!");
                break;
            case NPCEnemyType.Boss:
                BossScriptable bossScriptable = _gameAssets.BossScriptableList
                    .Where(boss => boss.Level == _bossLevel)
                    .ToList().GetRandomElement();
                if (bossScriptable == null)
                    Debug.LogError("There are no scriptable character bases!");
                _npcWeapons.InitializeBossWeapon(bossScriptable.WeaponOfChoice);
                _npcCharacterBase = bossScriptable;
                break;
        }

        _animator.runtimeAnimatorController = _npcCharacterBase.CharacterAOC;
        _npcStats.InitializeStats(_npcCharacterBase);
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
