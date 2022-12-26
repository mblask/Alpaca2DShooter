using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MainLightingManager : MonoBehaviour
{
    private static MainLightingManager _instance;

    private Light2D _mainLight;
    private Vector2 _mainLightingStrength = new Vector2(0.1f, 0.8f);

    private bool _flickerLight = true;

    private void Awake()
    {
        _instance = this;
        _mainLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        setRandomLightIntensity();
    }

    #region Square sine light intensity
    private void SineSquareLightIntensityStatic(float speed, float duration = 0.0f, float amp = 0.5f, float min = 0.25f)
    {
        _instance?.sineSquareLightIntensity(speed, duration, amp, min);
    }

    private void stopLightingCoroutines()
    {
        _flickerLight = false;
        StopCoroutine(nameof(sineSquareLightIntensityCoroutine));
    }

    private void sineSquareLightIntensity(float speed, float duration = 0.0f, float amp = 0.5f, float min = 0.25f)
    {
        StartCoroutine(sineSquareLightIntensityCoroutine(speed, duration, amp, min));
    }

    private IEnumerator sineSquareLightIntensityCoroutine(float speed, float duration, float amp, float min)
    {
        Color defaultColor = _mainLight.color;
        _mainLight.color = Color.red;
        float timer = 0.0f;

        while (_flickerLight)
        {
            float phase = speed * Mathf.PI;
            float lightIntensityMaximum = 1.0f;
            float intensity = (lightIntensityMaximum - min) - amp * Mathf.Pow(Mathf.Sin(Time.time * phase), 2);
            setLightIntensity(intensity);

            if (duration == 0.0f)
                continue;

            timer += Time.deltaTime;

            if (timer >= duration)
                break;

            yield return null;
        }

        _flickerLight = false;
        _mainLight.color = defaultColor;

        StopCoroutine(nameof(sineSquareLightIntensityCoroutine));
    }
    #endregion

    private void setLightIntensity(float intensity)
    {
        _mainLight.intensity = intensity;
    }

    private void setRandomLightIntensity()
    {
        float intensity = Random.Range(_mainLightingStrength.x, _mainLightingStrength.y);
        setLightIntensity(intensity);
    }
}
