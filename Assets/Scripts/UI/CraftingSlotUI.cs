using UnityEngine;
using UnityEngine.UI;

public class CraftingSlotUI : MonoBehaviour
{
    private Image _image;
    private Item _item;
    private AlpacaButtonUI _button;
    private GamePlayCanvas _canvas;

    private void Awake()
    {
        _image = transform.Find("Image").GetComponent<Image>();
        _button = transform.Find("Image").GetComponent<AlpacaButtonUI>();
    }

    private void Start()
    {
        _canvas = GamePlayCanvas.Instance;
        _button.onCursorEnter = () =>
        {
            if (_item != null)
            {
                _canvas.SetupItemTooltip(_item);
            }
        };

        _button.onCursorExit = () =>
        {
            if (_item != null)
            {
                _canvas.RemoveItemTooltip();
            }
        };
    }

    public void AddItemToSlot(Item item)
    {
        _item = item;
        _image.preserveAspect = true;
        _image.sprite = item.ItemSprite;
        _image.enabled = true;
        _image.preserveAspect = true;
    }

    public void RemoveItemFromSlot()
    {
        _item = null;
        _image.sprite = null;
        _image.enabled = false;
    }
}
