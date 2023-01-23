using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum MainCanvasScreen
{
    Main,
    Highscore,
    Character,
    Loading,
}

public class MainMenuCanvas : Singleton<MainMenuCanvas>
{
    public List<Vector2> Resolutions;

    [SerializeField] private Transform HighscoreUIPrefab;

    private Transform _mainMenuTransform;
    private Transform _characterMenuTransform;
    private Transform _highscoresTransform;
    private Transform _howToPlaTransform;
    private Transform _creditsTransform;
    private Transform _loadingScreenTransform;

    private Slider _loadingSlider;

    private Transform _activeScreen;
    private AudioSource _audioSource;

    private CanvasScaler _canvasScaler;
    private int _currentResolutionIndex = 0;

    public override void Awake()
    {
        base.Awake();

        _audioSource = Camera.main.GetComponent<AudioSource>();
        _mainMenuTransform = transform.Find("MainMenu");
        _characterMenuTransform = transform.Find("CharacterMenu");
        _highscoresTransform = transform.Find("Highscores");
        _howToPlaTransform = transform.Find("HowToPlay");
        _creditsTransform = transform.Find("Credits");
        _loadingScreenTransform = transform.Find("LoadingScreen");

        _loadingSlider = _loadingScreenTransform.Find("LoadingBar").Find("Slider").GetComponent<Slider>();

        _canvasScaler = GetComponent<CanvasScaler>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _activeScreen = getActiveScreen();
            _activeScreen.gameObject.SetActive(false);
            
            _mainMenuTransform.gameObject.SetActive(true);

            _audioSource.Play();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            _currentResolutionIndex++;

            if (_currentResolutionIndex >= Resolutions.Count)
                _currentResolutionIndex = 0;

            _canvasScaler.referenceResolution = Resolutions[_currentResolutionIndex];
        }
    }

    public IEnumerator ActivateLoadingScreen()
    {
        _loadingScreenTransform.gameObject.SetActive(true);

        _loadingSlider.value = 0.0f;

        while (_loadingSlider.value < _loadingSlider.maxValue)
        {
            _loadingSlider.value += Time.deltaTime * 2.5f;

            yield return null;
        }

        SceneManager.LoadScene(1);
    }

    public void StartGame()
    {
        _mainMenuTransform.gameObject.SetActive(false);

        StartCoroutine("ActivateLoadingScreen");
    }

    private Transform getActiveScreen()
    {
        if (_mainMenuTransform.gameObject.activeSelf)
            return _mainMenuTransform;

        if (_characterMenuTransform.gameObject.activeSelf)
            return _characterMenuTransform;

        if (_highscoresTransform.gameObject.activeSelf)
            return _highscoresTransform;

        if (_howToPlaTransform.gameObject.activeSelf)
            return _howToPlaTransform;

        if (_creditsTransform.gameObject.activeSelf)
            return _creditsTransform;

        if (_loadingScreenTransform.gameObject.activeSelf)
            return _loadingScreenTransform;

        return null;
    }

    public void PopulateHighscores()
    {
        List<Highscore> highscoreList = SaveManager.Load();

        Transform highscoresContainer = _highscoresTransform.Find("Container").Find("HighscoresContainer");
        for (int i = 0; i < highscoresContainer.childCount; i++)
        {
            Destroy(highscoresContainer.GetChild(i).gameObject);
        }

        if (highscoreList == null)
            return;

        foreach (Highscore highscore in highscoreList)
        {
            Transform highscoreUI = Instantiate(HighscoreUIPrefab, highscoresContainer);
            highscoreUI.GetComponent<TextMeshProUGUI>().SetText(highscore.name + ": " + highscore.score);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}