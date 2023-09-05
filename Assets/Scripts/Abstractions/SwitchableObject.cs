using UnityEngine;

public abstract class SwitchableObject : MonoBehaviour
{
    /// <summary>
    /// Is the <see cref="SwitchableObject"/> turned On
    /// </summary>
    [field: SerializeField] public bool IsOn { get; protected set; }
    /// <summary>
    /// Is the <see cref="SwitchableObject"/> broken
    /// </summary>
    [field: SerializeField] public bool IsBroken { get; protected set; } = false;
    /// <summary>
    /// Does the <see cref="SwitchableObject"/> have electricity available
    /// </summary>
    [field: SerializeField] public bool HasElectricity { get; protected set; } = true;
    /// <summary>
    /// Sets the availability of electricity
    /// </summary>
    /// <param name="value"></param>
    public abstract void ElectricityAvailable(bool value);
    /// <summary>
    /// Turns the <see cref="SwitchableObject"/> on
    /// </summary>
    public abstract void TurnOn();
    /// <summary>
    /// Turns the <see cref="SwitchableObject"/> off
    /// </summary>
    public abstract void TurnOff();
    /// <summary>
    /// Toggles the <see cref="SwitchableObject"/> on or off
    /// </summary>
    /// <returns></returns>
    public abstract bool Toggle();
}
