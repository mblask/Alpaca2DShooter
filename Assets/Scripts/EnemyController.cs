using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float EnemySpeed = 1.0f;
    public float ViewDistance = 3.0f;
    public float FollowDistance = 5.0f;
    public List<Transform> Waypoints = new List<Transform>();
    public Transform MoveToTarget;
    public float RotationSmoothing = 0.1f;

    private Rigidbody2D _rigidBody;
    private Transform _nextWaypoint = null;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
    }

    private void moveEnemyTo(Transform target)
    {
        if (target == null)
            return;

        rotateEnemyTowards(target);

        Vector2 direction = (target.position - transform.position).normalized;

        float distance = (target.position - transform.position).magnitude;

        Vector2 finalDestination = transform.position + EnemySpeed * (Vector3)direction * Time.deltaTime;

        _rigidBody.MovePosition(finalDestination);
    }

    private void patrol(Transform waypoint1, Transform waypoint2)
    {
        Debug.Log("Patrol");       
    }

    private Transform generateWaypoint()
    {
        Transform newWaypoint = null;
        Vector3 waypointDisplacement = new Vector2();

        waypointDisplacement.x = Random.Range(-1.0f, 1.0f) * ViewDistance;
        waypointDisplacement.y = Random.Range(-1.0f, 1.0f) * ViewDistance;
        waypointDisplacement.z = transform.position.z;

        newWaypoint.position = transform.position + waypointDisplacement;

        return newWaypoint;
    }

    private void locatePlayer()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, ViewDistance);

        if (hit.Length != 0)
        {
            foreach (Collider2D collider in hit)
            {
                if (collider.gameObject.CompareTag(ConstsEnums.PlayerTag))
                {
                    Debug.Log("Player found");
                    rotateEnemyTowards(collider.transform);
                }
            }
        }
    }

    private void rotateEnemyTowards(Transform target)
    {
        if (target == null)
            return;

        Quaternion q = new Quaternion();

        Vector2 direction = (target.position - transform.position).normalized;

        if (direction.x >= 0)
            q = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        else
        {
            q = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            direction.x = -1.0f * direction.x;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, q * Quaternion.AngleAxis(angle, Vector3.forward), RotationSmoothing);
    }
}
