using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterBase", menuName = "Scriptable Objects / Character Base", order = 3)]
public class CharacterBaseScriptable : ScriptableObject
{
    public CharacterBaseType CharacterType;
    public AnimatorOverrideController CharacterAOC;
    public int ScoreToUnlock;
    
    [Header("Base Stats")]
    public float Health;
    public float HealthModifier;
    public float Stamina;
    public float MovementSpeed;
    public float Accuracy;
    public float Defense;
    public int Strength;
    public float Hacking;
    public float LimbToughness;

    [Header("Stats list")]
    public List<BaseStat> BaseStats = new List<BaseStat>();
}

[Serializable]
public class BaseStat
{
    public StatType Type;
    public float Value;
}