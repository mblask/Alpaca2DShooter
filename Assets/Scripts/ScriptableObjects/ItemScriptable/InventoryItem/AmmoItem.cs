using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Item", menuName = "Scriptable Objects / Non Inventory Item / Ammo", order = 1)]
public class AmmoItem : InventoryItem
{
    [Header("Ammo characterstics")]
    public AmmoType AmmoType;
    public int Amount;
    public SFXClip LoadAudio;

    public override bool UseItem()
    {
        if (PlayerWeapons.AddAmmoStatic(this))
            return PlayerInventory.DeleteItemFromInventoryStatic(this);

        return false;
    }
}
