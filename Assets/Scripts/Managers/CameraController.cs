using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("Camera Follow")]
    private Transform _targetToFollow;
    [SerializeField] [Range(0.1f, 6.0f)] private float _smoothIntensity = 4.0f;

    [Header("Camera Wobble")]
    [SerializeField] [Tooltip("Camera wobble deactivates smooth follow")] private bool _cameraWobble;
    [SerializeField] [Range(0.0f, 1.0f)]private float _wobbleWeight = 1.0f;
    [SerializeField] private float _wobbleWeightReductionSpeed = 0.1f;
    private float _yAmplitude;
    private float _yPhaseSpeed;
    private float _xAmplitude;
    private float _xPhaseSpeed;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _targetToFollow = PlayerController.Instance?.transform;

        if (_cameraWobble)
            randomizeWobbleParameters();
    }

    private void LateUpdate()
    {
        smoothFollow();
    }

    private void smoothFollow()
    {
        if (_targetToFollow == null)
            return;

        Vector3 targetPosition = new Vector3();

        targetPosition[0] = _targetToFollow.position.x;
        targetPosition[1] = _targetToFollow.position.y;
        targetPosition[2] = transform.position.z;

        if (_cameraWobble)
        {
            Vector2 wobblePosition = _wobbleWeight * cameraWobble();
            Vector3 newTransformPosition = targetPosition + new Vector3(wobblePosition.x, wobblePosition.y, 0.0f);
            transform.position = newTransformPosition;

            _wobbleWeight -= Time.deltaTime * _wobbleWeightReductionSpeed;
            if (_wobbleWeight <= 0.0f)
            {
                _wobbleWeight = 0.0f;
                _cameraWobble = false;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothIntensity * Time.deltaTime);
        }
    }

    private Vector2 cameraWobble()
    {
        Vector2 position = _yAmplitude * Vector2.up * Mathf.Sin(Time.time * _yPhaseSpeed) + _xAmplitude * Vector2.right * Mathf.Cos(Time.time * _xPhaseSpeed) + _yAmplitude * Vector2.left * Mathf.Sin(Time.time * _xPhaseSpeed) * Mathf.Cos(Time.time * _xPhaseSpeed);
        
        return position;
    }

    private void randomizeWobbleParameters()
    {
        float min = 1.5f;
        float max = 2.5f;

        _xAmplitude = Random.Range(min, max);
        _xPhaseSpeed = Random.Range(min, max);
        _yAmplitude = Random.Range(min, max);
        _yPhaseSpeed = Random.Range(min, max);
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(CameraShaker(duration, magnitude));
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

    public void WobbleCamera(bool value, float duration = 0.0f)
    {
        _cameraWobble = value;

        if (!value)
            return;

        if (duration != 0.0f)
            _wobbleWeightReductionSpeed = 1 / duration;

        _wobbleWeight = 1.0f;
        randomizeWobbleParameters();
    }

    public void StopCameraWobble()
    {
        _cameraWobble = false;
    }
}
