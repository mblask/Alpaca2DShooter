using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Item", menuName = "Scriptable Objects / Non Inventory Item / Weapon", order = 0)]
public class WeaponItem : NonInventoryItem
{
    [Header("Weapon characteristics")]
    public AnimationType Trigger;
    public SFXClip WeaponShootAudio;
    public SFXClip WeaponReloadAudio;
    public bool Automatic;
    public float ShootInterval;
    public Vector2 WeaponDamage;
    public int MagazineBullets;
    public AmmoType AmmoType;

    [Space]
    public float StrengthRequired;

    public override bool UseItem()
    {
        if (PlayerWeapons.Instance == null)
            return false;

        return PlayerWeapons.Instance.AddWeapon(new Weapon(this, 30));
    }
}
