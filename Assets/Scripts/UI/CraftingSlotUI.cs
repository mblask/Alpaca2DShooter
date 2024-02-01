using UnityEngine;
using UnityEngine.UI;

public class CraftingSlotUI : MonoBehaviour
{
    private Image _image;
    private Item _item;
    private AlpacaButtonUI _button;

    private void Awake()
    {
        _image = transform.Find("Image").GetComponent<Image>();
        _button = transform.Find("Image").GetComponent<AlpacaButtonUI>();
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
