using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float RotationSpeed;

    private void Update()
    {
        rotate2DBody();
    }

    private void rotate2DBody()
    {
        if (RotationSpeed == 0.0f)
            return;

        Vector3 rotationIncrement = Vector3.forward * RotationSpeed * Mathf.Rad2Deg * Time.deltaTime;
        Vector3 finalRotation = transform.rotation.eulerAngles + rotationIncrement;
        Quaternion q = Quaternion.Euler(finalRotation);
        transform.rotation = q;
    }
}
