using AlpacaMyGames;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NpcAi : MonoBehaviour, IBlindable
{
    [SerializeField] private NPCAllegiance _npcAllegiance;
    private NpcState _state = NpcState.Patrol;
    private Vector2 _originalPosition;
    private Vector2 _waypoint;

    private FloatingTextSingle _alertedText;

    private float _idleTimer = 0.0f;

    private const float MIN_IDLE_TIME = 1.0f;
    private const float MAX_IDLE_TIME = 3.0f;
    private const float DEFAULT_PATROL_RADIUS = 3.0f;
    private const float STOP_DISTANCE = 0.5f;

    private bool _isBlinded = false;
    private float _defaultBlindnessTime = 5.0f;
    private TimerObject _blindnessTimer;

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

    private Pathfinding _pathfinding;
    private bool _pathFound = false;
    private List<Vector2> _walkPoints;

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
        _pathfinding = new Pathfinding(15, 15, 0.75f, transform.position);
        _walkPoints = new List<Vector2>();
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

    public NPCAllegiance GetAllegiance()
    {
        return _npcAllegiance;
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
        moveTo(_waypoint, () => _state = NpcState.Idle);
        rotateTowards(_waypoint);

        if (_npcAllegiance.Equals(NPCAllegiance.Ally))
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerStats.transform.position);
        if (distanceToPlayer < _viewDistance)
        {
            if (ObstaclesInRaycast(_playerStats.transform.position))
                return;

            _state = NpcState.Chase;
            _npcWeapons.PresentWeapon(true);
            rotateTowards(_playerStats.transform.position);
        }
    }

    private void blindedState()
    {
        if (!_isBlinded)
            return;

        moveTo(_waypoint);
        rotateTowards(_waypoint);

        if (_blindnessTimer.Update())
        {
            _isBlinded = false;
            _state = NpcState.Patrol;
        }

        _viewDistance = _blindnessTimer.Timer / _blindnessTimer.Duration * DEFAULT_VIEW_DISTANCE;
    }

    private void chaseState()
    {
        if (_playerStats == null)
            _playerStats = PlayerStats.Instance;

        rotateTowards(_playerStats.transform.position);

        float distanceToPlayer = Vector2.Distance(transform.position, _playerStats.transform.position);
        if (distanceToPlayer > _stopFollowingDistance)
        {
            //go to idle
            _npcWeapons.StopAttack();
            _npcWeapons.PresentWeapon(false);
            _state = NpcState.Idle;

            if (_pathFound)
            {
                _pathFound = false;
                _walkPoints = new List<Vector2>();
            }
        }

        if (distanceToPlayer <= _stopFollowingDistance && distanceToPlayer > _attackDistance)
        {
            //follow player
            if (!_pathFound)
            {
                Vector3 directionVector = _playerStats.transform.position - transform.position;
                _pathfinding.Find(transform.position, _playerStats.transform.position);
                _walkPoints = _pathfinding.GetWorldPoints();

                _pathFound = _walkPoints.Count > 0;
            }

            moveThroughWaypoints(() => { _pathFound = false; });
        }

        if (distanceToPlayer <= _attackDistance)
        {
            //attack
            if (_pathFound)
            {
                _pathFound = false;
                _walkPoints = new List<Vector2>();
            }

            if (!ObstaclesInRaycast(_playerStats.transform.position))
                _npcWeapons.AttackTarget(_playerStats.transform);
            else
                _npcWeapons.StopAttack();
        }
    }

    private void normalizeAwareness()
    {
        float toleranceMagnitude = 0.1f;
        float normalizationFactor = 0.05f;

        float sign = Mathf.Sign(_viewDistance - DEFAULT_VIEW_DISTANCE);

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
        _blindnessTimer = new TimerObject(_defaultBlindnessTime);
        _npcStats.TemporaryModifyStat(StatType.Defense, -15.0f, 1.0f, _defaultBlindnessTime);
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

        if (!ObstaclesInRaycast(_playerStats.transform.position))
            _waypoint = _playerStats.transform.position;

        if (Utilities.ChanceFunc(50))
            npcAlerted();
    }

    private void npcAlerted()
    {
        string randomAlert = Constants.NPC_ALERT_MESSAGES.GetRandomElement();
        
        if (_alertedText == null)
            FloatingTextSpawner.CreateFloatingTextStatic
                (transform.position, randomAlert, Color.red, destroyAfter: 0.8f, fontSize: 8, floatSpeed: 0.5f);
    }

    private void getNextWaypoint()
    {
        Vector2 positionIncrement = Utilities.GetRandomVector2(_patrolRadius);
        _waypoint = _originalPosition + positionIncrement;

        if (ObstaclesInRaycast(_waypoint))
        {
            Vector2 direction = _waypoint - (Vector2)transform.position;
            _waypoint += direction.normalized.InvertVector();
        }
    }

    public bool ObstaclesInRaycast(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;
        List<RaycastHit2D> hits =
            Physics2D.RaycastAll(transform.position, direction.normalized, direction.magnitude).ToList();

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

            if (hit.transform.GetComponent<TilemapCollider2D>() != null)
                return true;

            if (hit.transform.GetComponent<BaseCollider>() != null)
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
        }
    }

    private void moveThroughWaypoints(Action onArrival = null)
    {
        if (_walkPoints.Count == 0)
            return;

        Vector3 walkPoint = _walkPoints[0];
        Vector3 direction = walkPoint - transform.position;
        direction.Normalize();

        Vector3 positionIncrement = _npcStats.EnemySpeed.GetCurrentValue() * direction * Time.deltaTime;
        transform.position += positionIncrement;

        if (Vector3.Distance(transform.position, _walkPoints[0]) < STOP_DISTANCE)
        {
            _walkPoints.RemoveAt(0);

            if (_walkPoints.Count == 0)
            {
                _walkPoints.Clear();
                onArrival?.Invoke();
            }
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
