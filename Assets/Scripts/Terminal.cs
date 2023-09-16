using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private List<Hackable> _hackables = new List<Hackable>();

    private bool _hackingInProgress = false;
    private float _hackingSpeed = 1.0f;
    private float _hackingTime = 3.0f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!_hackingInProgress)
            return;

        _hackingTime -= _hackingSpeed * Time.deltaTime;
        if (_hackingTime <= 0.0f)
        {
            _hackingInProgress = false;
            UseTerminal();
        }
    }

    public void Interact()
    {
        _hackingInProgress = true;
        _hackables.AddRange(transform.parent.GetComponentsInChildren<Hackable>());
        _hackables.AddRange(transform.parent.parent.Find("NPCs").GetComponentsInChildren<Hackable>());
    }

    public void UseTerminal()
    {
        foreach (Hackable hackable in _hackables)
        {
            if (hackable == null)
                continue;

            hackable.Hack();
        }
    }

    public void Highlight()
    {
        _spriteRenderer.color = Constants.HIGHLIGHT_COLOR;
        FloatingTextSpawner.CreateFloatingTextStatic(transform.position, "Terminal", Color.white, 1.0f, 4.0f, 0.5f);
    }

    public void RemoveHighlight()
    {
        _spriteRenderer.color = Constants.DEFAULT_COLOR;
    }
}

public abstract class Hackable : MonoBehaviour
{
    public abstract void Hack();
}
