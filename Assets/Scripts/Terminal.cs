using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;
    private TerminalCanvas _terminalCanvas;

    [SerializeField] private List<Hackable> _hackables = new List<Hackable>();

    private bool _hackingInProgress = false;
    private float _hackingSpeed = 1.0f;
    private float _hackingTime = 3.0f;
    private float _stopwatch = 0.0f;

    private PlayerStats _playerStats;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _terminalCanvas = transform.Find("TerminalCanvas").GetComponent<TerminalCanvas>();
    }

    private void Start()
    {
        _playerStats = PlayerStats.Instance;
    }

    private void Update()
    {
        hackProcedure();
    }

    private void hackProcedure()
    {
        if (!_hackingInProgress)
            return;

        if (!_playerStats.IsAlive())
            return;

        if (checkPlayerTooFar())
        {
            stopHacking();
            return;
        }

        _stopwatch += _hackingSpeed * Time.deltaTime;
        _terminalCanvas.UpdateSlider(_stopwatch / _hackingTime);
        if (_stopwatch >= _hackingTime)
        {
            _hackingInProgress = false;
            UseTerminal();
        }
    }

    private bool checkPlayerTooFar()
    {
        float distance = Vector2.Distance(_playerStats.transform.position, transform.position);
        return distance >= Constants.SHORT_INTERACTION_DISTANCE;
    }

    private void stopHacking()
    {
        _stopwatch = 0.0f;
        _terminalCanvas.UpdateSlider(0.0f);
        _hackingInProgress = false;
        _terminalCanvas.Activate(false);
        FloatingTextSpawner.CreateFloatingTextStatic
            (transform.position, "Too far from\nthe computer", Color.white, 1.0f, 4.0f, 0.5f);
    }

    public void Interact()
    {
        _hackingInProgress = true;
        _hackingSpeed = _playerStats.HackingSpeed.GetFinalValue();
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
        FloatingTextSpawner.CreateFloatingTextStatic
            (transform.position, "Terminal", Color.white, 1.0f, 4.0f, 0.5f);
    }

    public void RemoveHighlight()
    {
        _spriteRenderer.color = Constants.DEFAULT_COLOR;
    }
}
