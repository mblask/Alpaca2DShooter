using System.Text;
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
    public float BleedingChance;

    [Header("Required to use")]
    public float StrengthRequired;

    [Header("Enemy use chance")]
    public float UseChance;

    public override string GetItemTooltipText()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Damage: ");
        sb.Append(WeaponDamage.x.ToString());
        sb.Append(" - ");
        sb.Append(WeaponDamage.y.ToString());

        if (MagazineBullets > 0)
        {
            sb.AppendLine();
            sb.Append("Mag. Capacity: ");
            sb.Append(MagazineBullets.ToString());
        }

        if (Automatic)
        {
            sb.AppendLine();
            sb.Append("Automatic");
        }

        if (StrengthRequired > 0)
        {
            sb.AppendLine();
            sb.Append("Strength: ");
            sb.Append(StrengthRequired.ToString());
        }

        return sb.ToString();
    }

    public override bool UseItem()
    {
        if (PlayerWeapons.Instance == null)
            return false;

        return PlayerWeapons.Instance.AddWeapon(new Weapon(this, 30));
    }
}
