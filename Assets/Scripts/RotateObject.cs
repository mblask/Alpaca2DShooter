using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 Axis;
    public float RotationSpeed;

    private void Update()
    {
        Rotate2DBody();
    }

    private void Rotate2DBody()
    {
        if (Axis == Vector3.zero)
            return;

        if (RotationSpeed == 0.0f)
            return;

        Vector3 rotationIncrement = Axis * RotationSpeed * Mathf.Rad2Deg * Time.deltaTime;
        Vector3 finalRotation = transform.rotation.eulerAngles + rotationIncrement;
        Quaternion q = Quaternion.Euler(finalRotation);
        transform.rotation = q;
    }
}
