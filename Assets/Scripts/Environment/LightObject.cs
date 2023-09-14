using UnityEngine.Experimental.Rendering.Universal;

public class LightObject : SwitchableObject
{
    private Light2D _light;
    private float _defaultIntensity;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _defaultIntensity = _light.intensity;
    }

    public override void ElectricityAvailable(bool value)
    {
        HasElectricity = value;
    }

    public override void TurnOn()
    {
        if (!HasElectricity)
            return;

        IsOn = true;
        _light.intensity = _defaultIntensity;
    }

    public override void TurnOff()
    {
        IsOn = false;
        _light.intensity = 0.0f;
    }

    public override bool Toggle()
    {
        if (!HasElectricity)
            return false;

        IsOn = !IsOn;

        _light.intensity = IsOn ? _defaultIntensity : 0.0f;

        return IsOn;
    }
}
