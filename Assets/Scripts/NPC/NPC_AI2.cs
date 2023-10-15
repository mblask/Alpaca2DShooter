using AlpacaMyGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum NpcState
{
    Idle,
    Patrol,
    Chase,
    Blinded
}

public class NPC_AI2 : MonoBehaviour, IBlindable
{
    [SerializeField] private NPCAllegiance _npcAllegiance;
    private NpcState _state = NpcState.Patrol;
    private Vector2 _originalPosition;
    private Vector2 _waypoint;

    private float _idleTimer = 0.0f;

    private const float MIN_IDLE_TIME = 1.0f;
    private const float MAX_IDLE_TIME = 3.0f;
    private const float DEFAULT_PATROL_RADIUS = 3.0f;
    private const float STOP_DISTANCE = 0.5f;

    private bool _isBlinded = false;

    private float _attackDistance = 0.0f;
    private float _viewDistance = 0.0f;
    private float _stopFollowingDistance = 0.0f;
    private float _patrolRadius = 0.0f;
    private float _maxIdleTime = 0.0f;

    private const float DEFAULT_ATTACK_DISTANCE = 6.0f;
    private const float DEFAULT_VIEW_DISTANCE = 8.0f;
    private const float DEFAULT_STOP_FOLLOWING_DISTANCE = 9.0f;

    private NPCStats _npcStats;
    private NPCWeapons _npcWeapons;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _npcStats = GetComponent<NPCStats>();
        _npcWeapons = GetComponent<NPCWeapons>();

        _attackDistance = DEFAULT_ATTACK_DISTANCE;
        _viewDistance = DEFAULT_VIEW_DISTANCE;
        _stopFollowingDistance = DEFAULT_STOP_FOLLOWING_DISTANCE;
        _patrolRadius = DEFAULT_PATROL_RADIUS;
        _maxIdleTime = MAX_IDLE_TIME;
    }

    private void Start()
    {
        _playerStats = PlayerStats.Instance;
        _originalPosition = transform.position;
        _waypoint = transform.position;
    }

    public void Update()
    {
        if (!_playerStats.IsAlive())
            return;

        switch (_state)
        {
            case NpcState.Idle:
                idleState();
                break;
            case NpcState.Patrol:
                patrolState();
                break;
            case NpcState.Blinded:
                blindedState();
                break;
            case NpcState.Chase:
                chaseState();
                break;
        }

        normalizeAwareness();
    }

    private void idleState()
    {
        _idleTimer -= Time.deltaTime;

        if (_idleTimer <= 0.0f)
        {
            _idleTimer = UnityEngine.Random.Range(MIN_IDLE_TIME, MAX_IDLE_TIME);
            _state = NpcState.Patrol;
            getNextWaypoint();
        }
    }

    private void patrolState()
    {
        moveTo(_waypoint);
        rotateTowards(_waypoint);

        if (_npcAllegiance.Equals(NPCAllegiance.Ally))
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerStats.transform.position);
        if (distanceToPlayer < _viewDistance)
        {
            if (ObstaclesInTheWayRaycast(_playerStats.transform.position))
                return;

            _state = NpcState.Chase;
            _npcWeapons.PresentWeapon(true);
            rotateTowards(_playerStats.transform.position);
        }
    }

    private void blindedState()
    {
        moveTo(_waypoint);
        rotateTowards(_waypoint);

        if (Mathf.Abs(_viewDistance - DEFAULT_VIEW_DISTANCE * 0.75f) < 0.0f)
            _isBlinded = false;

        if (!_isBlinded)
            _state = NpcState.Patrol;
    }

    private void chaseState()
    {
        rotateTowards(_playerStats.transform.position);

        float distanceToPlayer = Vector2.Distance(transform.position, _playerStats.transform.position);
        if (distanceToPlayer > _stopFollowingDistance)
        {
            //go to idle
            _npcWeapons.StopAttack();
            _npcWeapons.PresentWeapon(false);
            _state = NpcState.Idle;
        }

        if (distanceToPlayer <= _stopFollowingDistance && distanceToPlayer > _attackDistance)
        {
            //follow player
            moveTo(_playerStats.transform.position);
        }
        
        if (distanceToPlayer <= _attackDistance)
        {
            //attack
            _npcWeapons.AttackTarget(_playerStats.transform);
        }
    }

    private void normalizeAwareness()
    {
        float toleranceMagnitude = 0.1f;
        float normalizationFactor = 0.05f;

        float sign = Mathf.Sign(_viewDistance - DEFAULT_VIEW_DISTANCE);

        if (Mathf.Abs(_viewDistance - DEFAULT_VIEW_DISTANCE) > toleranceMagnitude)
            _viewDistance -= sign * normalizationFactor * DEFAULT_VIEW_DISTANCE * Time.deltaTime;

        if (Mathf.Abs(_attackDistance - DEFAULT_ATTACK_DISTANCE) > toleranceMagnitude)
            _attackDistance -= sign * normalizationFactor * DEFAULT_ATTACK_DISTANCE * Time.deltaTime;

        if (Mathf.Abs(_stopFollowingDistance - DEFAULT_STOP_FOLLOWING_DISTANCE) > toleranceMagnitude)
            _stopFollowingDistance -= sign * normalizationFactor * DEFAULT_STOP_FOLLOWING_DISTANCE * Time.deltaTime;

        if (Mathf.Abs(_patrolRadius - DEFAULT_PATROL_RADIUS) > toleranceMagnitude)
            _patrolRadius -= sign * normalizationFactor * DEFAULT_PATROL_RADIUS * Time.deltaTime;

        if (Mathf.Abs(_maxIdleTime - MAX_IDLE_TIME) > toleranceMagnitude)
            _maxIdleTime -= sign * normalizationFactor * MAX_IDLE_TIME * Time.deltaTime;
    }

    public void Blind()
    {
        float blindingFactor = 0.1f;

        _isBlinded = true;
        _state = NpcState.Blinded;
        _npcWeapons.StopAttack();

        _viewDistance = DEFAULT_VIEW_DISTANCE * blindingFactor;
        _attackDistance = DEFAULT_ATTACK_DISTANCE * blindingFactor;
        _stopFollowingDistance = DEFAULT_STOP_FOLLOWING_DISTANCE * blindingFactor;
        _patrolRadius = DEFAULT_PATROL_RADIUS * blindingFactor;
        _maxIdleTime = MAX_IDLE_TIME * blindingFactor;
    }

    public void ExtendAwareness()
    {
        if (_isBlinded)
            return;

        float factor = 1.7f;
        _viewDistance = DEFAULT_VIEW_DISTANCE * factor;
        _attackDistance = DEFAULT_ATTACK_DISTANCE * factor;
        _stopFollowingDistance = DEFAULT_STOP_FOLLOWING_DISTANCE * factor;
    }

    public void AlertNPC()
    {
        ExtendAwareness();

        if (_npcWeapons.HasShootingTarget)
            return;

        _waypoint = _playerStats.transform.position;

        if (Utilities.ChanceFunc(50))
            npcAlerted();
    }

    private void npcAlerted()
    {
        string randomAlert = Constants.NPC_ALERT_MESSAGES.GetRandomElement();
        FloatingTextSpawner
            .CreateFloatingTextStatic(transform.position, randomAlert, Color.red, destroyAfter: 0.8f, fontSize: 4, floatSpeed: 0.5f);
    }

    private void getNextWaypoint()
    {
        Vector2 positionIncrement = Utilities.GetRandomVector2(_patrolRadius);
        _waypoint = _originalPosition + positionIncrement;

        if (ObstaclesInTheWayRaycast(_waypoint))
        {
            Vector2 direction = _waypoint - (Vector2)transform.position;
            _waypoint += direction.normalized.InvertVector();
        }
    }

    public bool ObstaclesInTheWayRaycast(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        List<RaycastHit2D> hits = 
            Physics2D.RaycastAll(transform.position, direction.normalized, direction.magnitude).ToList();

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<TilemapCollider2D>() != null)
                return true;

            Door door = hit.transform.GetComponent<Door>();
            if (door != null && door.IsClosed())
                return true;

            if (hit.transform.GetComponent<NPCBase>() != null && hit.transform != transform)
                return true;
        }

        return false;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _viewDistance);
    }
}
