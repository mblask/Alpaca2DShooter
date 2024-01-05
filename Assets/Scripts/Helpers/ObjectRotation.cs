using UnityEngine;

public class ObjectRotation
{
    private Transform _transform;
    public float RotationSpeed { get; set; }

    public ObjectRotation(Transform transform, float rotationSpeed)
    {
        _transform = transform;
        RotationSpeed = rotationSpeed;
    }

    public void Rotate2dBody()
    {
        Vector3 rotationIncrement = Vector3.forward * RotationSpeed * Mathf.Rad2Deg * Time.deltaTime;
        Vector3 finalRotation = _transform.rotation.eulerAngles + rotationIncrement;
        Quaternion q = Quaternion.Euler(finalRotation);
        _transform.rotation = q;
    }
}