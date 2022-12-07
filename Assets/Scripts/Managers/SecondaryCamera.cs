using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryCamera : MonoBehaviour
{
    private PlayerController _playerController;

    private float _cameraZ;

    private void Start()
    {
        _playerController = PlayerController.Instance;
        _cameraZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (_playerController != null)
        {
            Vector3 position = new Vector3(_playerController.transform.position.x, _playerController.transform.position.y, _cameraZ);
            transform.position = position;
        }
    }
}
