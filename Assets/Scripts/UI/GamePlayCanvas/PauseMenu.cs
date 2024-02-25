using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private Transform _container;
    private Transform _audioUIContainer;
    private GameManager _gameManager;

    private bool _isPaused = false;
    public bool IsPaused => _isPaused;

    private void Awake()
    {
        _container = transform.Find("PauseMenuContainer");
        _audioUIContainer = transform.Find("AudioUI").Find("Container");
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }


    public void TogglePause()
    {
        if (_audioUIContainer.gameObject.activeSelf)
        {
            _audioUIContainer.gameObject.SetActive(false);
            _container.gameObject.SetActive(true);
        }
        else
        {
            _isPaused = !_isPaused;
            _gameManager.SetPaused(_isPaused);
            _container.gameObject.SetActive(_isPaused);
            Time.timeScale = _isPaused ? 0.0f : 1.0f;
        }
    }
}
