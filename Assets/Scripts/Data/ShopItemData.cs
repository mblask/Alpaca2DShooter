using System;

[Serializable]
public class ShopItemData
{
    public Item Item { get; set; }
    public float Price { get; set; }
    public int Amount { get; set; }
    public bool Buyable { get; set; } = true;

    public ShopItemData() { }

    public ShopItemData(MarketItemData marketItem)
    {
        Item = marketItem.Item;
        Price = marketItem.Price;
        Amount = marketItem.Amount;
    }
}
