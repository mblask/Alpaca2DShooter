using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    private Image _image;
    private AlpacaButtonUI _button;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _priceText;
    private TextMeshProUGUI _amountText;

    private MarketManager _marketManager;

    [SerializeField]
    private ShopItemData _itemData;

    private void Awake()
    {
        _image = transform.Find("Image").GetComponent<Image>();
        _button = transform.GetComponent<AlpacaButtonUI>();
        _nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        _priceText = transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
        _amountText = transform.Find("AmountText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _marketManager = MarketManager.Instance;

        _button.onLeftClick = () => {
            if (_itemData.Buyable)
                _marketManager.BuyItem(_itemData);
            else
                _marketManager.SellItem(_itemData);
            
            Destroy(this.gameObject);
        };
    }

    public void SetupShopItem(MarketItemData marketItem)
    {
        _itemData = new ShopItemData(marketItem);
        _image.sprite = marketItem.Item.ItemSprite;
        _image.preserveAspect = true;
        _image.color = marketItem.Item.Color;
        _nameText.SetText(marketItem.Item.ItemName);
        _priceText.SetText("Price: " + marketItem.Price.ToString("F1"));
        _amountText.SetText("Amount: " + marketItem.Amount.ToString("F0"));
    }

    public void Buyable(bool value)
    {
        _itemData.Buyable = value;
    }
}