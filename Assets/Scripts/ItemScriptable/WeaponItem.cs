using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Item", menuName = "Scriptable Objects / Weapon", order = 0)]
public class WeaponItem : Item
{
    public string WeaponTrigger;
    public bool Automatic;
    public float ShootInterval;
    public int WeaponDamage;
    public int MagazineBullets;
    public int TotalAvailableBullets;

    [Header("Read-only")]
    public int CurrentMagazineBullets;

    public void AddToTotalAmmo()
    {
        TotalAvailableBullets += MagazineBullets;
    }
}
