using AlpacaMyGames;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroy : MonoBehaviour, IDamagable
{
    [Header("Drop items on destroy")]
    [SerializeField] private bool _dropRandomItem = false;
    [SerializeField] private Item _dropItem;
    [SerializeField] private List<Item> _dropItemList = new List<Item>();

    [Header("Particle system settings")]
    [SerializeField] private bool _useOriginalSprite;
    [SerializeField] private Sprite _particleSprite;
    [SerializeField] private Color _particleColor;

    private int _hitCount = 0;

    private string _bulletTagName = "Bullet";

    private void Start()
    {
        _hitCount = Random.Range(0, 2);
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

        if (Utilities.ChanceFunc(50))
            return;

        if (_dropRandomItem)
            ItemSpawner.Instance.SpawnRandomItemAt(transform.position);

        if (_dropItemList.Count > 0)
            ItemSpawner.Instance.SpawnItem(transform.position, _dropItemList.GetRandomElement());            

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
}
