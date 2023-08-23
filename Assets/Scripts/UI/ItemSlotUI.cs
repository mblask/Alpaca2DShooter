using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    private Item _item;
    private Image _itemImage;
    private AlpacaButtonUI _itemButton;
    private InventoryUI _inventoryUI;

    private void Awake()
    {
        _inventoryUI = GetComponentInParent<InventoryUI>();
        _itemImage = transform.Find("Image").GetComponent<Image>();
        _itemButton = transform.Find("Image").GetComponent<AlpacaButtonUI>();
    }

    private void Start()
    {
        _itemButton.onLeftClick = () => {
            if (_item != null)
                UseItemInSlot();
        };

        _itemButton.onRightClick = () =>
        {
            if (_item != null && PlayerInventory.ThrowFromInventoryStatic(_item))
                removeItemFromSlot();
        };
    }

    public void UseItemInSlot()
    {
        if (_item != null)
        {
            if (_item.UseItem())
                _inventoryUI.UpdateItemContainer();
        }
    }

    public void AddItemToSlot(Item item)
    {
        _item = item;
        _itemImage.sprite = item.ItemSprite;
        _itemImage.enabled = true;
    }

    private void removeItemFromSlot()
    {
        _item = null;
        _itemImage.sprite = null;
        _itemImage.enabled = false;
    }
}
