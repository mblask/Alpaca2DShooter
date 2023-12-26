using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Terminal : Box2dCollider, IInteractable
{
    private SpriteRenderer _spriteRenderer;
    private TerminalCanvas _terminalCanvas;

    public string InteractableName { get; } = "Terminal";

    [SerializeField] private List<Hackable> _hackables = new List<Hackable>();
    [SerializeField] private Portal _exitPortal;
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
    private TerminalUI _terminalUI;
    private TextConsoleUI _textConsoleUI;
    private AchievementManager _achievementManager;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _terminalCanvas = transform.Find("TerminalCanvas").GetComponent<TerminalCanvas>();
    }

    private void Start()
    {
        _playerStats = PlayerStats.Instance;
        _playerInventory = PlayerInventory.Instance;
        _terminalUI = TerminalUI.Instance;
        _textConsoleUI = TextConsoleUI.Instance;
        _achievementManager = AchievementManager.Instance;
        _exitPortalPosition = ExitPortalPosition.Instance;
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
        _terminalUI.ActivateUI(false);
        _textConsoleUI.CloseUI(false);
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
            stopHacking();
            return;
        }

        _stopwatch += _hackingSpeed * Time.deltaTime;
        _terminalCanvas.UpdateSlider(_stopwatch / _hackingTime);
        if (_stopwatch >= _hackingTime)
        {
            _hackingInProgress = false;
            _isHacked = true;
            _achievementManager.CheckOnTerminalHacked();
            openTerminal();
        }
    }

    private void openTerminal()
    {
        _terminalUI.SetTerminal(this);
        _terminalUI.ActivateUI(true);
        _terminalUI.UpdateButtonAvailability(_playerStats.Hacking.GetCurrentValue());
        _terminalUI.InsertedDataItem(_chosenDataItem);
        _terminalUI.AddDataItemsUI(checkForDataItems());
    }

    private List<DataItem> checkForDataItems()
    {
        List<DataItem> dataItems = new List<DataItem>();
        List<Item> items = _playerInventory.GetItems();
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
        _hackables.AddRange
            (transform.parent?.GetComponentsInChildren<Hackable>() ?? new Hackable[0]);
        _hackables.AddRange
            (transform.parent?.parent?.Find("NPCs").GetComponentsInChildren<Hackable>() ?? new Hackable[0]);
        _exitPortal = transform.parent?.GetComponentsInChildren<Portal>()
            .Where(portal => portal.PortalType.Equals(PortalType.Exit)).First();
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

            hackable.Hack();
        }
    }

    public void TurnOnOff()
    {
        foreach (Hackable hackable in _hackables)
        {
            if (hackable == null)
                continue;

            hackable.TurnOnOff();
        }
    }

    public void InsertDataItem(DataItem dataItem)
    {
        if (_chosenDataItem != null)
            return;

        _chosenDataItem = dataItem;
        PlayerInventory.DeleteItemFromInventoryStatic(dataItem);
    }

    public void RemoveDataItem()
    {
        if (_chosenDataItem == null)
            return;

        PlayerInventory.AddToInventoryStatic(_chosenDataItem);
        _chosenDataItem = null;
    }

    public void RunDataCarrier()
    {
        if (_chosenDataItem == null)
            return;

        DataItem dataItem = _chosenDataItem;
        _textConsoleUI.TextToWrite(dataItem.Text);
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
