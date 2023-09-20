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

    public override bool UseItem()
    {
        if (PlayerStats.Instance == null)
            return false;

        if (PlayerStats.Instance.UseItem(this))
            return PlayerInventory.DeleteItemFromInventoryStatic(this);

        return false;
    }
}
