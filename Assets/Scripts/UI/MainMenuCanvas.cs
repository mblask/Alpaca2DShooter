using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuCanvas : MonoBehaviour
{
    private static MainMenuCanvas _instance;
    public static MainMenuCanvas Instance
    {
        get
        {
            return _instance;
        }
    }

    public List<Vector2> Resolutions;

    [SerializeField] private Transform HighscoreUIPrefab;

    private Transform _mainMenuTransform;
    private Transform _characterMenuTransform;
    private Transform _highscoresTransform;
    private Transform _controlsTransform;
    private Transform _creditsTransform;
    private Transform _loadingScreenTransform;
    private Transform _scoreInfoTransform;

    private Slider _loadingSlider;

    private Transform _activeScreen;
    private AudioSource _audioSource;

    private bool _sceneLoading = false;
    private float _loadingSliderFactor = 2.5f;

    public void Awake()
    {
        _instance = this;

        _audioSource = Camera.main.GetComponent<AudioSource>();
        _mainMenuTransform = transform.Find("MainMenu");
        _characterMenuTransform = transform.Find("CharacterMenu").Find("Container");
        _highscoresTransform = transform.Find("Highscores").Find("Container");
        _controlsTransform = transform.Find("Controls").Find("Container");
        _creditsTransform = transform.Find("Credits").Find("Container");
        _loadingScreenTransform = transform.Find("LoadingScreen").Find("Container");
        _scoreInfoTransform = transform.Find("ScoreInfo").Find("Container");

        _loadingSlider = _loadingScreenTransform.Find("LoadingBar").Find("Slider").GetComponent<Slider>();
    }

    private void Update()
    {
        loadingScreenProcedure();
    }

    public void CloseActiveUi()
    {
        _activeScreen = getActiveScreen();
        _activeScreen.gameObject.SetActive(false);
        _scoreInfoTransform.gameObject.SetActive(false);
        
        _mainMenuTransform.gameObject.SetActive(true);
        
        _audioSource.Play();
    }

    public void ActivateLoading()
    {
        _loadingScreenTransform.gameObject.SetActive(true);
        _loadingSlider.value = 0.0f;
        _sceneLoading = true;
    }

    private void loadingScreenProcedure()
    {
        if (!_sceneLoading)
            return;

        _loadingSlider.value += Time.deltaTime * _loadingSliderFactor;

        if (_loadingSlider.value < _loadingSlider.maxValue)
            return;
        
        _loadingSlider.value = 0.0f;
        SceneManager.LoadScene(1);
    }

    public void StartGame()
    {
        _mainMenuTransform.gameObject.SetActive(false);

        _loadingScreenTransform.gameObject.SetActive(true);
        _loadingSlider.value = 0.0f;
        _sceneLoading = true;
    }

    private Transform getActiveScreen()
    {
        if (_mainMenuTransform.gameObject.activeSelf)
            return _mainMenuTransform;

        if (_characterMenuTransform.gameObject.activeSelf)
            return _characterMenuTransform;

        if (_highscoresTransform.gameObject.activeSelf)
            return _highscoresTransform;

        if (_controlsTransform.gameObject.activeSelf)
            return _controlsTransform;

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
