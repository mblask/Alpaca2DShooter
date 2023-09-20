using System.Collections.Generic;
using UnityEngine;

public class ElectricityGenerator : MonoBehaviour, IDamagable
{
    private bool _isBroken = false;
    private float _health = 50.0f;

    [SerializeField] private List<SwitchableObject> _switchables = new List<SwitchableObject>();

    private void Start()
    {
        _switchables.AddRange(transform.parent.GetComponentsInChildren<SwitchableObject>());
    }

    public void DamageObject(float value)
    {
        if (_isBroken)
            return;

        _health -= value;

        if (_health < 0.0f)
        {
            _isBroken = true;
            turnOffAttachedSwitchableObjects();
        }
    }

    private void turnOffAttachedSwitchableObjects()
    {
        _switchables.TrimExcess();
        foreach (SwitchableObject switchable in _switchables)
        {
            if (switchable == null)
                continue;

            switchable.ElectricityAvailable(false);
            switchable.TurnOff();
        }
    }
}
