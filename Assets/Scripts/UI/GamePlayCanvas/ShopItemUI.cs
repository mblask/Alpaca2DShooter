using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    private Image _image;
    private TextMeshProUGUI _priceText;
    //private TextMeshProUGUI _descriptionText;

    private void Awake()
    {
        _image = transform.Find("Image").GetComponent<Image>();
        _priceText = transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
        //_descriptionText = transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
    }

    public void SetupShopItem(Item item, float price, int amount)
    {
        _image.sprite = item.ItemSprite;
        _priceText.SetText("Price: " + price.ToString("F1"));
    }
}
