using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSpriteOnStart : MonoBehaviour
{
    [Tooltip("Has a 50% chance not to spawn")]
    public bool SpawnRandomly;
    public bool HasRandomRotation;

    [Header("Fading")]
    public bool ShouldFade;
    public float FadeSpeed = 1.0f;

    [Space]
    [Tooltip("All sprites related to this object")]
    public List<Sprite> Sprites;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (SpawnRandomly)
        {
            if (0 == Random.Range(0, 2))
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }

                Destroy(gameObject);
            }
        }

        if (Sprites.Count > 1)
            _spriteRenderer.sprite = Sprites[Random.Range(0, Sprites.Count)];

        if (HasRandomRotation)
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
    }

    private void Update()
    {
        if (ShouldFade)
            fade();
    }

    private void fade()
    {
        Color color = _spriteRenderer.color;
        color.a -= FadeSpeed * Time.deltaTime;

        if (color.a <= 0.0f)
            Destroy(gameObject);

        _spriteRenderer.color = color;
    }
}
