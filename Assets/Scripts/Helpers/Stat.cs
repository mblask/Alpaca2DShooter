using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private StatType _statType;
    [SerializeField] private float _baseValue = 0;
    private float _finalValue = 0;
    private List<float> _modifiers;
    private List<float> _multipliers;

    private bool _isHandicaped = false;

    public Stat()
    {
        _modifiers = new List<float>();
        _multipliers = new List<float>();
    }

    public Stat(StatType statType, float baseValue)
    {
        _statType= statType;
        _baseValue = baseValue;
    }

    public void SetStatType(StatType statType)
    {
        _statType = statType;
    }

    public float GetBaseValue()
    {
        return _baseValue;
    }

    public void SetBaseValue(float value)
    {
        _baseValue = value;
    }

    public float GetFinalValue()
    {
        _finalValue = _baseValue;
        _multipliers.ForEach(x => _finalValue *= x);
        _modifiers.ForEach(x => _finalValue += x);

        return _finalValue;
    }

    public void AddModifier(float modifier)
    {
        if (modifier != 0.0f)
            _modifiers.Add(modifier);
    }

    public void RemoveModifier(float modifier)
    {
        if (modifier != 0)
            _modifiers.Remove(modifier);
    }

    public void AddBaseMultiplier(float multiplier)
    {
        if (multiplier != 1.0f)
            _multipliers.Add(multiplier);
    }
    
    public void RemoveBaseMultiplier(float multiplier)
    {
        if (multiplier != 1.0f)
            _multipliers.Remove(multiplier);
    }

    public void SetHandicaped(bool value)
    {
        _isHandicaped = value;
    }

    public bool IsHandicaped()
    {
        return _isHandicaped;
    }
}
