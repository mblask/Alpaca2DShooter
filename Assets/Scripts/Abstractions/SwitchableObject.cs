using UnityEngine;

public abstract class SwitchableObject : MonoBehaviour
{
    public abstract void TurnOn();
    public abstract void TurnOff();
    public abstract bool Toggle();
}
