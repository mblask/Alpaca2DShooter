using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private List<KeyCode> _upKeyCode = new List<KeyCode>();
    [SerializeField] private List<KeyCode> _downKeyCode = new List<KeyCode>();
    [SerializeField] private List<KeyCode> _rightKeyCode = new List<KeyCode>();
    [SerializeField] private List<KeyCode> _leftKeyCode = new List<KeyCode>();
    [SerializeField] private List<KeyCode> _sprintKeyCode = new List<KeyCode>();
    [SerializeField] private List<KeyCode> _reloadKeyCode = new List<KeyCode>();

    void Update()
    {
        if (getOneKey(_upKeyCode))
            Debug.Log("Up");

        if (getOneKey(_downKeyCode))
            Debug.Log("Down");

        if (getOneKey(_rightKeyCode))
            Debug.Log("Right");

        if (getOneKey(_leftKeyCode))
            Debug.Log("Left");

        if (getOneKey(_sprintKeyCode))
            Debug.Log("Sprint");

        if (getOneKey(_reloadKeyCode))
            Debug.Log("Reload");
    }

    private bool getOneKey(List<KeyCode> keyCodes)
    {
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyDown(keyCode))
                return true;
        }

        return false;
    }
}
