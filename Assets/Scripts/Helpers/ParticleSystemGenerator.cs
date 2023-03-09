using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemGenerator : MonoBehaviour, IParticleSystemGenerator
{
    private Transform _spawnTransform;
    private bool _useOriginalSprite;
    private Sprite _particleSprite;
    private Color _particleColor;

    public ParticleSystemGenerator(Transform spawnTransform)
    {
        _spawnTransform = spawnTransform;
        _useOriginalSprite = true;
        _particleSprite = null;
        _particleColor = Color.white;
    }

    public ParticleSystemGenerator(Transform spawnTransform, bool useOriginalSprite, Sprite particleSprite, Color particleColor)
    {
        _spawnTransform = spawnTransform;
        _useOriginalSprite = useOriginalSprite;
        _particleSprite = particleSprite;
        _particleColor = particleColor;
    }

    public void Generate()
    {
        ParticleSystem objectDestroyPS = Instantiate(GameAssets.Instance.ObjectDestroyPS, _spawnTransform.position, Quaternion.identity, null);
        ParticleSystem.MainModule mainModule = objectDestroyPS.main;

        if (_useOriginalSprite)
        {
            SpriteRenderer objectSpriteRenderer = GetComponent<SpriteRenderer>();
            objectDestroyPS.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, objectSpriteRenderer.sprite);
            mainModule.startColor = objectSpriteRenderer.color;
            return;
        }

        if (_particleSprite == null)
            return;

        objectDestroyPS.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, _particleSprite);
        mainModule.startColor = _particleColor;
    }
}
