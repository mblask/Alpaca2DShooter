using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    private Item _item;
    private Image _itemImage;
    private AlpacaButtonUI _itemButton;

    private GamePlayCanvas _canvas;

    private void Awake()
    {
        _itemImage = transform.Find("Image").GetComponent<Image>();
        _itemButton = transform.Find("Image").GetComponent<AlpacaButtonUI>();
    }

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;

        _itemButton.onLeftClick = () => {
            if (_item != null)
                UseItemInSlot();
        };

        _itemButton.onCursorEnter = () =>
        {
            if (_item != null)
                _canvas.SetupItemTooltip(_item);
        };

        _itemButton.onCursorExit = () =>
        {
            if (_item != null)
                _canvas.RemoveItemTooltip();
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
                _canvas.UpdateItemContainer();
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
