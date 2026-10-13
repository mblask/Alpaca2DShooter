using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    private AlpacaButtonUI _buyButton;
    private AlpacaButtonUI _sellButton;
    private Transform _shopItemsContainer;

    private GameAssets _gameAssets;

    private PlayerInventory _inventory;
    private MarketManager _market;

    private void OnEnable()
    {
        _gameAssets = GameAssets.Instance;
        _market = MarketManager.Instance;

        _buyButton = transform.Find("ShopBuyButton").GetComponent<AlpacaButtonUI>();
        _sellButton = transform.Find("ShopSellButton").GetComponent<AlpacaButtonUI>();
        _shopItemsContainer = transform.Find("ScrollView").Find("Viewport").Find("Content");

        _buyButton.onLeftClick = () => { DisplayBuyableItems(); };
        _sellButton.onLeftClick = () => { DisplaySellableItems(); };
    }

    private void DisplayBuyableItems()
    {
        ClearShopItemContainer();
    }

    private void DisplaySellableItems()
    {
        if (_inventory == null)
            _inventory = PlayerInventory.Instance;

        ClearShopItemContainer();
        List<MarketItem> playerItems = _market.GetPlayerItemsForSale();
        foreach (MarketItem item in playerItems)
            InstantiateShopItem(item.Item, item.Price, item.Amount);
    }

    private void ClearShopItemContainer()
    {
        for (int i = 0; i < _shopItemsContainer.childCount; i++)
            Destroy(_shopItemsContainer.GetChild(i).gameObject);
    }

    private void InstantiateShopItem(Item item, float price, int amount)
    {
        Transform shopItemTransform = Instantiate(_gameAssets.ShopItemUI, _shopItemsContainer);
        ShopItemUI shopItem = shopItemTransform.GetComponent<ShopItemUI>();
        shopItem.SetupShopItem(item, price, amount);
    }
}

public class MarketManager
{
    private static MarketManager _instance;
    public static MarketManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MarketManager();

            return _instance;
        }
    }

    private PlayerInventory _playerInventory;

    private List<Item> _soldItems = new List<Item>();
    private List<Item> _boughtItems = new List<Item>();

    private const float MARKET_SELLING_FACTOR = 1.2f;
    private const float MARKET_BUYING_FACTOR = 0.5f;

    public MarketManager()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public void SellItem(Item sold)
    {
        Item found = _soldItems.Where(item => item.ItemName == sold.ItemName).FirstOrDefault();
        if (found != null)
            return;

        _soldItems.Add(sold);
    }

    public void BuyItem(Item bought)
    {
        Item found = _boughtItems.Where(item => item.ItemName == bought.ItemName).FirstOrDefault();
        if (found != null)
            return;

        _boughtItems.Add(bought);
    }

    public List<MarketItem> GetPlayerItemsForSale()
    {
        if (_playerInventory == null)
            _playerInventory = PlayerInventory.Instance;

        List<MarketItem> marketItems = new List<MarketItem>();
        foreach (Item item in _playerInventory.GetItems())
        {
            float price = item.Price;
            int amount = 1;
            if (item is AmmoItem ammo)
            {
                amount = ammo.Amount;
                price = ammo.Price / ammo.ForAmount * amount * MARKET_BUYING_FACTOR;
            }

            marketItems.Add(new MarketItem { Item = item, Amount = amount, Price = price });
        }

        return marketItems;
    }

    public List<MarketItem> GetMarketItemsForBuying()
    {
        return new List<MarketItem>();
    }
}

public class MarketItem
{
    public Item Item { get; set; }
    public int Amount { get; set; }
    public float Price { get; set; }
}