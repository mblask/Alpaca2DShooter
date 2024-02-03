using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Color _highlightColor = Color.green;
    [SerializeField] private Color _defaultColor = Color.white;
    public string InteractableName { get; } = "NPC";

    private SpriteRenderer _renderer;

    private NpcAi _npcAi;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _npcAi = GetComponent<NpcAi>();
    }

    public void Highlight()
    {
        _renderer.color = _highlightColor;
    }

    public void Interact()
    {
        
    }

    public void RemoveHighlight()
    {
        _renderer.color = _defaultColor;
    }
}
