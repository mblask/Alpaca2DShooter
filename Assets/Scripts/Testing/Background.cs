using UnityEngine;

public class Background : MonoBehaviour
{
    private Transform _cameraTransform;
    private float _backgroundZ;
    private float _paralaxFactor = 0.9f;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
        _backgroundZ = transform.position.z;
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = _cameraTransform.position * _paralaxFactor;
        transform.position = new Vector3(cameraPosition.x, cameraPosition.y, _backgroundZ);
    }
}
