using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCMenu : MonoBehaviour
{
    [SerializeField] private Transform _buttonPrefab;

    private Camera _mainCamera;
    private RectTransform _rectTransform;
    private Transform _container;
    private Transform _menuContainer;
    private Button _talkButton;
    private Button _exitButton;
    private Button _backButton;

    private Transform _targetToFollow = null;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _container = transform.Find("Container");
        _menuContainer = _container.Find("MenuContainer");
        _talkButton = _menuContainer.Find("TalkButton").GetComponent<Button>();
        _exitButton = _menuContainer.Find("ExitButton").GetComponent<Button>();
        _backButton = _menuContainer.Find("BackButton").GetComponent<Button>();
    }

    private void Start()
    {
        NPCInteraction.OnInteraction += InteractableCharacter_OnInteraction;
        
        _mainCamera = Camera.main;
        _exitButton.onClick.AddListener(deactivateMenu);
    }

    private void LateUpdate()
    {
        followTarget();
    }

    private void followTarget()
    {
        if (_targetToFollow != null)
        {
            _rectTransform.position = _mainCamera.WorldToScreenPoint(_targetToFollow.position);
        }
    }

    private void InteractableCharacter_OnInteraction(NPCInteraction interactableCharacter)
    {
        _targetToFollow = interactableCharacter.transform;

        _talkButton.onClick.RemoveAllListeners();
        _talkButton.onClick.AddListener(() => {
            ConversationSystem conversationSystem = interactableCharacter.GetComponent<ConversationSystem>();
            List <Conversation> conversationList = conversationSystem.GetConversationList();
            for (int i = 0; i < conversationList.Count; i++)
            {
                Transform newConversationButtonTransform = Instantiate(_buttonPrefab, _menuContainer);

                TextMeshProUGUI newConversationButtonText = newConversationButtonTransform.Find("Text").GetComponent<TextMeshProUGUI>();
                newConversationButtonText.SetText(conversationList[i].Topic);

                Button newConversationButton = newConversationButtonTransform.GetComponent<Button>();
                Conversation conversationToActivate = conversationList[i];
                newConversationButton.onClick.AddListener(() => {
                    if (!SpeechBox.IsWriting())
                    {
                        //start writing current speech
                        SpeechBox.ActivateSpeechBoxSingleStatic(conversationToActivate.Speech.SpeechList[0]);
                    }
                    else
                    {
                        //interupt current speech and write it completely
                        SpeechBox.WriteCurrentMessageCompletelyStatic();
                    }

                    return;
                });
            }

            _talkButton.gameObject.SetActive(false);
            _exitButton.gameObject.SetActive(false);
            _backButton.gameObject.SetActive(true);
        });

        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() => {
            foreach (Transform item in _menuContainer)
            {
                if (!item.name.Equals(_talkButton.transform.name) && !item.name.Equals(_exitButton.transform.name) && !item.name.Equals(_backButton.transform.name))
                    Destroy(item.gameObject);
            }

            SpeechBox.DeactivateSpeechBoxSingleStatic();

            _talkButton.gameObject.SetActive(true);
            _exitButton.gameObject.SetActive(true);
            _backButton.gameObject.SetActive(false);
        });

        activateMenu();
    }

    private void activateMenu()
    {
        _container.gameObject.SetActive(true);
    }

    private void deactivateMenu()
    {
        _container.gameObject.SetActive(false);
    }
}
