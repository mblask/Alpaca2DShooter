using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class PlayerHitManager : MonoBehaviour
{
    private CameraController _mainCameraController;
    private PostProcessingManager _postProcessingManager;

    private void Start()
    {
        _mainCameraController = Camera.main.GetComponent<CameraController>();
        _postProcessingManager = PostProcessingManager.Instance;
    }

    public void CheckHit()
    {
        Color hitTextColor = Color.red;

        float headShotChance = 1.0f;
        float legShotChance = 3.0f;
        float armShotChance = 5.0f;

        if (Utilities.ChanceFunc(headShotChance))
        {
            //head hit, activate postprocessing and wobbling
            _mainCameraController.WobbleCamera(true);
            _postProcessingManager.ActivatePostProcessing();

            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Head shot!", hitTextColor, 0.5f, 5.0f, 2.0f, false, FloatDirection.Up);
            //Audio trigger
        }
        else if (Utilities.ChanceFunc(legShotChance))
        {
            //leg hit, decrease movement speed
            float legInjuryDuration = 8.0f;
            float speedBaseMultiplier = 0.6f;
            PlayerStats.TemporarilyModifyStat(StatType.Speed, legInjuryDuration, true, 0.0f, speedBaseMultiplier);
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Leg Hit!", hitTextColor, 0.5f, 5.0f, 2.0f, false, FloatDirection.Down);
            //Audio trigger
        }
        else if (Utilities.ChanceFunc(armShotChance))
        {
            //arm hit, decrease accuracy
            float armInjuryDuration = 10.0f;
            float accuracyBaseMultiplier = 0.1f;
            PlayerStats.TemporarilyModifyStat(StatType.Accuracy, armInjuryDuration, true, 0.0f, accuracyBaseMultiplier);
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Arm Hit!", hitTextColor, 0.5f, 5.0f, 2.0f, false, FloatDirection.Any);
            //Audio trigger
        }
        else
            return;
    }
}
