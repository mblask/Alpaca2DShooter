using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MainLightingManager : MonoBehaviour
{
    private Light2D _mainLight;
    private Vector2 _mainLightingStrength = new Vector2(0.1f, 0.8f);

    private void Awake()
    {
        _mainLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        _mainLight.intensity = Random.Range(_mainLightingStrength.x, _mainLightingStrength.y);
    }
}
