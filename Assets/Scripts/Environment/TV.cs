using AlpacaMyGames;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TV : SwitchableObject
{
    private Light2D _tvPointLight;

    private bool _isOn = false;
    private bool _isBroken = false;

    private void Awake()
    {
        _tvPointLight = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        _tvPointLight = GetComponentInChildren<Light2D>();

        _isOn = Utilities.ChanceFunc(50);
        Toggle();
    }

    public override void TurnOn()
    {
        if (_isBroken)
            return;

        _isOn = true;
        _tvPointLight.intensity = 1.0f;
    }

    public override void TurnOff()
    {
        _isOn = false;
        _tvPointLight.intensity = 0.0f;
    }

    public override void Disable(bool value)
    {
        _isBroken = value;
    }

    public override bool Toggle()
    {
        if (_isBroken)
            return false;

        _isOn = !_isOn;
        _tvPointLight.intensity = _isOn ? 1.0f : 0.0f;

        return _isOn;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet != null)
        {
            _isOn = false;
            _isBroken = true;
            _tvPointLight.intensity = 0.0f;
        }

        Destroy(collision.gameObject);
    }
}
