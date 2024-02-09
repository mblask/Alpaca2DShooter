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

    public override bool UseItem()
    {
        return true;
    }

    public virtual bool StoreItem()
    {
        return PlayerInventory.AddToInventoryStatic(this);
    }
}
