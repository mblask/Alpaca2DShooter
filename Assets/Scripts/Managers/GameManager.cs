using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public event Action<float> OnTimeUpdated;
    public event Action<int> OnArtefactsUpdated;
    public event Action<GameEndType, int> OnGameComplete;
    public event Action OnTogglePause;

    [Header("Read-only")]
    [SerializeField] private float _levelTime;
    private float _incrementedTime;
    private float _timeIncrement = 0.1f;

    [SerializeField] private int _enemiesKilled = 0;

    private bool _gameIsRunning = true;
    private bool _isPaused = false;

    private void Start()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnPlayerDeath += TriggerFailure;
        NPCStats.OnEnemyDeath += EnemyStats_OnEnemyDeath;
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnPlayerDeath -= TriggerFailure;

        NPCStats.OnEnemyDeath -= EnemyStats_OnEnemyDeath;
    }

    private void EnemyStats_OnEnemyDeath(NPCStats enemyStats)
    {
        _enemiesKilled++;
    }

    private void Update()
    {
        if (_gameIsRunning)
        {
            timeManager();

            if (Input.GetKeyDown(KeyCode.Escape))
                OnTogglePause?.Invoke();
        }
    }

    private void timeManager()
    {
        _levelTime += Time.deltaTime;

        _incrementedTime += Time.deltaTime;

        if (_incrementedTime >= _timeIncrement)
        {
            OnTimeUpdated?.Invoke(_levelTime);
            _incrementedTime -= _timeIncrement;
        }
    }

    public void TriggerVictory()
    {
        Debug.Log("GameManager: TriggerVictory");
    }

    public void TriggerFailure()
    {
        Debug.Log("GameManager: TriggerFailure");
    }

    public void SetPaused(bool value)
    {
        _isPaused = value;
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    public bool IsGameRunning()
    {
        return _gameIsRunning;
    }

    public void SetGameRunning(bool value)
    {
        _gameIsRunning = value;
    }

    private void resetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    public void ExitGame()
    {
        resetTimeScale();
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        resetTimeScale();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
