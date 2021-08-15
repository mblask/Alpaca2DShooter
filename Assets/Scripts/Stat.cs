using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int _baseValue = 0;

    [Header("Read-only")]
    [SerializeField] private int _finalValue = 0;

    private List<int> _modifiers = new List<int>();

    public int GetBaseValue()
    {
        return _baseValue;
    }

    public int GetValue()
    {
        _finalValue = _baseValue;
        _modifiers.ForEach(x => _finalValue += x);

        return _finalValue;
    }

    public void AddModifier(int modifier)
    {
        if (modifier != 0)
            _modifiers.Add(modifier);
    }

    public void RemoveModifier(int modifier)
    {
        if (modifier != 0)
        {
            _modifiers.Remove(modifier);
        }
    }
}
