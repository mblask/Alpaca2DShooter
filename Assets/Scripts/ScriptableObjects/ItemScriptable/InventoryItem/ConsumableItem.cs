using UnityEngine;

[CreateAssetMenu(fileName = "Consumable Item", menuName = "Scriptable Objects / Inventory Item / Consumable", order = 3)]
public class ConsumableItem : InventoryItem
{
    public Vector2 LifeRestored;
    public Vector2 StaminaRestored;
    public bool LimbPatcher;
    public Vector2 LimbToughnessDuration;

    public override void UseItem()
    {
        PlayerStats.Instance?.HealCharacter(this);
    }
}
