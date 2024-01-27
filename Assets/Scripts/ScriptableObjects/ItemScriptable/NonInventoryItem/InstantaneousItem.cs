using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Instantaneous Item", menuName = "Scriptable Objects / Non Inventory Item / Instantaneous", order = 1)]
public class InstantaneousItem : NonInventoryItem
{
    [Header("Instantaneous characteristics")]
    public Vector2 LifeRestored;
    public Vector2 StaminaRestored;
    public float Armor;

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

        return sb.ToString();
    }

    public override bool UseItem()
    {
        if (PlayerStats.Instance == null)
            return false;

        return PlayerStats.Instance.UseItem(this);
    }
}
