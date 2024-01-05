using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private Item _item;

    private bool _isPickable = true;

    private AudioManager _audioManager;
    private AchievementManager _achievementManager;
    private ObjectRotation _rotation;
    [SerializeField] private float _rotationSpeed = 2.5f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rotation = new ObjectRotation(transform, _rotationSpeed);
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance;
        _achievementManager = AchievementManager.Instance;

        if (_item != null)
            _spriteRenderer.sprite = _item.ItemSprite;
    }

    private void Update()
    {
        _rotation.Rotate2dBody();
    }

    public void SetRotationSpeed(float speed)
    {
        _rotation.RotationSpeed = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player == null)
            return;

        if (!_isPickable)
            return;

        if (_item is InventoryItem)
        {
            InventoryItem inventoryItem = _item as InventoryItem;
            if (!inventoryItem.StoreItem())
                return;
        }

        if (_item is NonInventoryItem)
        {
            NonInventoryItem nonInventoryItem = _item as NonInventoryItem;
            if (!nonInventoryItem.UseItem())
                return;

            _achievementManager.CheckOnItemUsed(_item);
        }

        ItemTooltip.RemoveTooltipStatic();

        _audioManager.PlayClip(_item.PickupAudio);
        Destroy(gameObject);
    }

    public Item GetItem()
    {
        return _item;
    }

    public void SetItem(Item item)
    {
        _item = item;
        _spriteRenderer.sprite = item.ItemSprite;
    }

    public void SetPickable(bool value)
    {
        _isPickable = value;
    }
}
