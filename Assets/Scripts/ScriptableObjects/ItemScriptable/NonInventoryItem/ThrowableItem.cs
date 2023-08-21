using UnityEngine;

[CreateAssetMenu(fileName = "Throwable Weapon", menuName = "Scriptable Objects / Non Inventory Item / Throwable", order = 0)]
public class ThrowableItem : WeaponItem
{
    [Header("Throwable characteristics")]
    public ThrowableWeaponType Type;
    public float ThrowForce;

    public override bool UseItem()
    {
        if (PlayerWeapons.Instance == null)
            return false;

        return PlayerWeapons.Instance.AddThrowable(new ThrowableWeapon(this, 1));
    }
}
