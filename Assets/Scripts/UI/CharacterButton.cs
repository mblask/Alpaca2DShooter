using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CharacterBaseScriptable _character;

    private Button _selectButton;
    private RectTransform _buttonRectTransform;
    private Transform _lockedTextTransform;

    private bool _mouseOver = false;
    private float _rotation = -120.0f;

    private PlayerSelector _playerSelector;

    private bool _locked;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _mouseOver = true;

        if (_locked)
            ScoreInfo.Instance.SetupScoreInfo(_character.ScoreToUnlock);
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
        _playerSelector = PlayerSelector.Instance;

        checkAndSetupButton();
        setButtonActive(_locked);
    }

    private void Update()
    {
        if (_mouseOver && !_locked)
            _buttonRectTransform.Rotate(Vector3.forward, _rotation * Time.deltaTime);
    }

    public void SelectAndStartGame()
    {
        _playerSelector.SelectPlayerBase(_character.CharacterType);
        MainMenuCanvas.Instance.ActivateLoading();
        //StartCoroutine(MainMenuCanvas.Instance.ActivateLoadingScreen());
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
        if (_character.ScoreToUnlock == 0)
        {
            _locked = false;
            return;
        }

        //score needed to unlock character
        List<Highscore> highscores = SaveManager.Load();

        if (highscores == null || highscores.Count == 0)
            return;

        foreach (Highscore highscore in highscores)
        {
            if (_character.ScoreToUnlock < highscore.score)
            {
                _locked = false;
                return;
            }
        }
    }
}
