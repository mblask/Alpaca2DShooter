using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Conversation
{
    public string Topic;
    public Speech Speech;
    public bool IsMission;
    private bool _complete = false;

    public void SetComplete(bool value)
    {
        _complete = value;
    }

    public bool IsComplete()
    {
        return _complete;
    }
}
