using UnityEngine;

public class Door : Box2dCollider, IInteractable
{
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    [SerializeField] private Item _keyItem;

    [SerializeField] private Color _defaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private Color _highlightColor = new Color(0.6f, 1.0f, 0.6f, 1.0f);
    public string InteractableName { get; } = "Door";

    [SerializeField] private bool _isLocked = true;
    private bool _isClosed = true;

    [SerializeField] [TextArea] private string _lockedDoorMessage;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void openDoor()
    {
        if (_isLocked)
            return;

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

    private void unlockAndOpenDoor()
    {
        if (!PlayerInventory.GetItemsStatic().Contains(_keyItem) && _keyItem != null)
        {
            string message = _lockedDoorMessage != null ? _lockedDoorMessage : "It's locked!";

            SpeechBox.TriggerSpeechBoxOnInteractionStatic(message);

            return;
        }
        
        PlayerInventory.DeleteItemFromInventoryStatic(_keyItem);
        _isLocked = false;

        openDoor();
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
            unlockAndOpenDoor();
            return;
        }

        if (_isClosed)
            openDoor();
    }

    public void RemoveHighlight()
    {
        _spriteRenderer.color = _defaultColor;
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
