using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AlpacaMyGames;

public enum NPCState
{
    Idle,
    Patrol,
    Chase,
    GoBack
}

public enum NPCType
{
    Enemy,
    Ally,
}

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

        resetInitialDistances();

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
        if (_gameManager != null && !_gameManager.IsGameRunning())
            return;

        stateMachineProcedure();
        if (!_runStateMachine)
            rotateEnemyTowards(_targetPosition);

        if (_isInteracting && Vector2.Distance(transform.position, _playerController.transform.position) > _interactionRadius)
            stopInteractWithPlayer();
    }

    private void LateUpdate()
    {
        resizeViewAndAttackDistance();
    }

    public NPCType GetNPCType()
    {
        return _type;
    }

    private void stateMachineProcedure()
    {
        if (!_runStateMachine)
            return;

        switch (_state)
        {
            case NPCState.Idle:
                if (Vector2.Distance(_playerController.transform.position, transform.position) < 40.0f)
                {
                    _state = NPCState.Patrol;
                }
                break;

            case NPCState.Patrol:
                moveTo(_targetPosition);
                if (Vector2.Distance(transform.position, _targetPosition) <= _stopDistance)
                {
                    _patrolWaitTimer -= Time.deltaTime;

                    if (_patrolWaitTimer <= 0.0f)
                    {
                        if (Utilities.ChanceFunc(33))
                        {
                            List<string> messages = new List<string> { "*Hmmph*", "*Grunt*", "*Ahh*", "What a'\n'boring day..." };
                            string randomMessage = messages.GetRandomElement();
                            FloatingTextSpawner.CreateFloatingTextStatic(transform.position, randomMessage, Color.white, 0.8f, 4, 0.5f);
                        }

                        _targetPosition = generatePatrollingPosition();
                        _patrolWaitTimer = UnityEngine.Random.Range(_patrolWaitTimeInterval.x, _patrolWaitTimeInterval.y);
                    }
                }

                findTarget();

                break;

            case NPCState.Chase:

                if (_playerStats.IsAlive() && _gameManager.IsGameRunning())
                {
                    _shouldChase = Vector2.Distance(transform.position, _playerController.transform.position) > _attackDistance;

                    if (Vector2.Distance(transform.position, _playerController.transform.position) >= _stopDistance && _shouldChase)
                        moveTo(_playerController.transform.position);

                    _NPCWeapons.PresentWeapon(true);

                    if (Vector2.Distance(transform.position, _playerController.transform.position) <= _attackDistance)
                    {
                        rotateEnemyTowards(_playerController.transform.position);
                        _NPCWeapons.AttackTarget(_playerController.transform);
                    }
                    else
                        _NPCWeapons.StopAttack();

                    if (Vector2.Distance(transform.position, _playerController.transform.position) >= _stopFollowingDistance)
                    {
                        _NPCWeapons.PresentWeapon(false);
                        _state = NPCState.GoBack;
                    }
                }
                else
                {
                    _NPCWeapons.StopAttack();
                    _state = NPCState.GoBack;
                }
                break;

            case NPCState.GoBack:
                moveTo(_originPosition);
                if (Vector2.Distance(transform.position, _originPosition) <= _stopDistance)
                    _state = NPCState.Patrol;
                break;

            default:
                break;
        }
    }

    public void OnInteractWithPlayer(Transform playerTransform)
    {
        if (playerTransform == null)
            return;

        if (_type.Equals(NPCType.Enemy))
            return;

        //interact with player, turn to him and wait
        _isInteracting = true;
        _targetPosition = playerTransform.position;
        setRunStateMachine(false);
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
        float changeFactor = 1.7f;

        _viewDistance *= changeFactor;
        _attackDistance *= changeFactor;
        _stopFollowingDistance *= changeFactor;
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

    private void findTarget()
    {
        if (_gameManager == null || _type.Equals(NPCType.Ally))
            return;

        //if (_type.Equals(NPCType.Ally))
        //    return;

        if (!_playerStats.IsAlive() || !_gameManager.IsGameRunning())
        {
            //state = EnemyState.GoBack;
            return;
        }

        if (Vector2.Distance(transform.position, _playerController.transform.position) < _viewDistance)
        {
            _targetPosition = _playerController.transform.position;
            _state = NPCState.Chase;
        }
        else
            _state = NPCState.Patrol;
    }

    public bool ObstaclesInTheWay(Vector3 targetPosition, float distance = 9999.0f)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float offsetFromOrigin = 1.0f;
        RaycastHit2D[] hits2D = Physics2D.RaycastAll(transform.position + direction * offsetFromOrigin, direction, distance);

        PlayerController playerController = hits2D[0].collider.GetComponent<PlayerController>();

        if (playerController != null)
            return false;
        else
            return true;
    }

    private void moveTo(Vector3 targetPosition)
    {
        if (targetPosition != default(Vector3))
        {
            Vector2 direction = targetPosition - transform.position;
            float distance = direction.magnitude;

            rotateEnemyTowards(targetPosition);

            if (distance > 0.1f)
                _rigidBody.MovePosition(transform.position + _NPCStats.EnemySpeed.GetFinalValue() * (Vector3)direction.normalized * Time.fixedDeltaTime);
        }
        else
            _rigidBody.velocity = Vector2.zero;
    }

    private void rotateEnemyTowards(Vector3 target)
    {
        if (target == null)
            return;

        Vector2 direction = (target - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.Lerp(transform.rotation, q, 0.3f);
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
