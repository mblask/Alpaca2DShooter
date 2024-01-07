using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlpacaMyGames;

public class PlayerHitManager : MonoBehaviour
{
    private static PlayerHitManager _instance;

    public static PlayerHitManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private CameraController _mainCameraController;
    private PostProcessingManager _postProcessingManager;

    private List<WoundType> _woundsList = new List<WoundType>();

    private PlayerStats _playerStats;
    private WoundedUI _woundedUI;
    private GamePlayCanvas _uiCanvas;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _mainCameraController = Camera.main.GetComponent<CameraController>();
        _postProcessingManager = PostProcessingManager.Instance;
        _playerStats = PlayerStats.Instance;
        _woundedUI = WoundedUI.Instance;
        _uiCanvas = GamePlayCanvas.Instance;
    }

    public void CheckHit()
    {
        Color hitTextColor = Color.red;

        float headShotChance = 1.0f * (1 - _playerStats.LimbToughness.GetFinalValue());
        float legShotChance = 3.0f * (1 - _playerStats.LimbToughness.GetFinalValue());
        float armShotChance = 5.0f * (1 - _playerStats.LimbToughness.GetFinalValue());

        if (Utilities.ChanceFunc(headShotChance))
        {
            //head hit, activate postprocessing and wobbling
            float headInjuryDuration = 10.0f;
            _mainCameraController.WobbleCamera(true, headInjuryDuration);
            _postProcessingManager.ActivatePostProcessing(headInjuryDuration);

            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Head shot!", hitTextColor, 0.5f, 8.0f, 2.0f, true, FloatDirection.Up);

            _woundsList.AddIfNone(WoundType.Head);

            StartCoroutine(removeWoundTypeCoroutine(WoundType.Head, headInjuryDuration));
            //Audio trigger
        }
        else if (Utilities.ChanceFunc(legShotChance))
        {
            //leg hit, decrease movement speed
            float legInjuryDuration = 8.0f;
            float speedBaseMultiplier = 0.6f;
            PlayerStats.TemporarilyModifyStat(new StatModifyingData
            {
                StatAffected = StatType.Speed,
                Duration = legInjuryDuration,
                StatHandicaped = true,
                StatModifier = 0.0f,
                StatMultiplier = speedBaseMultiplier
            });
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Leg Hit!", hitTextColor, 0.5f, 8.0f, 2.0f, true, FloatDirection.Down);

            _woundsList.AddIfNone(WoundType.Legs);

            StartCoroutine(removeWoundTypeCoroutine(WoundType.Legs, legInjuryDuration));
            //Audio trigger
        }
        else if (Utilities.ChanceFunc(armShotChance))
        {
            //arm hit, decrease accuracy
            float armInjuryDuration = 10.0f;
            float accuracyBaseMultiplier = 0.3f;
            PlayerStats.TemporarilyModifyStat(new StatModifyingData {
                StatAffected = StatType.Accuracy, 
                Duration = armInjuryDuration, 
                StatHandicaped = true, 
                StatModifier = 0.0f, 
                StatMultiplier = accuracyBaseMultiplier 
            });
            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Arm Hit!", hitTextColor, 0.5f, 8.0f, 2.0f, true, FloatDirection.Any);

            _woundsList.AddIfNone(WoundType.Arms);

            StartCoroutine(removeWoundTypeCoroutine(WoundType.Arms, armInjuryDuration));
            //Audio trigger
        }
        else
            return;

        _uiCanvas.ActivateWoundedUI();
    }

    private IEnumerator removeWoundTypeCoroutine(WoundType woundType, float after)
    {
        float timer = 0.0f;

        while (timer < after)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        _woundsList.Remove(woundType);
        _uiCanvas.ActivateWoundedUI();
    }

    public void RemoveAllWounds()
    {
        _woundsList.Clear();
        _uiCanvas.ActivateWoundedUI();
    }

    public static List<WoundType> GetWoundsListStatic()
    {
        return _instance?.getWoundList();
    }

    private List<WoundType> getWoundList()
    {
        return _woundsList;
    }
}
