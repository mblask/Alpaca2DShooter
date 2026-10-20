[System.Serializable]
public class Weapon
{
    public WeaponItem WeaponItem;
    public int CurrentAmmo = 0;
    public int TotalAmmo;

    public Weapon(WeaponItem weaponItem, int ammo)
    {
        WeaponItem = weaponItem;
        TotalAmmo = ammo;
    }

    public Weapon(Weapon weapon)
    {
        WeaponItem = weapon.WeaponItem;
        CurrentAmmo = weapon.CurrentAmmo;
        TotalAmmo = weapon.TotalAmmo;
    }
}
