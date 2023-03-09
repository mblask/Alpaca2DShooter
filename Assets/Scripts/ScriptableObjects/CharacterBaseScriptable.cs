using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBase", menuName = "Scriptable Objects / Character Base", order = 3)]
public class CharacterBaseScriptable : ScriptableObject
{
    public CharacterBaseType CharacterType;
    public AnimatorOverrideController CharacterAOC;
    
    [Header("Base Stats")]
    public float Health;
    public float HealthModifier;
    public float Stamina;
    public float MovementSpeed;
    public float Accuracy;
    public float Defense;
    public int Strength;
}
