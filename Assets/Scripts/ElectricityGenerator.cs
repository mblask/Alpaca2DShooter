using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricityGenerator : MonoBehaviour, IDamagable
{
    private bool _isBroken = false;
    private float _health = 50.0f;

    [SerializeField]
    private List<SwitchableObject> _switchables = new List<SwitchableObject>();

    private void Start()
    {
        List<SwitchableObject> switchables = transform.parent != null ?
            transform.parent.GetComponentsInParent<SwitchableObject>().ToList() : new List<SwitchableObject>();
        _switchables.AddRange(switchables);
    }

    public void DamageObject(DamageData damageData)
    {
        if (_isBroken)
            return;

        _health -= damageData.Damage;

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
