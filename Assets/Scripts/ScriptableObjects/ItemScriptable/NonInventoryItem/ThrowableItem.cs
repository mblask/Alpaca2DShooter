using UnityEngine;

[CreateAssetMenu(fileName = "Throwable Weapon", menuName = "Scriptable Objects / Non Inventory Item / Throwable", order = 0)]
public class ThrowableItem : NonInventoryItem
{
    [Header("Throwable characteristics")]
    public ThrowableWeaponType Type;
    public Vector2 WeaponDamage;
    public float ThrowForce;

    public override bool UseItem()
    {
        if (PlayerWeapons.Instance == null)
            return false;

        return PlayerWeapons.Instance.AddThrowable(new ThrowableWeapon(this, 1));
    }
}
