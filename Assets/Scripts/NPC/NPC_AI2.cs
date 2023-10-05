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
        }
    }

    private void patrolState()
    {
        moveTo(_waypoint, () => _state = NpcState.Idle);
    }

    private void chaseState()
    {

    }

    private void getNextWaypoint()
    {
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
            getNextWaypoint();
        }
    }
}
