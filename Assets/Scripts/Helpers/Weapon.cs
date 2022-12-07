using UnityEngine;

[System.Serializable]
public class Weapon
{
    public WeaponItem WeaponItem;
    public float Durability;
    public int TotalAmmo;

    public Weapon(WeaponItem weaponItem, int ammo)
    {
        WeaponItem = weaponItem;
        Durability = weaponItem.MaxDurability;
        TotalAmmo = ammo;
    }

    public Weapon(Weapon weapon)
    {
        WeaponItem = weapon.WeaponItem;
        Durability = weapon.Durability;
        TotalAmmo = weapon.TotalAmmo;
    }
}
