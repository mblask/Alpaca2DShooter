using UnityEngine;

using AlpacaMyGames;

public class Lamp : SwitchableObject, IInteractable, IDamagable
{
    private SpriteRenderer _spriteRenderer;
    private UnityEngine.Rendering.Universal.Light2D _light;

    private float _defaultIntensity;

    [SerializeField] private Color _defaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private Color _highlightColor = new Color(0.6f, 1.0f, 0.6f, 1.0f);

    public string InteractableName { get; } = "Lamp";

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _light = transform.Find("Light2D").GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        _defaultIntensity = _light.intensity;
    }

    private void Start()
    {
        randomWorkingLamp();
    }

    private void randomWorkingLamp()
    {
        IsOn = Utilities.ChanceFunc(50);
        _light.intensity = IsOn ? _defaultIntensity : 0.0f;
    }

    public override void TurnOn()
    {
        if (IsBroken)
            return;

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

    public override void ElectricityAvailable(bool value)
    {
        HasElectricity = value;
    }

    public override bool Toggle()
    {
        if (IsBroken)
            return false;

        if (!HasElectricity)
            return false;

        IsOn = !IsOn;
        _light.intensity = IsOn ? _defaultIntensity : 0.0f;

        return IsOn;
    }

    public void Highlight()
    {
        _spriteRenderer.color = _highlightColor;
    }

    public void Interact()
    {
        if (IsBroken)
            return;

        Toggle();
    }

    public void RemoveHighlight()
    {
        _spriteRenderer.color = _defaultColor;
    }

    public void DamageObject(float value)
    {
        IsBroken = true;
        IsOn = false;
        _light.intensity = 0.0f;
    }
}
