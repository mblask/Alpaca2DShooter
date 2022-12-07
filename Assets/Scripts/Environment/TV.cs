using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TV : MonoBehaviour
{
    private Light2D _tvPointLight;

    private void Awake()
    {
        _tvPointLight = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        _tvPointLight = GetComponentInChildren<Light2D>();

        activateTV(1 == Random.Range(0, 2));
    }

    private void activateTV(bool value)
    {
        if (value)
            _tvPointLight.intensity = 1.0f;
        else
            _tvPointLight.intensity = 0.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet != null)
            activateTV(false);

        Destroy(collision.gameObject);
    }
}
