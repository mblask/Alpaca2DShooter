using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

public class Door : Box2dCollider, IInteractable
{
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    [SerializeField] private Color _defaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private Color _highlightColor = new Color(0.6f, 1.0f, 0.6f, 1.0f);
    public string InteractableName { get; } = "Door";

    private bool _isLocked = false;
    private bool _isLockpicking = false;
    private FloatingTextSingle _floatingText;
    private bool _isClosed = true;
    private TimerObject _lockpickingTimer;
    
    private int _hitPoints = 0;

    private PlayerInventory _playerInventory;
    private DoorCanvas _doorCanvas;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _doorCanvas = transform.Find("DoorCanvas").GetComponent<DoorCanvas>();
    }

    private void Start()
    {
        _playerInventory = PlayerInventory.Instance;
        
    }

    private void Update()
    {
        lockpickingProcedure();
    }

    private void lockpickingProcedure()
    {
        if (!_isLockpicking)
            return;

        if (!_isLocked)
            return;

        if (!_lockpickingTimer.Update())
        {
            if (checkPlayerTooFar())
            {
                breakLockpicking();
                return;
            }

            _doorCanvas.UpdateSlider(_lockpickingTimer.Timer);
            return;
        }

        _doorCanvas.Activate(false);
        _isLockpicking = false;
        _isLocked = false;
        openDoor();
    }

    private bool checkPlayerTooFar()
    {
        float distance = Vector2.Distance(_playerInventory.transform.position, transform.position);
        return distance >= Constants.SHORT_INTERACTION_DISTANCE;
    }

    private void openDoor()
    {
        _isClosed = false;

        float alfa = 0.3f;

        Color color = _spriteRenderer.color;
        color.a = alfa;
        _spriteRenderer.color = color;

        Color newDefaultColor = _defaultColor;
        newDefaultColor.a = alfa;
        _defaultColor = newDefaultColor;

        _boxCollider.isTrigger = true;
    }

    public void StartLockpicking()
    {
        _isLockpicking = true;

        float lockpickingDuration = 1.0f;
        _lockpickingTimer = new TimerObject(lockpickingDuration);
        _doorCanvas.Activate(true);
    }

    private bool checkForLockpick()
    {
        bool lockpickAvailable = false;
        List<Item> items = _playerInventory.GetItems();
        foreach (Item item in items)
        {
            InventoryItem inventoryItem = item as InventoryItem;
            if (inventoryItem == null)
                continue;

            if (!inventoryItem.ItemName.Equals("Lockpick"))
                continue;

            lockpickAvailable = true;
            break;
        }

        return lockpickAvailable;
    }

    private void breakLockpicking()
    {
        _isLocked = true;
        _isLockpicking = false;
        _lockpickingTimer.Reset();
        _doorCanvas.Activate(false);
    }

    public void Highlight()
    {
        _defaultColor = _spriteRenderer.color;
        _spriteRenderer.color = _highlightColor;
    }

    public void Interact()
    {
        if (_isLocked)
        {
            if (_isLockpicking)
            {
                _floatingText = FloatingTextSpawner
                .CreateFloatingTextStatic(transform.position, "Still picking the lock!", Color.yellow, 1.0f, 8.0f, 1.5f);
                return;
            }

            if (checkForLockpick())
            {
                StartLockpicking();
                return;
            }

            if (_floatingText != null)
                return;

            _floatingText = FloatingTextSpawner
                .CreateFloatingTextStatic(transform.position, "Door locked", Color.yellow, 1.0f, 8.0f, 1.5f);
            return;
        }

        if (_isClosed)
            openDoor();
    }

    public void RemoveHighlight()
    {
        _spriteRenderer.color = _defaultColor;
    }

    public void DamageDoor()
    {
        if (!_isLocked)
            return;

        _hitPoints--;

        if (_hitPoints <= 0)
        {
            FloatingTextSpawner.CreateFloatingTextStatic
                (transform.position, "Lock broken", Color.green, 1.5f, 8.0f, 1.5f);
            _isLocked = false;
        }
    }

    public void LockDoor(bool value)
    {
        _isLocked = value;

        Vector2Int hitPointInterval = new Vector2Int(3, 6);
        if (_isLocked)
            _hitPoints = hitPointInterval.GetRandom();
    }

    public bool IsLocked()
    {
        return _isLocked;
    }

    public bool IsClosed()
    {
        return _isClosed;
    }
}
