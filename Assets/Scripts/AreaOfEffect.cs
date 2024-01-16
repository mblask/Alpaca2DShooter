using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    private float _destructionRadius = 5.0f;
    private float _destructionSpeed = 30.0f;
    private Vector2 _damage = new Vector2(20.0f, 40.0f);

    private AreaOfEffectType _areaOfEffectType;

    private float _scaleIncrement = 0.0f;

    private List<IDamagable> _damagableList = new List<IDamagable>();
    private List<IBlindable> _blindableList = new List<IBlindable>();

    private void Update()
    {
        _scaleIncrement += _destructionSpeed * Time.deltaTime;
        transform.localScale += new Vector3(1.0f, 1.0f, 0.0f) * _destructionSpeed * Time.deltaTime;

        if (transform.localScale.x > _destructionRadius)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);
            Destroy(gameObject);
        }

        if (_scaleIncrement < 0.1f)
            return;
        
        searchForDamagables();
        searchForBlindable();
        _scaleIncrement = 0.0f;
    }

    private void searchForBlindable()
    {
        if (!_areaOfEffectType.Equals(AreaOfEffectType.Blinding))
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x);
        if (hits.Length == 0)
            return;

        foreach (Collider2D hit in hits)
        {
            IBlindable blindable = hit.gameObject.GetComponent<IBlindable>();
            if (blindable == null)
                continue;

            if (_blindableList.Contains(blindable))
                continue;

            _blindableList.Add(blindable);
            blindable.Blind();
        }
    }

    private void searchForDamagables()
    {
        if (!_areaOfEffectType.Equals(AreaOfEffectType.Damaging))
            return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x);
        if (hits.Length == 0)
            return;

        foreach (Collider2D collider in hits)
        {
            IDamagable damagable = collider.gameObject.GetComponent<IDamagable>();
            if (damagable == null)
                continue;

            if (_damagableList.Contains(damagable))
                continue;

            _damagableList.Add(damagable);
            float damage = _damage.GetRandom() / Mathf.Pow(transform.localScale.x, 2);
            damagable.DamageObject(new DamageData { Damage = damage });
        }
    }

    public void SetAreaType(AreaOfEffectType type)
    {
        _areaOfEffectType = type;
    }

    public void SetDestructionRadius(float destructionRadius)
    {
        _destructionRadius = destructionRadius;
    }

    public void SetDamage(Vector2 damageVector)
    {
        _damage = damageVector;
    }
}
