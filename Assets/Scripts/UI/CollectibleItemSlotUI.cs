using UnityEngine;
using UnityEngine.UI;

public class CollectibleItemSlotUI : MonoBehaviour
{
    private Image _itemImage;
    private AlpacaButtonUI _button;
    
    private Item _item;

    private ItemTooltip _tooltip;

    private void Awake()
    {
        _itemImage = transform.Find("Image").GetComponent<Image>();
        _button = _itemImage.GetComponent<AlpacaButtonUI>();
    }

    private void OnDisable()
    {
        _tooltip.RemoveTooltip();
    }

    private void Start()
    {
        _tooltip = ItemTooltip.Instance;

        _button.onCursorEnter = () => { 
            if (_item != null)
                _tooltip.SetupTooltip(_item); 
        };

        _button.onCursorExit = () => {
            if (_item != null)
                _tooltip.RemoveTooltip();
        };
    }

    public void AddItemToSlot(Item item)
    {
        _item = item;
        _itemImage.sprite = item.ItemSprite;
        _itemImage.enabled = true;
    }

    public void RemoveItem()
    {
        _itemImage.sprite = null;
        _itemImage.enabled = false;
    }
}
