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

    [SerializeField] private List<Achievement> _unlockedAchievements = new List<Achievement>();
    private int _itemsCrafted;
    private int _medicItemsUsed;
    private int _terminalsHacked;

    private GameManager _gameManager;
    private PlayerStats _playerStats;
    private PlayerWeapons _playerWeapons;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _playerStats = PlayerStats.Instance;
        _playerWeapons = PlayerWeapons.Instance;
    }

    public void CheckOnGameFinished()
    {
        float gameTime = _gameManager.GetGameTime();
        if (gameTime <= 120.0f)
        {
            Achievement achievement = 
                AchievementConstants.GetAchievement(AchievementType.Lightning, gameTime.ToString());
            _unlockedAchievements.AddIfNone(achievement);
            Debug.Log(AchievementType.Lightning);
        }

        int enemiesKilled = _gameManager.GetEnemiesKilled();
        if (enemiesKilled == 0)
        {
            Achievement achievement = AchievementConstants.GetAchievement(AchievementType.Pacifist);
            _unlockedAchievements.AddIfNone(achievement);
            Debug.Log(AchievementType.Pacifist);
        }

        float playerMaxHealth = _playerStats.PlayerHealth.GetFinalValue();
        float playerHealth = _playerStats.PlayerHealth.GetCurrentValue();
        if (playerHealth <= 0.05f * playerMaxHealth)
        {
            Achievement achievement = AchievementConstants.GetAchievement
                (AchievementType.Survivalist, (playerHealth / playerMaxHealth * 100.0f).ToString());
            _unlockedAchievements.AddIfNone(achievement);
            Debug.Log(AchievementType.Survivalist);
        }

        float totalHealthLoss = _playerStats.GetTotalHealthLoss();
        if (totalHealthLoss <= 0.1f * playerMaxHealth)
        {
            Achievement achievement = AchievementConstants.GetAchievement
                (AchievementType.Ironman, (totalHealthLoss / playerMaxHealth * 100.0f).ToString());
            _unlockedAchievements.AddIfNone(achievement);
            Debug.Log(AchievementType.Ironman);
        }

        float accuracy = _playerWeapons.GetAccuracy();
        if (accuracy >= 0.8f)
        {
            Achievement achievement = 
                AchievementConstants.GetAchievement(AchievementType.Sniper, accuracy.ToString());
            _unlockedAchievements.AddIfNone(achievement);
            Debug.Log(AchievementType.Sniper);
        }
    }

    public void CheckOnNpcKilled(int enemiesKilled)
    {
        if (enemiesKilled == 50)
        {
            Achievement achievement = AchievementConstants.GetAchievement(AchievementType.Bloodthirst);
            _unlockedAchievements.AddIfNone(achievement);
            Debug.Log(AchievementType.Bloodthirst);
        }
    }

    public void CheckOnItemCrafted(Item item)
    {
        _itemsCrafted++;
        if (_itemsCrafted == 20)
        {
            Achievement achievement = AchievementConstants.GetAchievement(AchievementType.Crafter);
            _unlockedAchievements.AddIfNone(achievement);
            Debug.Log(AchievementType.Crafter);
        }
    }

    public void CheckOnItemUsed(Item item)
    {
        if (item is ConsumableItem)
        {
            _medicItemsUsed++;
            if (_medicItemsUsed == 20)
            {
                Achievement achievement = AchievementConstants.GetAchievement(AchievementType.Medic);
                _unlockedAchievements.AddIfNone(achievement);
                Debug.Log(AchievementType.Medic);
            }
        }
    }

    public void CheckOnTerminalHacked()
    {
        _terminalsHacked++;
        if (_terminalsHacked == 5)
        {
            Achievement achievement = AchievementConstants.GetAchievement(AchievementType.Hacker);
            _unlockedAchievements.AddIfNone(achievement);
            Debug.Log(AchievementType.Hacker);
        }
    }
}
