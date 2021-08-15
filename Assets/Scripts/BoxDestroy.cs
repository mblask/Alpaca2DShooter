using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroy : MonoBehaviour
{
    public GameObject BoxDestroyPS;

    private int _bulletHitsToDestroy;
    private int _hitCount = 0;

    private void Start()
    {
        _bulletHitsToDestroy = Random.Range(2, 5);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            _hitCount++;

            if (_hitCount >= _bulletHitsToDestroy)
            {
                GameObject boxDextroyPS = Instantiate(BoxDestroyPS, transform.position, Quaternion.identity);
                boxDextroyPS.transform.parent = null;

                Destroy(gameObject);
            }
        }
    }
}
