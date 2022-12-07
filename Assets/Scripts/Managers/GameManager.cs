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
    public event Action<int, bool> OnActivateNamePanel;
    public event Action<GameEndType, int> OnGameComplete;
    public event Action OnTogglePause;

    [Header("Read-only")]
    [SerializeField] private float _levelTime;
    private float _incrementedTime;
    private float _timeIncrement = 0.1f;

    [SerializeField] private int _enemiesKilled = 0;

    private int _score = 0;
    private bool _gameIsRunning = true;
    private bool _isPaused = false;

    private void Start()
    {
        PlayerArtefacts.Instance.OnArtefactCollected += UpdateCollectedArtefacts;
        PlayerStats.Instance.OnPlayerDeath += TriggerFailure;
        NPCStats.OnEnemyDeath += EnemyStats_OnEnemyDeath;
        EnterNamePanel.OnNameEntered += EnterNamePanel_SaveScore;
    }

    private void OnDisable()
    {
        if (PlayerArtefacts.Instance != null)
            PlayerArtefacts.Instance.OnArtefactCollected -= UpdateCollectedArtefacts;
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnPlayerDeath -= TriggerFailure;

        NPCStats.OnEnemyDeath -= EnemyStats_OnEnemyDeath;
        EnterNamePanel.OnNameEntered -= EnterNamePanel_SaveScore;
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
        if (_gameIsRunning)
        {
            _gameIsRunning = false;

            _score = 0;

            int timeScore = Mathf.FloorToInt(100.0f * Mathf.Exp(50 / _levelTime));
            int accuracyScore = Mathf.FloorToInt(PlayerWeapons.Instance.GetAccuracy() * 100);
            int healthScore = Mathf.FloorToInt(PlayerStats.Instance.CurrentHealth);
            int enemyScore = _enemiesKilled * 30;

            _score = timeScore + accuracyScore + healthScore + enemyScore;

            checkHighscores(_score);
        }
    }

    private void checkHighscores(int score)
    {
        List<Highscore> savedHighscores = SaveManager.Load();

        if (savedHighscores != null)
        {
            if (savedHighscores.Count < 10)
            {
                //save the score
                //Debug.Log("Number of saved highscores less than 10. Save the score");
                if (score > savedHighscores[0].score)
                {
                    //Debug.Log("New highscore");
                    OnActivateNamePanel?.Invoke(score, true);
                }
                else
                {
                    OnActivateNamePanel?.Invoke(score, false);
                }
            }
            else
            {
                //check if the score is higher than the lowest score
                if (score > savedHighscores[savedHighscores.Count - 1].score)
                {
                    //save the score
                    //Debug.Log("Score higher than the lowest score in the highscore list. Save the score");
                    if (score > savedHighscores[0].score)
                    {
                        OnActivateNamePanel?.Invoke(score, true);
                    }
                    else
                    {
                        OnActivateNamePanel?.Invoke(score, false);
                    }
                }
                else
                {
                    //only show the score
                    //Debug.Log("Score lower than the lowest score in the highscore list. Only show the score");
                    OnGameComplete?.Invoke(GameEndType.Success, score);
                }
            }
        }
        else
        {
            //Debug.Log("Save first highscore");
            OnActivateNamePanel?.Invoke(score, true);
        }
    }

    public void EnterNamePanel_SaveScore(string scoreName)
    {
        if (_score == 0)
            return;

        Highscore newHighscore = new Highscore{ name = scoreName, score = _score};
        SaveManager.Save(newHighscore);
    }

    public void TriggerFailure()
    {
        if (_gameIsRunning)
        {
            OnGameComplete?.Invoke(GameEndType.Fail, 0);

            _gameIsRunning = false;
        }
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

    public void UpdateCollectedArtefacts(int value)
    {
        if (value == 0)
            return;

        OnArtefactsUpdated?.Invoke(value);
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
