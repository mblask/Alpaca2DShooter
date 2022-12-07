using System;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public static event Action<Item> OnMouseEnter;
    public static event Action OnMouseExit;
    public static event Action<SFXClip> OnItemPickedUpAudio;

    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private Item _item;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (_item != null)
            _spriteRenderer.sprite = _item.ItemSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            if (_item is InventoryItem)
            {
                InventoryItem inventoryItem = _item as InventoryItem;
                if (!inventoryItem.StoreItem())
                    return;
            }

            if (_item is NonInventoryItem)
            {
                NonInventoryItem nonInventoryItem = _item as NonInventoryItem;
                nonInventoryItem.UseItem();
            }

            ItemTooltip.RemoveTooltipStatic();

            OnItemPickedUpAudio?.Invoke(_item.PickupAudio);
            Destroy(gameObject);
        }
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
}
