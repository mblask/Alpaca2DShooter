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
    private AchievementUI _achievementUI;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _playerStats = PlayerStats.Instance;
        _playerWeapons = PlayerWeapons.Instance;
        _achievementUI = AchievementUI.Instance;
    }

    public void CheckOnGameFinished()
    {
        float gameTime = _gameManager.GetGameTime();
        if (gameTime <= AchievementConstants.LIGHTNING_GOAL)
            triggerAchievement(AchievementType.Lightning, gameTime.ToString());

        int enemiesKilled = _gameManager.GetEnemiesKilled();
        if (enemiesKilled == AchievementConstants.PACIFIST_GOAL)
            triggerAchievement(AchievementType.Pacifist);

        float playerMaxHealth = _playerStats.PlayerHealth.GetFinalValue();
        float playerHealth = _playerStats.PlayerHealth.GetCurrentValue();
        if (playerHealth <= AchievementConstants.SURVIVALIST_GOAL * playerMaxHealth)
            triggerAchievement(AchievementType.Survivalist, (playerHealth / playerMaxHealth * 100.0f).ToString());

        float totalHealthLoss = _playerStats.GetTotalHealthLoss();
        if (totalHealthLoss <= AchievementConstants.IRONMAN_GOAL * playerMaxHealth)
            triggerAchievement(AchievementType.Ironman, (totalHealthLoss / playerMaxHealth * 100.0f).ToString());

        float accuracy = _playerWeapons.GetAccuracy();
        if (accuracy >= AchievementConstants.SNIPER_GOAL)
            triggerAchievement(AchievementType.Sniper, accuracy.ToString());
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

    private void triggerAchievement(AchievementType type, string value = null)
    {
        Debug.Log(type);
        Achievement achievement = AchievementConstants.GetAchievement(type, value);
        if (_unlockedAchievements.AddIfNone(achievement))
            _achievementUI.UpdateText(type);
    }
}
