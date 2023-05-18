using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroy : MonoBehaviour, IDamagable
{
    [Header("Object sturdiness")]
    [Tooltip("Affects how many bullets needs to destroy the object.")]
    [SerializeField] private ObjectSturdiness _sturdiness;

    [Header("Drop items on destroy")]
    [SerializeField] private bool _dropRandomItem = false;
    [SerializeField] private Item _dropItem;

    [Header("Particle system settings")]
    [SerializeField] private bool _useOriginalSprite;
    [SerializeField] private Sprite _particleSprite;
    [SerializeField] private Color _particleColor;

    private int _bulletHitsToDestroy;
    private int _hitCount = 0;

    private string _bulletTagName = "Bullet";

    private void Start()
    {
        _bulletHitsToDestroy = sturdinessToBulletNum(_sturdiness);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_bulletTagName))
        {
            DamageObject();

            Destroy(collision.gameObject);
        }
    }

    public void DamageObject(float damage = 1.0f)
    {
        _hitCount -= (int)damage;

        if (_hitCount <= 0)
        {
            destroyObject();
        }
    }

    private void destroyObject()
    {
        generateParticles();
        dropItemOnDestroy();
        Destroy(gameObject);
    }

    private void dropItemOnDestroy()
    {
        if (!_dropRandomItem && _dropItem == null)
            return;

        if (_dropRandomItem)
            ItemSpawner.Instance.SpawnRandomItemAt(transform.position);

        if (_dropItem != null)
            ItemSpawner.Instance.SpawnItem(transform.position, _dropItem);
    }

    private void generateParticles()
    {
        ParticleSystem objectDestroyPS = Instantiate(GameAssets.Instance.ObjectDestroyPS, transform.position, Quaternion.identity, null);
        ParticleSystem.MainModule mainModule = objectDestroyPS.main;

        if (_particleSprite == null)
        {
            SpriteRenderer objectSpriteRenderer = GetComponent<SpriteRenderer>();
            objectDestroyPS.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, objectSpriteRenderer.sprite);
            mainModule.startColor = objectSpriteRenderer.color;
            return;
        }

        objectDestroyPS.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, _particleSprite);
        mainModule.startColor = _particleColor;
    }

    private int sturdinessToBulletNum(ObjectSturdiness sturdiness)
    {
        switch (sturdiness)
        {
            case ObjectSturdiness.Weak:
                return Random.Range(1, 3);
            case ObjectSturdiness.Medium:
                return Random.Range(3, 5);
            case ObjectSturdiness.Strong:
                return Random.Range(5, 7);
            case ObjectSturdiness.Sturdy:
                return Random.Range(7, 9);
            case ObjectSturdiness.Indestructible:
                return 9999;
            default:
                return 9999;
        }
    }
}
