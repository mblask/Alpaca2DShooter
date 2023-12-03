using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects / Skill", order = 6)]
public class SkillSO : ScriptableObject
{
    public string Name;
    public StatType Stat;
    public float Modifier;
    public float Multiplier;
}
