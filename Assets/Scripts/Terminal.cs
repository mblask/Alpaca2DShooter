using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Terminal : MonoBehaviour, IInteractable
{
    private SpriteRenderer _spriteRenderer;
    private TerminalCanvas _terminalCanvas;

    public string InteractableName { get; } = "Terminal";

    [SerializeField]
    private List<Hackable> _hackables = new List<Hackable>();
    [SerializeField]
    private Portal _exitPortal;
    private ExitPortalPosition _exitPortalPosition;

    private bool _isHacked = false;
    private bool _hackingInProgress = false;
    private float _hackingSpeed = 1.0f;
    private float _hackingTime = 3.0f;
    private float _stopwatch = 0.0f;

    private bool _isInteracting = false;

    private DataItem _chosenDataItem;

    private PlayerStats _playerStats;
    private PlayerInventory _playerInventory;
    private AchievementManager _achievementManager;
    private GamePlayCanvas _uiCanvas;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _terminalCanvas = transform.Find("TerminalCanvas").GetComponent<TerminalCanvas>();
    }

    private void Start()
    {
        _playerStats = PlayerStats.Instance;
        _playerInventory = PlayerInventory.Instance;
        _achievementManager = AchievementManager.Instance;
        _exitPortalPosition = ExitPortalPosition.Instance;
        _uiCanvas = GamePlayCanvas.Instance;
    }

    private void Update()
    {
        breakingInProcedure();
        isInInteractionRange();
    }

    private void isInInteractionRange()
    {
        if (!_isInteracting)
            return;

        if (!checkPlayerTooFar())
            return;

        _isInteracting = false;
        _uiCanvas.ActivateTerminalUI(false);
        _uiCanvas.CloseTextConsoleUI(false);
    }

    private void breakingInProcedure()
    {
        if (_isHacked)
            return;

        if (!_hackingInProgress)
            return;

        if (!_playerStats.IsAlive())
            return;

        if (checkPlayerTooFar())
        {
            AudioManager.Instance.StopPlaying();
            stopHacking();
            return;
        }

        _stopwatch += _hackingSpeed * Time.deltaTime;
        _terminalCanvas.UpdateSlider(_stopwatch / _hackingTime);
        if (_stopwatch >= _hackingTime)
        {
            AudioManager.Instance.StopPlaying();
            _hackingInProgress = false;
            _isHacked = true;
            _achievementManager.CheckOnTerminalHacked();
            openTerminal();
        }
    }

    private void openTerminal()
    {
        _uiCanvas.SetTerminal(this);
        _uiCanvas.ActivateTerminalUI(true);
        _uiCanvas.UpdateButtonAvailability(_playerStats.Hacking.GetCurrentValue());
        _uiCanvas.InsertedDataItem(_chosenDataItem);
        _uiCanvas.AddDataItemsUI(checkForDataItems());
    }

    private List<DataItem> checkForDataItems()
    {
        List<DataItem> dataItems = new List<DataItem>();
        List<Item> items = _playerInventory.GetCollectibles();
        foreach (Item item in items)
        {
            DataItem dataItem = item as DataItem;
            if (dataItem != null)
                dataItems.Add(dataItem);
        }

        return dataItems;
    }

    private bool checkPlayerTooFar()
    {
        if (_playerStats == null)
            return true;

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
            (_playerStats.transform.position, "Too far from\nthe computer", Color.white, 1.0f, 8.0f, 0.5f);
    }

    public void Interact()
    {
        _isInteracting = true;

        isNotHacked();
        isHacked();
    }

    private void isHacked()
    {
        if (!_isHacked)
            return;
        
        openTerminal();
    }

    private void isNotHacked()
    {
        if (_isHacked)
            return;

        _hackingInProgress = true;
        _hackingSpeed = _playerStats.Hacking.GetFinalValue();
        _hackables.AddRange(transform.parent?.GetComponentsInChildren<Hackable>() ?? new Hackable[0]);
        _hackables.AddRange(transform.parent?.parent?.Find("NPCs").GetComponentsInChildren<Hackable>() ?? new Hackable[0]);
        _exitPortal = transform.parent?.GetComponentsInChildren<Portal>()
            .Where(portal => portal.PortalType.Equals(PortalType.Exit)).First();
        AudioManager.Instance.PlayClip(SFXClip.KeyboardTyping);
    }

    public void ShowExitPortal()
    {
        _exitPortalPosition.SetExitPortalPosition(_exitPortal.transform);
    }

    public void Hack()
    {
        foreach (Hackable hackable in _hackables)
        {
            if (hackable == null)
                continue;

            hackable.SwitchAllegiance();
        }
    }

    public void TurnOnOff()
    {
        foreach (Hackable hackable in _hackables)
        {
            if (hackable == null)
                continue;

            hackable.Toggle();
        }
    }

    public void InsertDataItem(DataItem dataItem)
    {
        if (_chosenDataItem != null)
            return;

        _chosenDataItem = dataItem;
        _playerInventory.RemoveCollectible(dataItem);
    }

    public bool RemoveDataItem()
    {
        if (_chosenDataItem == null)
            return false;

        if (!PlayerInventory.AddToInventoryStatic(_chosenDataItem))
            return false;

        _uiCanvas.CloseTextConsoleUI(false);
        _uiCanvas.AddDataItemsUI(checkForDataItems());
        _chosenDataItem = null;
        return true;
    }

    public void RunDataCarrier()
    {
        if (_chosenDataItem == null)
            return;

        _uiCanvas.ActivateTextConsoleUI(_chosenDataItem.Text);
    }

    public void Highlight()
    {
        _spriteRenderer.color = Constants.HIGHLIGHT_COLOR;
    }

    public void RemoveHighlight()
    {
        _spriteRenderer.color = Constants.DEFAULT_COLOR;
    }
}
