using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MainLightingManager : MonoBehaviour
{
    private static MainLightingManager _instance;

    public static float GlobalIntensity
    {
        get
        {
            if (_instance == null)
                return 0.0f;

            return _instance._mainLight.intensity;
        }
    }

    public static float MaxIntensity
    {
        get
        {
            if (_instance == null)
                return 0.5f;

            return _instance._mainLightingStrength.y;
        }
    }

    private Light2D _mainLight;
    private Vector2 _mainLightingStrength = new Vector2(0.1f, 0.5f);

    private void Awake()
    {
        _instance = this;
        _mainLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        setRandomLightIntensity();
    }

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
