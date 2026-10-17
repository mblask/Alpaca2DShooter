using System.Collections.Generic;
using System.Linq;

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

    private GameAssets _gameAssets;
    private PlayerInventory _playerInventory;

    private List<Item> _itemPool = new List<Item>();

    private const float MARKET_SELLING_FACTOR = 1.2f;
    private const float MARKET_BUYING_FACTOR = 0.5f;

    public MarketManager()
    {
        if (_instance != null)
            return;

        _instance = this;
    }

    public void SellItem(ShopItemData itemToSell)
    {
        _playerInventory.AddCredits(itemToSell.Price);
        _playerInventory.DeleteItemFromInventory(itemToSell.Item);
    }

    public void BuyItem(ShopItemData itemToBuy)
    {
        if (itemToBuy.Price > _playerInventory.GetCredits())
            return;

        _playerInventory.AddCredits(-1 * itemToBuy.Price);
        _playerInventory.AddToInventory(itemToBuy.Item);
    }

    public List<MarketItemData> GetPlayerItemsForSale()
    {
        if (_playerInventory == null)
            _playerInventory = PlayerInventory.Instance;

        List<MarketItemData> marketItems = new List<MarketItemData>();
        foreach (Item item in _playerInventory.GetItems())
        {
            float price = item.Price;
            int amount = 1;
            if (item is AmmoItem ammo)
            {
                amount = ammo.Amount;
                price = ammo.Price / ammo.ForAmount * amount;
            }

            marketItems.Add(new MarketItemData { Item = item, Amount = amount, Price = price * MARKET_BUYING_FACTOR });
        }

        return marketItems;
    }

    public List<MarketItemData> GetMarketItemsForBuying()
    {
        if (!_itemPool.Any())
            PopulateItemPool();

        List<MarketItemData> marketItems = new List<MarketItemData>();
        foreach (Item item in _itemPool)
        {
            int amount = 1;
            float price = item.Price;

            if (item is AmmoItem ammo)
            {
                amount = ammo.Amount;
                price = ammo.Price / ammo.ForAmount * amount;
            }

            marketItems.Add(new MarketItemData { Item = item, Amount = amount, Price = price * MARKET_SELLING_FACTOR });
        }

        return marketItems;
    }

    private void PopulateItemPool()
    {
        if (_gameAssets == null)
            _gameAssets = GameAssets.Instance;

        _itemPool.AddRange(_gameAssets.AvailableAmmoItems);
        _itemPool.AddRange(_gameAssets.Consumables);
        _itemPool.AddRange(_gameAssets.JunkItems);
        _itemPool.AddRange(_gameAssets.WeaponsList);
        _itemPool.AddRange(_gameAssets.ThrowablesList);
    }
}
