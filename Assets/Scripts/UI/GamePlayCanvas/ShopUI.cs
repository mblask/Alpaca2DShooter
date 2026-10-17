using System.Collections.Generic;
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

        DisplayBuyableItems();

        _buyButton.onLeftClick = () => { DisplayBuyableItems(); };
        _sellButton.onLeftClick = () => { DisplaySellableItems(); };
    }

    private void DisplayBuyableItems()
    {
        ClearShopItemContainer();
        List<MarketItemData> marketItems = _market.GetMarketItemsForBuying();
        foreach (MarketItemData item in marketItems)
        {
            ShopItemUI shopItemUi = InstantiateShopItem(item);
            shopItemUi.Buyable(true);
        }
    }

    private void DisplaySellableItems()
    {
        if (_inventory == null)
            _inventory = PlayerInventory.Instance;

        ClearShopItemContainer();
        List<MarketItemData> playerItems = _market.GetPlayerItemsForSale();
        foreach (MarketItemData item in playerItems)
        {
            ShopItemUI shopItemUi = InstantiateShopItem(item);
            shopItemUi.Buyable(false);
        }
    }

    private void ClearShopItemContainer()
    {
        for (int i = 0; i < _shopItemsContainer.childCount; i++)
            Destroy(_shopItemsContainer.GetChild(i).gameObject);
    }

    private ShopItemUI InstantiateShopItem(MarketItemData marketItem)
    {
        Transform shopItemTransform = Instantiate(_gameAssets.ShopItemUI, _shopItemsContainer);
        ShopItemUI shopItem = shopItemTransform.GetComponent<ShopItemUI>();
        shopItem.SetupShopItem(marketItem);
        
        return shopItem;
    }
}