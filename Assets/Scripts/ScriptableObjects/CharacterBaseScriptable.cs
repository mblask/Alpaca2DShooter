using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBase", menuName = "Scriptable Objects / Character Base", order = 3)]
public class CharacterBaseScriptable : ScriptableObject
{
    public CharacterBaseType CharacterType;
    public AnimatorOverrideController CharacterAOC;
    public float MovementSpeed;
    public float Accuracy;
    public float HealthModifier;
    public int Strength;
}
