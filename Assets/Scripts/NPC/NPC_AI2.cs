using AlpacaMyGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC_AI2 : MonoBehaviour
{
    public enum NpcState
    {
        Idle,
        Patrol,
        Chase
    }

    private NpcState _state = NpcState.Patrol;
    private Vector2 _originalPosition;
    private Vector2 _waypoint;

    private float _idleTime = 0.0f;
    private float _idleTimer = 0.0f;

    private const float MAX_IDLE_TIME = 3.0f;
    private const float PATROL_RADIUS = 3.0f;
    private const float STOP_DISTANCE = 0.1f;

    private float _attackDistance = 0.0f;
    private float _viewDistance = 0.0f;
    private float _stopFollowingDistance = 0.0f;

    private const float DEFAULT_ATTACK_DISTANCE = 6.0f;
    private const float DEFAULT_VIEW_DISTANCE = 8.0f;
    private const float DEFAULT_STOP_FOLLOWING_DISTANCE = 9.0f;

    private NPCStats _npcStats;

    private void Awake()
    {
        _npcStats = GetComponent<NPCStats>();
    }

    private void Start()
    {
        _originalPosition = transform.position;
        getNextWaypoint();
    }

    public void Update()
    {
        switch (_state)
        {
            case NpcState.Idle:
                idleState();
                break;
            case NpcState.Patrol:
                patrolState();
                break;
            case NpcState.Chase:
                chaseState();
                break;
        }
    }

    private void idleState()
    {
        _idleTimer -= Time.deltaTime;

        if (_idleTimer <= 0.0f)
        {
            _idleTimer = UnityEngine.Random.Range(0.0f, MAX_IDLE_TIME);
            _state = NpcState.Patrol;
            getNextWaypoint();
        }
    }

    private void patrolState()
    {
        moveTo(_waypoint);
        rotateTowards(_waypoint);
    }

    private void chaseState()
    {

    }

    private void getNextWaypoint()
    {
        if (obstaclesInTheWay(transform.position + Vector3.up * PATROL_RADIUS))
        {
            Debug.Log("Obstacle to the top");
        }

        if (obstaclesInTheWay(transform.position - Vector3.up * PATROL_RADIUS))
        {
            Debug.Log("Obstacle to the bottom");
        }

        if (obstaclesInTheWay(transform.position + Vector3.right * PATROL_RADIUS))
        {
            Debug.Log("Obstacle to the right");
        }

        if (obstaclesInTheWay(transform.position - Vector3.right * PATROL_RADIUS))
        {
            Debug.Log("Obstacle to the left");
        }

        Vector2 positionIncrement = Utilities.GetRandomVector2(PATROL_RADIUS);
        _waypoint = _originalPosition + positionIncrement;
    }

    private bool obstaclesInTheWay(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        Ray ray = new Ray(transform.position, direction.normalized);

        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, direction.magnitude);
        return hits.Length > 0;
    }

    private void moveTo(Vector2 position, Action onArrival = null)
    {
        Vector3 direction = position - (Vector2)transform.position;
        direction.Normalize();

        Vector3 positionIncrement = 
            _npcStats.EnemySpeed.GetCurrentValue() * direction * Time.deltaTime;
        transform.position += positionIncrement;

        if (Vector2.Distance((Vector2)transform.position, position) < STOP_DISTANCE)
        {
            onArrival?.Invoke();
            _state = NpcState.Idle;
        }
    }

    private void rotateTowards(Vector3 target)
    {
        if (target == null)
            return;

        Vector2 direction = (target - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, 0.3f);
    }
}
