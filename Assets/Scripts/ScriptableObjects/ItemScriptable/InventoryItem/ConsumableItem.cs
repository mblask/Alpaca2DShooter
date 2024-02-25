using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable Item", menuName = "Scriptable Objects / Inventory Item / Consumable", order = 3)]
public class ConsumableItem : InventoryItem
{
    [Header("Consumable characteristics")]
    public ConsumableType ConsumableType;
    public Vector2 LifeRestored;
    public Vector2 StaminaRestored;
    public bool LimbPatcher;
    /// <summary>
    /// X value is toughness gained, Y value is duration during which the effect is valid
    /// </summary>
    public Vector2 LimbToughnessDuration;

    public override string GetItemTooltipText()
    {
        StringBuilder sb = new StringBuilder();

        if (LifeRestored != Vector2.zero)
        {
            sb.Append("Restores Life:");
            sb.AppendLine();
            sb.Append(LifeRestored.x.ToString() + " to " + LifeRestored.y.ToString());
            sb.AppendLine();
        }

        if (StaminaRestored != Vector2.zero)
        {
            sb.Append("Restores Stamina:");
            sb.AppendLine();
            sb.Append(StaminaRestored.x.ToString() + " to " + StaminaRestored.y.ToString());
        }

        if (LimbPatcher)
        {
            sb.Append("Limb Patcher");
            sb.AppendLine();
        }

        if (LimbToughnessDuration != Vector2.zero)
        {
            char sign = Mathf.Sign(LimbToughnessDuration.x) > 0.0f ? '+' : '-';
            sb.Append("Toughness " + sign + LimbToughnessDuration.x + "% (" + LimbToughnessDuration.y + "s)");
        }

        return sb.ToString();
    }

    public override bool UseItem()
    {
        if (PlayerStats.Instance == null)
            return false;

        if (PlayerStats.Instance.UseItem(this))
            return PlayerInventory.DeleteItemFromInventoryStatic(this);

        return false;
    }
}
