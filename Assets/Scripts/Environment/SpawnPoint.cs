using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType
{
    NPC,
    Artefact,
}

[RequireComponent(typeof(CircleCollider2D))]
public class SpawnPoint : MonoBehaviour
{
    private CircleCollider2D _collider;

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
}
