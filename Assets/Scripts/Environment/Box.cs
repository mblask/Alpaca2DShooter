using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;
    private ObjectDestroy _objectDestroy;

    private Color _highlightColor = Color.green;
    private Color _defaultColor = Color.white;

    public string InteractableName { get; } = "Box";

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _objectDestroy = GetComponent<ObjectDestroy>();
    }

    public void Highlight()
    {
        _defaultColor = _spriteRenderer.color;
        _spriteRenderer.color = _highlightColor;
    }

    public void Interact()
    {
        _objectDestroy.DamageObject(new DamageData());
    }

    public void RemoveHighlight()
    {
        _spriteRenderer.color = _defaultColor;
    }
}
