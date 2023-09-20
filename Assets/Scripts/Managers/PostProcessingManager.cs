using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : MonoBehaviour
{
    private static PostProcessingManager _instance;
    
    public static PostProcessingManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private Volume _volume;

    [SerializeField] private float _reductionSpeed = 0.02f;

    private void Awake()
    {
        _instance = this;
        _volume = GetComponent<Volume>();
    }

    private void LateUpdate()
    {
        reduceVolumeWeight();
    }

    private void reduceVolumeWeight()
    {
        if (_volume.weight > 0.0f)
            _volume.weight -= Time.deltaTime * _reductionSpeed;
    }

    public void ActivatePostProcessing(float duration = 0.0f)
    {
        _volume.weight = 1.0f;

        if (duration != 0.0f)
            _reductionSpeed = 0.2f / duration;
    }

    public void ResetPostProcessing()
    {
        _volume.weight = 0.0f;
    }
}
