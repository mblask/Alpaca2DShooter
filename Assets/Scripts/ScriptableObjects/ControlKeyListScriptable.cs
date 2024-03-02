using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Controls", menuName = "Scriptable Objects / Controls List", order = 7)]
public class ControlKeyListScriptable : ScriptableObject
{
    public List<ControlKey> controls = new List<ControlKey>();
}
