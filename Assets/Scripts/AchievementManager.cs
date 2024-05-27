using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager _instance;
    public static AchievementManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    private List<Achievement> _unlockedAchievements = new List<Achievement>();
    private int _itemsCrafted;
    private int _medicItemsUsed;
    private int _terminalsHacked;

    private GameManager _gameManager;
    private PlayerStats _playerStats;
    private PlayerWeapons _playerWeapons;
    private GamePlayCanvas _uiCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (_instance != null)
            Destroy(_instance.gameObject);

        _instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _playerStats = PlayerStats.Instance;
        _playerWeapons = PlayerWeapons.Instance;
        _uiCanvas = GamePlayCanvas.Instance;
    }

    public void CheckOnGameFinished()
    {
        List<AchievementType> achievementsUnlocked = new List<AchievementType>();

        float gameTime = _gameManager.GetGameTime();
        if (gameTime <= AchievementConstants.LIGHTNING_GOAL &&
            addAchievement(AchievementType.Lightning, gameTime.ToString()))
            achievementsUnlocked.Add(AchievementType.Lightning);

        int enemiesKilled = _gameManager.GetEnemiesKilled();
        if (enemiesKilled == AchievementConstants.PACIFIST_GOAL &&
            addAchievement(AchievementType.Pacifist))
            achievementsUnlocked.Add(AchievementType.Pacifist);

        float playerMaxHealth = _playerStats.Health.GetFinalValue();
        float playerHealth = _playerStats.Health.GetCurrentValue();
        if (playerHealth <= AchievementConstants.SURVIVALIST_GOAL * playerMaxHealth &&
            addAchievement(AchievementType.Survivalist, (playerHealth / playerMaxHealth * 100.0f).ToString()))
            achievementsUnlocked.Add(AchievementType.Survivalist);

        float totalHealthLoss = _playerStats.GetTotalHealthLoss();
        if (totalHealthLoss <= AchievementConstants.IRONMAN_GOAL * playerMaxHealth &&
            addAchievement(AchievementType.Ironman, (totalHealthLoss / playerMaxHealth * 100.0f).ToString()))
            achievementsUnlocked.Add(AchievementType.Ironman);

        float accuracy = _playerWeapons.GetAccuracy();
        if (accuracy >= AchievementConstants.SNIPER_GOAL &&
            addAchievement(AchievementType.Sniper, accuracy.ToString()))
            achievementsUnlocked.Add(AchievementType.Sniper);

        if (achievementsUnlocked.Count == 0)
            return;

        if (achievementsUnlocked.Count == 1)
        {
            _uiCanvas.AchievementUnlockedUI(achievementsUnlocked[0]);
            return;
        }

        _uiCanvas.ManyAchievementsUnlockedUI(achievementsUnlocked);
    }

    public void CheckOnNpcKilled(int enemiesKilled)
    {
        if (enemiesKilled != AchievementConstants.BLOODTHIRST_GOAL)
            return;

        triggerAchievement(AchievementType.Bloodthirst);
    }

    public void CheckOnItemCrafted()
    {
        _itemsCrafted++;
        if (_itemsCrafted != AchievementConstants.CRAFTER_GOAL)
            return;

        triggerAchievement(AchievementType.Crafter);
    }

    public void CheckOnItemUsed(Item item)
    {
        switch (item)
        {
            case ConsumableItem:
                _medicItemsUsed++;
                if (_medicItemsUsed != AchievementConstants.MEDIC_GOAL) return;
                triggerAchievement(AchievementType.Medic);
                break;
            default:
                break;
        }
    }

    public void CheckOnTerminalHacked()
    {
        _terminalsHacked++;
        if (_terminalsHacked != AchievementConstants.HACKER_GOAL)
            return;

        triggerAchievement(AchievementType.Hacker);
    }

    private bool addAchievement(AchievementType type, string value = null)
    {
        Achievement achievement = AchievementConstants.GetAchievement(type, value);
        return _unlockedAchievements.AddIfNone(achievement);
    }

    private void triggerAchievement(AchievementType type, string value = null)
    {
        if (!addAchievement(type, value))
            return;
        
        _uiCanvas.AchievementUnlockedUI(type);
    }

    public List<Achievement> GetUnlockedAchievements()
    {
        return _unlockedAchievements;
    }
}
