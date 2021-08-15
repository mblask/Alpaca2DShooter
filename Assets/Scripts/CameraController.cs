using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance = null;

    public static CameraController Instance
    {
        get
        {
            return _instance;
        }
    }

    public Transform TargetToFollow;
    public float Smoothing;    

    #region Singleton
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }
    #endregion

    private void LateUpdate()
    {
        SmoothFollow();
    }

    private void SmoothFollow()
    {
        if (TargetToFollow == null)
            return;

        Vector3 targetPosition = new Vector3();

        targetPosition[0] = TargetToFollow.position.x;
        targetPosition[1] = TargetToFollow.position.y;
        targetPosition[2] = gameObject.transform.position.z;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Smoothing * Time.deltaTime);
    }

    public IEnumerator CameraShaker(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float x = Random.Range(-1.0f, 1.0f) * magnitude;
            float y = Random.Range(-1.0f, 1.0f) * magnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
    }
}
