using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private Camera _camera;

    private bool _isRunning = false;
    private bool _canRun = true;
    private bool _legsInjured = false;
    private bool _inputActive = true;
    
    private Rigidbody2D _rigidBody;
    private Vector2 _movement;
    private Vector2 _mousePosition;

    private PlayerStats _playerStats;

    public override void Awake()
    {
        base.Awake();

        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();        
        _camera = Camera.main;

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameComplete += GameManager_OnGameComplete;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameComplete -= GameManager_OnGameComplete;
    }

    private void GameManager_OnGameComplete(GameEndType arg1, int arg2)
    {
        DeactivateInput();
    }

    private void Update()
    {
        if (_inputActive)
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.y = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                int itemIndex = 0;
                Item itemToUse = ConsumableItemsUIMenu.GetConsumableItemsUIStatic()[itemIndex];
                PlayerInventory.UseConsumableItemStatic(itemToUse);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                int itemIndex = 1;
                Item itemToUse = ConsumableItemsUIMenu.GetConsumableItemsUIStatic()[itemIndex];
                PlayerInventory.UseConsumableItemStatic(itemToUse);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                int itemIndex = 2;
                Item itemToUse = ConsumableItemsUIMenu.GetConsumableItemsUIStatic()[itemIndex];
                PlayerInventory.UseConsumableItemStatic(itemToUse);
            }
        }

        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        triggerRunning();
    }

    private void FixedUpdate()
    {
        if (_playerStats.IsAlive())
        {
            rotatePlayer(_mousePosition);
            movePlayer(_movement.normalized);
        }
    }

    public void DeactivateInput()
    {
        _inputActive = false;
        _movement = Vector2.zero;
    }

    private void movePlayer(Vector2 axisMovement)
    {
        if (axisMovement == null)
            return;

        if (axisMovement.sqrMagnitude > 1.0f)
            axisMovement.Normalize();

        Vector2 currentPosition = _rigidBody.position;
        float currentAngle = _rigidBody.rotation;

        Vector2 movementDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle));
        Vector2 strafeDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (currentAngle - 90.0f)), Mathf.Sin(Mathf.Deg2Rad * (currentAngle - 90.0f)));

        Vector2 finalMovementPosition = currentPosition + (axisMovement.x * strafeDirection + axisMovement.y * movementDirection) * _playerStats.PlayerSpeed.GetFinalValue() * Time.fixedDeltaTime;

        _rigidBody.MovePosition(finalMovementPosition);
    }

    public void SetLegsInjured(bool value)
    {
        _legsInjured = value;
    }

    private void triggerRunning()
    {
        if (_legsInjured)
            return;

        float runningSpeedMultiplier = 1.5f;

        _canRun = _playerStats.CurrentStamina > 0.0f;
        
        if (!_canRun && _isRunning)
        {
            _isRunning = false;
            _playerStats.PlayerSpeed.RemoveBaseMultiplier(runningSpeedMultiplier);
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!_isRunning)
            {
                if (_movement.magnitude > 0.0f)
                {
                    _isRunning = true;
                    _playerStats.PlayerSpeed.AddBaseMultiplier(runningSpeedMultiplier);
                }
            }
            else
            {
                _isRunning = false;
                _playerStats.PlayerSpeed.RemoveBaseMultiplier(runningSpeedMultiplier);
            }
        }
    }

    private void rotatePlayer(Vector2 targetPosition)
    {
        Vector2 direction = targetPosition - _rigidBody.position;

        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        _rigidBody.rotation = rotationAngle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDisturbable disturbable = collision.GetComponent<IDisturbable>();

        if (disturbable != null)
            disturbable.DisturbAnimation();
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    public bool IsMoving()
    {
        return _movement.magnitude > 0.0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 40.0f);
    }
}