using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccuracyPanel : Singleton<AccuracyPanel>
{
    private TextMeshProUGUI _accuracyText;

    public override void Awake()
    {
        _accuracyText = transform.Find("AccuracyText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        PlayerWeapons.Instance.OnEnemyHit += SetupAccuracyText;
        
        SetupAccuracyText(0.0f);
    }

    private void OnDisable()
    {
        if (PlayerWeapons.Instance != null)
            PlayerWeapons.Instance.OnEnemyHit -= SetupAccuracyText;
    }

    private void SetupAccuracyText(float accuracy)
    {
        accuracy *= 100.0f;
        _accuracyText.SetText(accuracy.ToString("F0") + "%");
    }
}
