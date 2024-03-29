using TMPro;
using UnityEngine;

public class InstanceCompleteUI : MonoBehaviour
{
    private const string IS_ACTIVE_BOOLEAN = "IsActive";
    private Animator _animator;
    private bool _isActive = false;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _text = transform.Find("Container").Find("Text").GetComponent<TextMeshProUGUI>();
        _animator.SetBool(IS_ACTIVE_BOOLEAN, _isActive);
    }

    public void SetText(string text)
    {
        _text.SetText(text);
    }

    public void ActivateUI(bool value)
    {
        _isActive = value;
        _animator.SetBool(IS_ACTIVE_BOOLEAN, value);
    }

    public void ToogleUI()
    {
        _isActive = !_isActive;
        _animator.SetBool(IS_ACTIVE_BOOLEAN, _isActive);
    }
}
