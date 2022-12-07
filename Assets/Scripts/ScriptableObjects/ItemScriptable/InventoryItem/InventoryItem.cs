public class InventoryItem : Item
{
    public virtual bool StoreItem()
    {
        //store item in player inventory
        return PlayerInventory.AddToInventoryStatic(this);
    }
}
