using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimePanel : MonoBehaviour
{
    private TextMeshProUGUI _timeRemainingText;

    private void Awake()
    {
        _timeRemainingText = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.Instance.OnTimeUpdated += UpdateTime;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnTimeUpdated -= UpdateTime;
    }

    public void UpdateTime(float time)
    {
        if (time < 0.0f)
            return;

        _timeRemainingText.SetText(time.ToString("F1"));
    }
}
