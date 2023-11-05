using AlpacaMyGames;
using UnityEngine;


public class TV : SwitchableObject
{
    private UnityEngine.Rendering.Universal.Light2D _tvPointLight;

    private void Awake()
    {
        _tvPointLight = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();
    }

    private void Start()
    {
        _tvPointLight = GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>();

        IsOn = Utilities.ChanceFunc(50);
        Toggle();
    }

    public override void TurnOn()
    {
        if (IsBroken)
            return;

        if (!HasElectricity)
            return;

        IsOn = true;
        _tvPointLight.intensity = 1.0f;
    }

    public override void TurnOff()
    {
        IsOn = false;
        _tvPointLight.intensity = 0.0f;
    }

    public override void ElectricityAvailable(bool value)
    {
        HasElectricity = value;
    }

    public override bool Toggle()
    {
        if (IsBroken)
            return false;

        if (HasElectricity)
            return false;

        IsOn = !IsOn;
        _tvPointLight.intensity = IsOn ? 1.0f : 0.0f;

        return IsOn;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();

        if (bullet != null)
        {
            IsOn = false;
            IsBroken = true;
            _tvPointLight.intensity = 0.0f;
        }

        Destroy(collision.gameObject);
    }
}
