using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreInfo : MonoBehaviour
{
    private static ScoreInfo _instance;
    public static ScoreInfo Instance
    {
        get
        {
            return _instance;
        }
    }

    private Transform _containerTransform;
    private TextMeshProUGUI _scoreInfoText;

    public void Awake()
    {
        _instance = this;

        _containerTransform = transform.Find("Container");
        _scoreInfoText = _containerTransform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void SetupScoreInfo(float score)
    {
        _scoreInfoText.SetText("Score higher than " + score.ToString("F0") + " needed!");
        ShowContainer();
    }

    public void ShowContainer()
    {
        _containerTransform.gameObject.SetActive(true);
    }

    public void HideContainer()
    {
        _containerTransform.gameObject.SetActive(false);
    }
}
