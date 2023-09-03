using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using AlpacaMyGames;

public class Lamp : SwitchableObject, IInteractable, IDamagable
{
    private SpriteRenderer _spriteRenderer;
    private Light2D _light;

    private float _defaultIntensity;
    private bool _isBroken = false;
    [SerializeField] private bool _isOn = false;

    [SerializeField] private Color _defaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    [SerializeField] private Color _highlightColor = new Color(0.6f, 1.0f, 0.6f, 1.0f);

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _light = transform.Find("Light2D").GetComponent<Light2D>();
        _defaultIntensity = _light.intensity;
    }

    private void Start()
    {
        randomWorkingLamp();
    }

    private void randomWorkingLamp()
    {
        _isOn = Utilities.ChanceFunc(50);
        _light.intensity = _isOn ? _defaultIntensity : 0.0f;
    }

    public override void TurnOn()
    {
        _isOn = true;
        _light.intensity = _defaultIntensity;
    }

    public override void TurnOff()
    {
        _isOn = true;
        _light.intensity = 0.0f;
    }

    public override bool Toggle()
    {
        _isOn = !_isOn;
        _light.intensity = !_isOn ? _defaultIntensity : 0.0f;

        return _isOn;
    }

    public void Highlight()
    {
        if (_isBroken)
            return;

        _spriteRenderer.color = _highlightColor;
    }

    public void Interact()
    {
        if (_isBroken)
            return;

        Toggle();
    }

    public void RemoveHighlight()
    {
        if (_isBroken)
            return;

        _spriteRenderer.color = _defaultColor;
    }

    public void DamageObject(float value)
    {
        _isBroken = true;
        _isOn = false;
        _light.intensity = 0.0f;
    }
}
