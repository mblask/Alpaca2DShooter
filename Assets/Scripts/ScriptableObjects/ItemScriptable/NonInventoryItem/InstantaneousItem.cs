using UnityEngine;

[CreateAssetMenu(fileName = "Instantaneous Item", menuName = "Scriptable Objects / Non Inventory Item / Instantaneous", order = 1)]
public class InstantaneousItem : NonInventoryItem
{
    public Vector2 LifeRestored;
    public Vector2 StaminaRestored;

    public override bool UseItem()
    {
        if (PlayerStats.Instance == null)
            return false;

        return PlayerStats.Instance.HealCharacter(this);
    }
}
