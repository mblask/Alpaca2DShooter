using AlpacaMyGames;
using System;
using UnityEngine;
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

    private Guid _gameId = Guid.Empty;

    [Header("Read-only")]
    [SerializeField] private float _gameTime;
    private float _incrementedTime;
    private float _timeIncrement = 0.1f;

    [SerializeField] private int _enemiesKilled = 0;

    private bool _gameIsRunning = true;
    private bool _isPaused = false;

    private GameAssets _gameAssets;
    private AchievementManager _achievementsManager;
    private Transform _mouseCursorTransform;
    private GamePlayCanvas _uiCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (_instance != null)
            Destroy(_instance.gameObject);

        _instance = this;
        initializeGameId();
    }

    private void Update()
    {
        if (!_gameIsRunning)
            return;

        timeManager();
        checkMouseCursor();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_uiCanvas == null)
                _uiCanvas = GamePlayCanvas.Instance;

            _uiCanvas.TogglePauseUI();
        }
    }

    private void initializeGameId()
    {
        _gameId = SaveManager.LoadGameId();

        if (_gameId != Guid.Empty)
            return;
        
        _gameId = Guid.NewGuid();
        SaveManager.SaveGameId(_gameId);
    }

    private void checkMouseCursor()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;

        if (_mouseCursorTransform != null)
            return;

        if (_gameAssets == null)
            _gameAssets = GameAssets.Instance;

        _mouseCursorTransform = MouseCursor.Instance.transform;

        if (_mouseCursorTransform == null)
        {
            _mouseCursorTransform = Instantiate
                (_gameAssets.MouseCursorObject, Utilities.GetMouseWorldLocation(), Quaternion.identity, null);
        }
    }

    public float GetGameTime()
    {
        return _gameTime;
    }

    public int GetEnemiesKilled()
    {
        return _enemiesKilled;
    }

    public void IncrementEnemiesKilled()
    {
        _enemiesKilled++;

        if (_achievementsManager == null)
            _achievementsManager = AchievementManager.Instance;

        _achievementsManager.CheckOnNpcKilled(_enemiesKilled);
    }

    private void timeManager()
    {
        _gameTime += Time.deltaTime;

        _incrementedTime += Time.deltaTime;

        if (_incrementedTime >= _timeIncrement)
            _incrementedTime -= _timeIncrement;
    }

    public void TriggerVictory()
    {
        Debug.Log("Game finished!");
        _gameIsRunning = false;

        if (_achievementsManager == null)
            _achievementsManager = AchievementManager.Instance;
        _achievementsManager.CheckOnGameFinished();
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
