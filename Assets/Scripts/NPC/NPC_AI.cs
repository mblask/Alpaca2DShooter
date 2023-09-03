using UnityEngine;
using System;
using AlpacaMyGames;

public class NPC_AI : MonoBehaviour
{
    public Vector2 PatrollingDistance = new Vector2();

    [Header("Read-only")]
    [SerializeField] private float _initialAttackDistance = 6.0f;
    [SerializeField] private float _initialViewDistance = 8.0f;
    [SerializeField] private float _initialStopFollowingDistance = 9.0f;

    private float _attackDistance;
    private float _viewDistance;
    private float _stopFollowingDistance;

    private NPCState _state;
    [Header("NPC Affiliation")]
    [SerializeField] private NPCType _type;

    private bool _isMoving = false;
    private Vector2 _patrolWaitTimeInterval = new Vector2(0.5f, 2.5f);
    private float _patrolWaitTimer = 0.0f;
    private float _stopDistance = 1.25f;
    private bool _shouldChase = true;
    private bool _runStateMachine = true;

    private float _interactionRadius = 5.0f;
    private bool _isInteracting = false;

    private Rigidbody2D _rigidBody;
    private NPCWeapons _NPCWeapons;
    private NPCStats _NPCStats;
    private Vector3 _originPosition;
    private Vector3 _targetPosition;

    private PlayerController _playerController;
    private PlayerStats _playerStats;
    private GameManager _gameManager;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _NPCWeapons = GetComponent<NPCWeapons>();
        _NPCStats = GetComponent<NPCStats>();
    }

    private void Start()
    {
        _playerController = PlayerController.Instance;
        _playerStats = PlayerStats.Instance;
        _gameManager = GameManager.Instance;
        
        _originPosition = transform.position;
        _targetPosition = generatePatrollingPosition();

        UpdateDistancesAccordingToLighting();

        _state = NPCState.Idle;
    }

    private void resetInitialDistances()
    {
        _attackDistance = _initialAttackDistance;
        _viewDistance = _initialViewDistance;
        _stopFollowingDistance = _initialStopFollowingDistance;
    }

    private void Update()
    {
        if (_gameManager != null)
            return;

        if (!_gameManager.IsGameRunning())
        {
            _NPCWeapons.StopAttack();
            return;
        }

        if (_isInteracting && Vector2.Distance(transform.position, _playerController.transform.position) > _interactionRadius)
            stopInteractWithPlayer();
    }

    private void FixedUpdate()
    {
        stateMachineProcedure();
        if (!_runStateMachine)
            rotateEnemyTowards(_targetPosition);
    }

    private void LateUpdate()
    {
        resizeViewAndAttackDistance();
    }

    public NPCType GetNPCType()
    {
        return _type;
    }

    public void UpdateDistancesAccordingToLighting()
    {
        resetInitialDistances();

        Vector2 factorInterval = new Vector2(0.6f, 1.0f);
        float factor = factorInterval.x + 
            MainLightingManager.GlobalIntensity / MainLightingManager.MaxIntensity * (factorInterval.y - factorInterval.x);

        _attackDistance *= factor;
        _viewDistance *= factor;
        _stopFollowingDistance *= factor;
    }

    private void npcSaysRandomStuff()
    {
        if (!Utilities.ChanceFunc(33))
            return;

        string randomMessage = Constants.NPC_RANDOM_MESSAGES.GetRandomElement();
        FloatingTextSpawner
            .CreateFloatingTextStatic(transform.position, randomMessage, Color.white, destroyAfter: 0.8f, fontSize: 4, floatSpeed: 0.5f);
    }

    private void npcAlerted()
    {
        string randomAlert = Constants.NPC_ALERT_MESSAGES.GetRandomElement();
        FloatingTextSpawner
            .CreateFloatingTextStatic(transform.position, randomAlert, Color.red, destroyAfter: 0.8f, fontSize: 4, floatSpeed: 0.5f);
    }

    private void stateMachineProcedure()
    {
        if (!_runStateMachine)
            return;

        if (!_gameManager.IsGameRunning())
            return;

        float idleDistance = 20.0f;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerController.transform.position);

        switch (_state)
        {
            case NPCState.Idle:
                if (distanceToPlayer < idleDistance)
                    _state = NPCState.Patrol;
                break;

            case NPCState.Patrol:
                moveTo(_targetPosition, npcSaysRandomStuff);
                if (findTarget())
                    break;

                if (distanceToPlayer > idleDistance)
                    _state = NPCState.Idle;

                float distanceToTarget = Vector2.Distance(transform.position, _targetPosition);
                if (distanceToTarget > _stopDistance)
                    break;

                _patrolWaitTimer -= Time.deltaTime;

                if (_patrolWaitTimer <= 0.0f)
                {
                    _targetPosition = generatePatrollingPosition();
                    _patrolWaitTimer = UnityEngine.Random.Range(_patrolWaitTimeInterval.x, _patrolWaitTimeInterval.y);
                }

                break;

            case NPCState.Chase:

                if (!_playerStats.IsAlive() || !_gameManager.IsGameRunning())
                {
                    _NPCWeapons.StopAttack();
                    _state = NPCState.GoBack;
                    break;
                }

                _shouldChase = distanceToPlayer > _attackDistance;

                if (distanceToPlayer >= _stopDistance && _shouldChase)
                    moveTo(_playerController.transform.position);

                _NPCWeapons.PresentWeapon(true);

                if (distanceToPlayer <= _attackDistance)
                {
                    rotateEnemyTowards(_playerController.transform.position);
                    _NPCWeapons.AttackTarget(_playerController.transform);
                }
                else
                    _NPCWeapons.StopAttack();

                if (distanceToPlayer >= _stopFollowingDistance)
                {
                    _NPCWeapons.PresentWeapon(false);
                    _state = NPCState.GoBack;
                }

                break;

            case NPCState.GoBack:
                moveTo(_originPosition);
                findTarget();

                float distanceToOrigin = Vector2.Distance(transform.position, _originPosition);
                if (distanceToOrigin <= _stopDistance)
                    _state = NPCState.Patrol;
                break;

            default:
                break;
        }
    }

    private bool findTarget()
    {
        if (_gameManager == null || _type.Equals(NPCType.Ally))
            return false;

        if (!_playerStats.IsAlive() || !_gameManager.IsGameRunning())
            return false;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerController.transform.position);
        if (distanceToPlayer <= _viewDistance)
        {
            _targetPosition = _playerController.transform.position;
            _state = NPCState.Chase;
            return true;
        }
        else
        {
            _state = NPCState.Patrol;
            return false;
        }
    }

    public void AlertNPC(Transform target)
    {
        if (_NPCWeapons.HasShootingTarget)
            return;

        _targetPosition = target.position;
        ExtendViewAndAttackDistance();
        npcAlerted();
    }

    public bool OnInteractWithPlayer(Transform playerTransform)
    {
        if (playerTransform == null)
            return false;

        if (_type.Equals(NPCType.Enemy))
            return false;

        //interact with player, turn to him and wait
        _isInteracting = true;
        _targetPosition = playerTransform.position;
        setRunStateMachine(false);

        return true;
    }

    private void stopInteractWithPlayer()
    {
        if (_type.Equals(NPCType.Enemy))
            return;

        //stop interacting and return to normal routine
        _isInteracting = false;
        _state = NPCState.Idle;
        setRunStateMachine(true);

        SpeechBox.DeactivateSpeechBoxSingleStatic();
    }

    private void setRunStateMachine(bool value)
    {
        _runStateMachine = value;
    }

    public void ExtendViewAndAttackDistance()
    {
        float extendFactor = 1.7f;

        _viewDistance *= extendFactor;
        _attackDistance *= extendFactor;
        _stopFollowingDistance *= extendFactor;
    }

    private void resizeViewAndAttackDistance()
    {
        float resizeSpeedFactor = 0.8f;

        if (_viewDistance > _initialViewDistance)
            _viewDistance -= Time.deltaTime * resizeSpeedFactor;

        if (_attackDistance > _initialAttackDistance)
            _attackDistance -= Time.deltaTime * resizeSpeedFactor;

        if (_stopFollowingDistance > _initialStopFollowingDistance)
            _stopFollowingDistance -= Time.deltaTime * resizeSpeedFactor;
    }


    public bool ObstaclesInTheWay(Vector3 targetPosition, float distance = 9999.0f)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float offsetFromOrigin = 1.0f;
        RaycastHit2D[] hits2D = Physics2D.RaycastAll(transform.position + direction * offsetFromOrigin, direction, distance);

        PlayerController playerController = hits2D[0].collider.GetComponent<PlayerController>();

        if (playerController != null)
            return false;

        Door door = hits2D[0].collider.GetComponent<Door>();

        if (door != null)
            return door.IsClosed();

        return true;
    }

    private void moveTo(Vector3 targetPosition, Action actionOnStopped = null, Action actionOnStarted = null)
    {
        if (targetPosition == default(Vector3))
        {
            _rigidBody.velocity = Vector3.zero;
            return;
        }

        Vector2 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        rotateEnemyTowards(targetPosition);

        float rigidbodyStopDistance = 0.1f;
        if (distance <= rigidbodyStopDistance)
        {
            if (_isMoving)
            {
                _isMoving = false;
                actionOnStopped?.Invoke();
            }

            return;
        }

        if (!_isMoving)
        {
            _isMoving = true;
            actionOnStarted?.Invoke();
        }

        _rigidBody.MovePosition(transform.position + _NPCStats.EnemySpeed.GetFinalValue() * (Vector3)direction.normalized * Time.fixedDeltaTime);
    }

    private void rotateEnemyTowards(Vector3 target)
    {
        if (target == null)
            return;

        Vector2 direction = (target - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, 0.3f);
    }

    private Vector3 generatePatrollingPosition()
    {
        return _originPosition + generateRandomDirection() * UnityEngine.Random.Range(PatrollingDistance.x, PatrollingDistance.y);
    }

    private Vector3 generateRandomDirection()
    {
        return new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _stopDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _viewDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackDistance);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _stopFollowingDistance);
    }
}
