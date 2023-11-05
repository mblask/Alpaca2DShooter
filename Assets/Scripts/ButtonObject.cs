using System;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObject : MonoBehaviour, IButton
{
    private SpriteRenderer _spriteRenderer;

    private bool _isPressed = false;
    private Color _pressedColor = Color.green;
    private Color _releasedColor = Color.red;

    public string InteractableName { get; } = "Button";

    private Action _onButtonPresssed;
    private Action _onButtonReleased;

    private List<SwitchableObject> _switchables = new List<SwitchableObject>();

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = _releasedColor;
    }

    public void Highlight()
    {

    }

    public void Interact()
    {
        _isPressed = !_isPressed;
        _spriteRenderer.color = _isPressed ? _pressedColor : _releasedColor;

        if (_isPressed )
        {
            _onButtonPresssed?.Invoke();
        }
        else
        {
            _onButtonReleased?.Invoke();
        }

        foreach (SwitchableObject switchable in _switchables)
        {
            switchable.Toggle();
        }
    }

    public void RemoveHighlight()
    {
        
    }

    public void SetOnPressed(Action onButtonPressed)
    {
        _onButtonPresssed = onButtonPressed;
    }

    public void SetOnReleased(Action onButtonReleased)
    {
        _onButtonReleased = onButtonReleased;
    }
}
