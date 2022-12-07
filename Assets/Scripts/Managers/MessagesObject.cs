using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagesObject : MonoBehaviour
{
    [SerializeField] [TextArea] private List<string> _urgent;
    [SerializeField] [TextArea] private List<string> _nonUrgent;

    private void Start()
    {
        if (_urgent.Count != 0)
            foreach (string message in _urgent)
            {
                Debug.LogError(message);
            }


        if (_nonUrgent.Count != 0)
            foreach (string message in _nonUrgent)
            {
                Debug.Log(message);
            }
    }
}
