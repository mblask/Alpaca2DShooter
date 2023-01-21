using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Item", menuName = "Scriptable Objects / Non Inventory Item / Weapon", order = 0)]
public class WeaponItem : NonInventoryItem
{
    public AnimationType Trigger;
    public SFXClip WeaponShootAudio;
    public SFXClip WeaponReloadAudio;
    public bool Automatic;
    public float ShootInterval;
    public Vector2 WeaponDamage;
    public int MagazineBullets;
    public float MaxDurability;
    public bool Throwable;

    [Space]
    public float StrengthRequired;

    public override bool UseItem()
    {
        if (PlayerWeapons.Instance == null)
            return false;

        if (Throwable)
            return PlayerWeapons.Instance.AddThrowable(new Weapon(this, 1));

        return PlayerWeapons.Instance.AddWeapon(new Weapon(this, 30));
    }
}
