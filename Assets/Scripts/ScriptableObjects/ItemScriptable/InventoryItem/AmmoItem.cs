using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Item", menuName = "Scriptable Objects / Non Inventory Item / Ammo", order = 1)]
public class AmmoItem : InventoryItem
{
    [Header("Ammo characterstics")]
    public AmmoType AmmoType;
    public int Amount;

    public override bool UseItem()
    {
        if (!PlayerWeapons.AddAmmoStatic(this))
            return PlayerInventory.AddToInventoryStatic(this);

        return true;
    }
}
