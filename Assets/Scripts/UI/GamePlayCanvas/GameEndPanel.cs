using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameEndType
{
    Success,
    Fail,
}

public class GameEndPanel : MonoBehaviour
{
    private Transform _containerTransform;
    private Transform _gameSuccessTransform;
    private Transform _gameFailTransform;
    private Animator _gameSuccessAnimator;
    private Animator _gameFailAnimator;
    private TextMeshProUGUI _scoreText;

    private void Awake()
    {
        _containerTransform = transform.Find("GameEndContainer");
        _gameSuccessTransform = _containerTransform.Find("GameSuccessPanel");
        _gameFailTransform = _containerTransform.Find("GameFailPanel");
        _scoreText = _gameSuccessTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>();

        _gameSuccessAnimator = _containerTransform.Find("GameSuccessPanel").GetComponent<Animator>();
        _gameFailAnimator = _containerTransform.Find("GameFailPanel").GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameComplete += SetupGameEndPanel;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameComplete -= SetupGameEndPanel;
    }

    public void SetupGameEndPanel(GameEndType type, int score)
    {
        _containerTransform.gameObject.SetActive(true);

        switch (type)
        {
            case GameEndType.Success:
                _gameFailTransform.gameObject.SetActive(false);
                _scoreText.SetText("Score: " + score.ToString());
                _gameSuccessAnimator.SetTrigger("GameEndTrigger");
                break;

            case GameEndType.Fail:
                _gameSuccessTransform.gameObject.SetActive(false);
                _gameFailAnimator.SetTrigger("GameEndTrigger");
                break;
        }
    }
}
