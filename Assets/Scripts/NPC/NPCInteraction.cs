using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Color _highlightColor = Color.green;
    [SerializeField] private Color _defaultColor = Color.white;
    public string InteractableName { get; private set; } = "NPC";

    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetBossName(string name)
    {
        InteractableName = name;
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
