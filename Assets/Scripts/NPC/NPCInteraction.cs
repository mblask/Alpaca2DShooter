using UnityEngine;
using System;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    public static event Action<NPCInteraction> OnInteraction;

    [SerializeField] private Color _highlightColor = Color.green;
    [SerializeField] private Color _defaultColor = Color.white;

    private SpriteRenderer _renderer;
    private NPC_AI _enemyAI;
    private PlayerBase _playerBase;
    private ConversationSystem _conversationSystem;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _enemyAI = GetComponent<NPC_AI>();
        _conversationSystem = GetComponent<ConversationSystem>();
    }

    private void Start()
    {
        _playerBase = PlayerBase.Instance;
    }

    public void Highlight()
    {
        _renderer.color = _highlightColor;
    }

    public void Interact()
    {
        if (!_enemyAI.OnInteractWithPlayer(_playerBase.transform))
            return;

        _conversationSystem.ActivateConversation();
    }

    public void RemoveHighlight()
    {
        _renderer.color = _defaultColor;
    }
}
