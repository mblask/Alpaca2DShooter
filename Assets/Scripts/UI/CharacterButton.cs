using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CharacterBaseType _characterType;

    private Button _selectButton;
    private RectTransform _buttonRectTransform;
    private Transform _lockedTextTransform;
    private MainMenuCanvas _mainMenuCanvas;

    private bool _mouseOver = false;
    private float _rotation = -0.8f;

    [Space]
    [SerializeField] private float _scoreToUnlock;
    private bool _locked;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseOver = true;

        if (_locked)
            ScoreInfo.Instance.SetupScoreInfo(_scoreToUnlock);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _mouseOver = false;
        _buttonRectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);

        if (_locked)
            ScoreInfo.Instance.HideContainer();
    }

    private void Awake()
    {
        _selectButton = transform.Find("SelectButton").GetComponent<Button>();
        _buttonRectTransform = transform.Find("SelectButton").GetComponent<RectTransform>();
        _lockedTextTransform = transform.Find("LockedPanel");
    }

    private void Start()
    {
        checkAndSetupButton();

        if (_locked)
            setButtonActive(true);
        else
            setButtonActive(false);
    }

    private void Update()
    {
        if (_mouseOver && !_locked)
            _buttonRectTransform.Rotate(Vector3.forward, _rotation);
    }

    public void SelectAndStartGame()
    {
        PlayerPrefs.SetString(GameAssets.CHARACTER_TYPE_STRING, _characterType.ToString());

        StartCoroutine(MainMenuCanvas.Instance.ActivateLoadingScreen());
    }

    private void setButtonActive(bool value)
    {
        _lockedTextTransform.gameObject.SetActive(value);
        _selectButton.interactable = !value;
    }

    private void checkAndSetupButton()
    {
        _locked = true;

        //no score needed to unlock a character
        if (_scoreToUnlock == 0)
        {
            _locked = false;
            return;
        }

        //score needed to unlock character
        List<Highscore> highscores = SaveManager.Load();

        if (highscores != null)
        {
            foreach (Highscore highscore in highscores)
            {
                if (_scoreToUnlock > highscore.score)
                    continue;
                else
                {
                    _locked = false;
                    return;
                }
            }            
        }
    }
}
