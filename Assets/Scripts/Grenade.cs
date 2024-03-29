using UnityEngine;

public class Grenade : AbstractGranade
{
    protected override void explode()
    {
        AreaOfEffect destructionArea = 
            Instantiate(_gameAssets.DestructionArea, transform.position, Quaternion.identity, null)
            .GetComponent<AreaOfEffect>();
        destructionArea.SetDestructionRadius(_explosionRadius);
        destructionArea.SetDamage(_item.WeaponDamage);

        CameraController.Instance.ShakeCamera(0.1f, 0.15f);
        
        Destroy(gameObject);
    }
}
