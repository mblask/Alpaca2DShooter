using UnityEngine;

public class Grenade : AbstractGranade
{
    protected override void explode()
    {
        DestructionArea destructionArea = Instantiate(_gameAssets.DestructionArea, transform.position, Quaternion.identity, null).GetComponent<DestructionArea>();
        destructionArea.SetDestructionRadius(_explosionRadius);
        destructionArea.SetDamage(_item.WeaponDamage);
        
        Destroy(gameObject);
    }
}
