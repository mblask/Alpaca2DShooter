using UnityEngine;

public class FlashGrenade : AbstractGranade
{
    protected override void explode()
    {
        DestructionArea destructionArea = Instantiate(_gameAssets.DestructionArea, transform.position, Quaternion.identity, null).GetComponent<DestructionArea>();
        destructionArea.SetDestructionRadius(_explosionRadius);
        destructionArea.SetAreaType(AreaOfEffectType.Blinding);
        destructionArea.SetDamage(Vector2.zero);

        Destroy(gameObject);
    }
}
