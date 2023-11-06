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

    private List<Achievement> _unlockedAchievements = new List<Achievement>();
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
            _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Lightning));
            Debug.Log(AchievementType.Lightning);
        }

        int enemiesKilled = _gameManager.GetEnemiesKilled();
        if (enemiesKilled == 0)
        {
            _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Pacifist));
            Debug.Log(AchievementType.Pacifist);
        }

        float playerMaxHealth = _playerStats.PlayerHealth.GetFinalValue();
        float playerHealth = _playerStats.PlayerHealth.GetCurrentValue();
        if (playerHealth <= 0.05f * playerMaxHealth)
        {
            _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Survivalist));
            Debug.Log(AchievementType.Survivalist);
        }

        float totalHealthLoss = _playerStats.GetTotalHealthLoss();
        if (totalHealthLoss <= 0.1f * playerMaxHealth)
        {
            _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Ironman));
            Debug.Log(AchievementType.Ironman);
        }

        float accuracy = _playerWeapons.GetAccuracy();
        if (accuracy >= 0.8f)
        {
            _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Sniper));
            Debug.Log(AchievementType.Sniper);
        }
    }

    public void CheckOnNpcKilled()
    {
        int enemiesKilled = _gameManager.GetEnemiesKilled();
        if (enemiesKilled == 50)
        {
            _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Bloodthirst));
            Debug.Log(AchievementType.Bloodthirst);
        }
    }

    public void CheckOnItemCrafted(Item item)
    {
        _itemsCrafted++;
        if (_itemsCrafted == 20)
        {
            _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Crafter));
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
                _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Medic));
                Debug.Log(AchievementType.Medic);
            }
        }
    }

    public void CheckOnTerminalHacked()
    {
        _terminalsHacked++;
        if (_terminalsHacked == 5)
        {
            _unlockedAchievements.AddIfNone(AchievementConstants.GetAchievement(AchievementType.Hacker));
            Debug.Log(AchievementType.Hacker);
        }
    }
}
