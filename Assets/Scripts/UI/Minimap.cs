using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private Transform _minimapContainer;

    private void Awake()
    {
        _minimapContainer = transform.Find("Container");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool minimapActive = _minimapContainer.gameObject.activeSelf;
            _minimapContainer.gameObject.SetActive(!minimapActive);
        }
    }
}
