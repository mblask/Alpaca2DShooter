using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private Transform _container;
    private Transform _audioUIContainer;
    private GameManager _gameManager;

    private bool _isPaused = false;

    private void Awake()
    {
        _container = transform.Find("PauseMenuContainer");
        _audioUIContainer = transform.Find("AudioUI").Find("Container");
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;

        _gameManager.OnTogglePause += TogglePause;
    }

    private void OnDisable()
    {
        if (_gameManager != null)
            _gameManager.OnTogglePause -= TogglePause;
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
