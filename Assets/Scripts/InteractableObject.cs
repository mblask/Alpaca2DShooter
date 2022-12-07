using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    private SpriteRenderer _renderer;

    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _interactColor = Color.yellow;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        Debug.Log("Interacting and destroying interactable");
        Destroy(gameObject);
    }

    public void Highlight()
    {
        _renderer.color = _interactColor;
    }

    public void RemoveHighlight()
    {
        _renderer.color = _defaultColor;
    }
}
