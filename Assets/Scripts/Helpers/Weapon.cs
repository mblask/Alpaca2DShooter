[System.Serializable]
public class Weapon
{
    public WeaponItem WeaponItem;
    public int TotalAmmo;

    public Weapon(WeaponItem weaponItem, int ammo)
    {
        WeaponItem = weaponItem;
        TotalAmmo = ammo;
    }

    public Weapon(Weapon weapon)
    {
        WeaponItem = weapon.WeaponItem;
        TotalAmmo = weapon.TotalAmmo;
    }
}
