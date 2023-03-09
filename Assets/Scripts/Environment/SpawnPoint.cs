using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class SpawnPoint : MonoBehaviour
{
    private CircleCollider2D _collider;
    
    [Header("Read-only")]
    [SerializeField] private bool _isActive = true;

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.isTrigger = true;
    }

    public Vector3 Location
    {
        get
        {
            return transform.position;
        }
    }

    public bool IsActive()
    {
        return _isActive;
    }

    public void SetActive(bool value)
    {
        _isActive = value;
    }
}
