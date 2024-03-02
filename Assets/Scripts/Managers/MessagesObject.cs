using System.Collections.Generic;
using UnityEngine;

public class MessagesObject : MonoBehaviour
{
    private const string MESSAGE_OBJECT_STRING = "MessageObject: ";

    [SerializeField] private bool _displayMessages = true;
    [SerializeField] [TextArea] private List<string> _urgent;
    [SerializeField] [TextArea] private List<string> _nonUrgent;

    private void Start()
    {
        if (!_displayMessages)
            return;

        if (_urgent.Count != 0)
            foreach (string message in _urgent)
            {
                Debug.LogError(MESSAGE_OBJECT_STRING + message);
            }


        if (_nonUrgent.Count != 0)
            foreach (string message in _nonUrgent)
            {
                Debug.Log(MESSAGE_OBJECT_STRING + message);
            }
    }
}
