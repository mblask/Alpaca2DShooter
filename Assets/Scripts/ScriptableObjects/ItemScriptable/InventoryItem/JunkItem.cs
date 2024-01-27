using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Junk Item", menuName = "Scriptable Objects / Inventory Item / Junk", order = 4)]
public class JunkItem : InventoryItem
{
    public override string GetItemTooltipText()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("Junk item");
        sb.AppendLine("Used in crafting");

        return sb.ToString();
    }
}
