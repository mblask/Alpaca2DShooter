using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TVScript : MonoBehaviour
{
    public bool OnOff = true;

    private Light2D _tvPointLight;

    void Start()
    {
        _tvPointLight = GetComponentInChildren<Light2D>();

        activateTV(OnOff);
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
        if (collision.CompareTag("Bullet"))
            activateTV(false);

        Destroy(collision.gameObject);
    }
}
