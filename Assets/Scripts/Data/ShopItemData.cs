using System;

[Serializable]
public class ShopItemData
{
    public Item Item;
    public float Price;
    public int Amount;
    public bool Buyable = true;

    public ShopItemData() { }

    public ShopItemData(MarketItemData marketItem)
    {
        Item = marketItem.Item;
        Price = marketItem.Price;
        Amount = marketItem.Amount;
    }
}
