using UnityEngine;

[CreateAssetMenu(fileName = "BossBase", menuName = "Scriptable Objects / Boss Base", order = 4)]
public class BossScriptable : CharacterBaseScriptable
{
    [Header("Boss settings")]
    public string Name;
    public int Level;
    public WeaponItem WeaponOfChoice;
}
