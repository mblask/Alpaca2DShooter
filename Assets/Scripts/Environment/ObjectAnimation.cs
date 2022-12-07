using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAnimation : MonoBehaviour, IDisturbable
{
    private Animator _animator;

    private string _disturbTriggerName = "TriggerDisturbed";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void DisturbAnimation()
    {
        _animator.SetTrigger(_disturbTriggerName);
    }
}
