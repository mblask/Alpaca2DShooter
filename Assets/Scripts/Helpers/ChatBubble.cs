using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatBubble : MonoBehaviour
{
    //public TextMeshPro TextMesh;
    private TextMeshPro _chatText;

    private Transform _targetToFollow = null;
    private Vector3 _offset = Vector3.zero;

    private bool _destructionActivated = false;
    private float _destructionTime = 1.5f;

    private ChatBubbleSpawner _chatBubbleSpawner;

    private void Awake()
    {
        _chatText = transform.Find("ChatText").GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        _chatBubbleSpawner = ChatBubbleSpawner.Instance;
    }

    private void LateUpdate()
    {
        if (_destructionActivated)
        {
            _destructionTime -= Time.deltaTime;

            if (_destructionTime <= 0.0f)
                _chatBubbleSpawner.RemoveBubble(this);
        }

        followTarget();
    }

    public void SetTargetToFollow(Transform target)
    {
        if (target == null)
            return;

        _targetToFollow = target;
    }

    public void SetOffset(Vector3 offset)
    {
        if (offset == null)
            return;

        _offset = offset;
    }

    private void followTarget()
    {
        if (_targetToFollow == null)
            return;

        transform.position = _targetToFollow.position + _offset;
    }

    public void SetupText(string textToDisplay)
    {
        if (_chatText == null)
            return;

        _chatText.SetText(textToDisplay);

        activateDestructionAfter(true, _destructionTime);
    }

    public Transform GetTargetToFollow()
    {
        return _targetToFollow;
    }

    private void activateDestructionAfter(bool value, float destructionTime)
    {
        _destructionActivated = value;
        _destructionTime = destructionTime;
    }
}
