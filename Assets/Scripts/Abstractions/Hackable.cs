using UnityEngine;

public abstract class Hackable : MonoBehaviour
{
    /// <summary>
    /// Hacks the object to turn its allegiance
    /// </summary>
    public abstract void Hack();
    /// <summary>
    /// Turns the object on or off
    /// </summary>
    public abstract void TurnOnOff();
}
