using System;

[Serializable]
public class ThrowableWeapon
{
    public ThrowableItem ThrowableItem;
    public int TotalAmmo;

    public ThrowableWeapon(ThrowableItem throwableItem, int ammo)
    {
        ThrowableItem = throwableItem;
        TotalAmmo = ammo;
    }

    public ThrowableWeapon(ThrowableWeapon throwableWeapon)
    {
        ThrowableItem = throwableWeapon.ThrowableItem;
        TotalAmmo = throwableWeapon.TotalAmmo;
    }
}
