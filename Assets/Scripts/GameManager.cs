using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public delegate void OnCollectedArtefacts();
    public OnCollectedArtefacts onCollectedArtefacts;

    public int ArtefactsRequiredToPassLevel;

    [SerializeField] private float _timeToPassLevel = 20.0f;

    private bool _artefactsAcquired = false;
    private bool _canExitLevel = false;
    public bool CanExitLevel
    {
        get
        {
            return _canExitLevel;
        }
    }

    #region Singleton
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
            
    }
    #endregion

    private void Update()
    {
        GameTimeMethod();
    }

    public void GameTimeMethod()
    {
        if (_timeToPassLevel > 0.0f && !_artefactsAcquired)

        {
            _timeToPassLevel -= Time.deltaTime;

            GamePlayCanvas.Instance.UpdateTime(_timeToPassLevel);

            if (_timeToPassLevel <= 0.0f)
            {
                GamePlayCanvas.Instance.ActivateGameOverScreen();
                Debug.Log("Game over! Restart level!");
                //SceneManager.LoadScene("SandboxScene");
            }
        }
    }

    public void UpdateCollectedArtefacts(int value)
    {
        if (value == 0)
            return;

        GamePlayCanvas.Instance.UpdateArtefactsText(value);

        if (value == ArtefactsRequiredToPassLevel)
        {
            _artefactsAcquired = true;
            _canExitLevel = true;
            Debug.Log("All artefacts collected! Get out of the level!");

            if (onCollectedArtefacts != null)
                onCollectedArtefacts.Invoke();
        }
    }
}
