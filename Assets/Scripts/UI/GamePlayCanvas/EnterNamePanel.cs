using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnterNamePanel : MonoBehaviour
{
    private const string NAME_ENTERED = "Name Entered!";
    private const string HIGHSCORE_SAVED = "Highscore Saved!";
    private const string SAVE_SCORE = "Save Score";
    private const string NEW_HIGHSCORE = "New Highscore!";

    public static event Action<string> OnNameEntered;

    private TMP_InputField _inputField;
    private TextMeshProUGUI _newScoreText;
    private TextMeshProUGUI _scoreValueText;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputField = transform.Find("InputField").GetComponent<TMP_InputField>();
        _newScoreText = transform.Find("NewScoreText").GetComponent<TextMeshProUGUI>();
        _scoreValueText = transform.Find("ScoreValueText").GetComponent<TextMeshProUGUI>();
    }

    private void Instance_OnActivateNamePanel(int score, bool isNewHighscore)
    {
        _scoreValueText.SetText("Score: " + score.ToString());

        if (isNewHighscore)
            _newScoreText.SetText(NEW_HIGHSCORE);
        else
            _newScoreText.SetText(SAVE_SCORE);

        activate();
    }

    public void GrabInput()
    {
        OnNameEntered?.Invoke(_inputField.text);
        _inputField.interactable = false;
        _inputField.text = HIGHSCORE_SAVED;
    }

    private void activate()
    {
        _animator.SetBool("IsActive", true);
        PlayerController.Instance.DeactivateInput();
    }

    private void deactivate()
    {
        _animator.SetBool("IsActive", false);
    }
}
