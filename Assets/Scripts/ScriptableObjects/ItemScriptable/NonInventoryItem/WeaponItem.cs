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

    public override void UseItem()
    {
        PlayerWeapons.Instance?.AddWeapon(new Weapon(this, 20));
    }
}
