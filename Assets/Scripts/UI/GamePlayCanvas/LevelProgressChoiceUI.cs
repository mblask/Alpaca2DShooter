using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressChoiceUI : MonoBehaviour
{
    private static LevelProgressChoiceUI _instance;

    public static LevelProgressChoiceUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = new LevelProgressChoiceUI();

            return _instance;
        }
    }

    private Transform _container;
    private Button _continueButton;
    private Button _repeatButton;

    private LevelsManager _levelsManager;

    private void Awake()
    {
        _instance = this;

        _container = transform.Find("Container");
        _continueButton = transform.Find("Container").Find("ContinueButton").GetComponent<Button>();
        _repeatButton = transform.Find("Container").Find("RepeatButton").GetComponent<Button>();
    }

    private void Start()
    {
        _levelsManager = LevelsManager.Instance;

        _continueButton.onClick.AddListener(continueButton);
        _repeatButton.onClick.AddListener(repeatButton);
    }

    private void activateUI(bool value)
    {
        _container.gameObject.SetActive(value);
    }

    private void repeatButton()
    {
        Debug.Log("Repeat button");
    }

    private void continueButton()
    {
        Debug.Log("Continue button");
    }
}
