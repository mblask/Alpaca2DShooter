using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Item", menuName = "Scriptable Objects / Non Inventory Item / Ammo", order = 1)]
public class AmmoItem : NonInventoryItem
{
    [Header("Ammo characterstics")]
    public AmmoType AmmoType;
    public int Amount;

    public override bool UseItem()
    {
        return PlayerWeapons.AddAmmoStatic(this);
    }
}
