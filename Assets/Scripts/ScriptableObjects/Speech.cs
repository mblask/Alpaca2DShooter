using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation List", menuName = "Scriptable Objects / Conversation", order = 10)]
public class Speech : ScriptableObject
{
    [TextArea] public List<string> SpeechList;
}
