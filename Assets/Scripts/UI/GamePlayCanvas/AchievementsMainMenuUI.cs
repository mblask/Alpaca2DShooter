using System.Collections.Generic;
using UnityEngine;

public class AchievementsMainMenuUI : MonoBehaviour
{
    private Transform _achievementContainer;
    private AchievementManager _achievementManager;
    private GameAssets _gameAssets;

    private void Awake()
    {
        _achievementContainer = transform.Find("Container").Find("Container").Find("ScrollView").Find("Viewport").Find("Content");
    }

    private void Start()
    {
        _achievementManager = AchievementManager.Instance;
        _gameAssets = GameAssets.Instance;
    }

    public void PopulateAchievements()
    {
        foreach (Transform item in _achievementContainer)
            Destroy(item.gameObject);

        List<Achievement> unlocked = _achievementManager.GetUnlockedAchievements();
        foreach (Achievement achievement in unlocked)
        {
            SingleAchievementUi single = Instantiate(_gameAssets.SingleAchievementUI, _achievementContainer)
                .GetComponent<SingleAchievementUi>();

            single.SetupUI(achievement);
        }
    }
}
