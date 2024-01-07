using AlpacaMyGames;
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
        _instance = this;
    }

    private void Start()
    {
        _mouseCursorTransform = MouseCursor.Instance.transform;
        _achievementsManager = AchievementManager.Instance;
        _gameAssets = GameAssets.Instance;
        _uiCanvas = GamePlayCanvas.Instance;
    }

    private void Update()
    {
        if (_gameIsRunning)
        {
            timeManager();
            checkMouseCursor();

            if (Input.GetKeyDown(KeyCode.Escape))
                _uiCanvas.TogglePauseUI();
        }
    }

    private void checkMouseCursor()
    {
        if (_mouseCursorTransform != null)
            return;
        
        _mouseCursorTransform = Instantiate(_gameAssets.MouseCursorObject, Utilities.GetMouseWorldLocation(), Quaternion.identity, transform.parent);
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
