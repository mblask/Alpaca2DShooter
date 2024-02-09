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
        AudioManager.Instance.PlayClip(SFXClip.BushRattle);
        _animator.SetTrigger(_disturbTriggerName);
    }
}
