using UnityEngine;
using System;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    public static event Action<NPCInteraction> OnInteraction;

    [SerializeField] private Color _highlightColor = Color.green;
    [SerializeField] private Color _defaultColor = Color.white;

    private SpriteRenderer _renderer;
    private ConversationSystem _conversationSystem;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _conversationSystem = GetComponent<ConversationSystem>();
    }

    public void Highlight()
    {
        _renderer.color = _highlightColor;
    }

    public void Interact()
    {
        _conversationSystem.ActivateConversation();
    }

    public void RemoveHighlight()
    {
        _renderer.color = _defaultColor;
    }
}
