using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionArea : MonoBehaviour
{
    private float _destructionRadius = 5.0f;
    private float _destructionSpeed = 30.0f;
    private Vector2 _damage = new Vector2(20.0f, 40.0f);

    private float _scaleIncrement = 0.0f;

    private List<IDamagable> damagableList = new List<IDamagable>();

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
        _scaleIncrement = 0.0f;
    }

    private void searchForDamagables()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x);
        if (hits.Length == 0)
            return;

        foreach (Collider2D collider in hits)
        {
            IDamagable damagable = collider.gameObject.GetComponent<IDamagable>();
            if (damagable == null)
                continue;

            if (damagableList.Contains(damagable))
                continue;

            damagableList.Add(damagable);
            float damage = Random.Range(_damage.x, _damage.y) / Mathf.Pow(transform.localScale.x, 2);
            damagable.DamageObject(damage);
        }
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
