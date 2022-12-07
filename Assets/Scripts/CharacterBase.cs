using UnityEngine;

[System.Serializable]
public class CharacterBase
{
    public CharacterBaseType CharacterType;
    public AnimatorOverrideController CharacterAOC;
    public float MovementSpeed;
    public float Accuracy;
    public float HealthModifier;
}
