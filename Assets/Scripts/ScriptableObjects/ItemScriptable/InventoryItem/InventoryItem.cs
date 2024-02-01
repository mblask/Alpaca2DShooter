using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory Item", menuName = "Scriptable Objects / Inventory Item / Inventory Item", order = 0)]
public class InventoryItem : Item
{
    public override string GetItemTooltipText()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("Inventory item");

        return sb.ToString();
    }

    public virtual bool StoreItem()
    {
        //store item in player inventory
        return PlayerInventory.AddToInventoryStatic(this);
    }
}
