using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Needed References")]
    [SerializeField] private PlayerBehaviour _playerBehaviour;
    private Rigidbody2D _rb;

    [Header("Movement")]    
    [SerializeField] private float _moveSpeed = 9f;
    [SerializeField] private float _acceleration = 13f;
    [SerializeField] private float _deceleration = 16f;
    [SerializeField] private float _velocityForce = 0.96f;
    [SerializeField] private float _frictionAmount = 0.35f;
    [SerializeField] private float _iceMapFrictionAmount = 0.1f;

    private Vector2 _movementInput;
    private const float _inputDeadzone = 0.1f;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _fallMultiplier = 2.7f; // Increases gravity while falling
    [SerializeField] private float _ascendMultiplier = 2.2f; // Adjusts gravity while rising
    private bool _isJumping = false;

    [Header("Dash")]
    [SerializeField] private float _dashForce = 10f;
    private float _actualDashModifier = 1f;
    private float _diagonalDashModifier = 0.7071f;

    [SerializeField] private float _dashingTime = 0.1f;
    [SerializeField] private float _dashCooldown = 0.2f;
    [SerializeField] private int _maxDashCount = 1;
    private int _currentDashCount;
    private bool _isDashOnCooldown = false;
    private const float _dashInputDeadzone = 0.4f;

    [Header("Landing")]
    [SerializeField] private float _landingAnimTime = 0.15f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentDashCount = _maxDashCount;

        if(GameManager.Instance._currentMapData._element == Element.Ice)
        {
            _frictionAmount = _iceMapFrictionAmount;
        }
    }

    void Update()
    {
        JumpAndDashUpdate();
    }

    private void FixedUpdate()
    {
        if (_playerBehaviour.CanAct() && !_playerBehaviour._isShielding && !_playerBehaviour._isAttacking)
        {
            Move();
        }

        ApplyFriction();

        if (!_playerBehaviour._isHitStunned && !_playerBehaviour._isDashing)
        {
            ApplyJumpPhysics();
        }
    }

    public void InputMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    public void InputJump(InputAction.CallbackContext context)
    {
        if(context.started && _playerBehaviour.CanAct() && !_playerBehaviour._isShielding && !_playerBehaviour._isAttacking && _playerBehaviour._isGrounded && !_isJumping)
        {
            Jump();
        }  
    }

    public void InputDash(InputAction.CallbackContext context)
    {
        if(context.started && _playerBehaviour.CanAct() && !_playerBehaviour._isShielding && !_playerBehaviour._isAttacking && _currentDashCount > 0 && !_isDashOnCooldown)
        {
            Dash();
        }
    }

    private void Move()
    {
        if (Mathf.Abs(_movementInput.x) > _inputDeadzone)
        {
            // Calculate target horizontal velocity
            float target_speed = _movementInput.x * _moveSpeed;

            // Calculate the difference between target and current velocity
            float speed_diff = target_speed - _rb.linearVelocity.x;

            // Determine acceleration or deceleration
            float accel_rate;

            if (Mathf.Abs(target_speed) > 0.01f)
            {
                accel_rate = _acceleration;
            }
            else 
            { 
                accel_rate = _deceleration;
            }

            // Apply acceleration based on the velocity difference
            float movement = Mathf.Pow(Mathf.Abs(speed_diff) * accel_rate, _velocityForce) * Mathf.Sign(speed_diff);

            // Apply horizontal movement force
            _rb.AddForce(movement * Vector2.right);

            _playerBehaviour.CheckIfCharacterShouldBeFlipped(_movementInput.x);

            if (_playerBehaviour._isGrounded && _playerBehaviour._groundedTime < _landingAnimTime)
            {
                _playerBehaviour.PlayAnimation("Landing");
            }
            else if (_playerBehaviour._isGrounded && !_isJumping)
            {
                _playerBehaviour.PlayAnimation("Walk");
            }
            else if (_playerBehaviour._isFalling)
            {
                _playerBehaviour.PlayAnimation("Fall");
            }
        }
        else
        {
            if (_playerBehaviour._isGrounded && _playerBehaviour._groundedTime < _landingAnimTime)
            {
                _playerBehaviour.PlayAnimation("Landing");
            }
            else if (_playerBehaviour._isGrounded && !_isJumping)
            {
                _playerBehaviour.PlayAnimation("Idle");
            }
            else if (_playerBehaviour._isFalling)
            {
                _playerBehaviour.PlayAnimation("Fall");
            }
        }
    }

    private void ApplyFriction()
    {
        // Apply ground friction when movement input is absent, or while attacking or shielding
        if (_playerBehaviour._isGrounded) {
            if (Mathf.Abs(_movementInput.x) < _inputDeadzone || _playerBehaviour._isAttacking || _playerBehaviour._isShielding)
            {
                float amount = Mathf.Min(Mathf.Abs(_rb.linearVelocity.x), Mathf.Abs(_frictionAmount));
                amount *= Mathf.Sign(_rb.linearVelocity.x);
                _rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            } 
        }
    }

    private void Jump()
    {
        float jump;

        jump = Mathf.Abs(_jumpForce);

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jump);

        _playerBehaviour.PlayAnimation("Jump");

        _isJumping = true;
    }

    private void Dash()
    {
        // Determine dash direction from player input
        Vector2 dash_direction = Vector2.zero;

        if (Mathf.Abs(_movementInput.x) > _dashInputDeadzone)
        {
            dash_direction.x = 1 * Mathf.Sign(_movementInput.x);
        }

        if (Mathf.Abs(_movementInput.y) > _dashInputDeadzone)
        {
            dash_direction.y = 1 * Mathf.Sign(_movementInput.y);
        }

        // Reduce dash force diagonally to maintain consistent speed
        if (dash_direction.x != 0 && dash_direction.y != 0)
        {
            _actualDashModifier = _diagonalDashModifier;
        }
        else
        {
            _actualDashModifier = 1f;
        }

        // Start the dash if a direction is specified
        if (dash_direction != Vector2.zero)
        {
            if (_playerBehaviour._cameraScript != null)
            {
                _playerBehaviour._cameraScript.OnDash();
            }

            StartCoroutine(DashCoroutine(dash_direction));
            _currentDashCount--;
        }
    }

    private IEnumerator DashCoroutine(Vector2 dash_direction)
    {
        _playerBehaviour._isDashing = true;
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;
        _rb.linearVelocity = dash_direction * _dashForce * _actualDashModifier;

        // Play dash animation
        if (dash_direction.x != 0 && dash_direction.y == 0)
        {
            _playerBehaviour.PlayAnimation("Dash");
        }
        else if (dash_direction.x != 0 && dash_direction.y == 1)
        {
            _playerBehaviour.PlayAnimation("Up diagonal dash");
        }
        else if (dash_direction.x != 0 && dash_direction.y == -1)
        {
            _playerBehaviour.PlayAnimation("Down diagonal dash");
        }
        else if (dash_direction.x == 0 && dash_direction.y == 1)
        {
            _playerBehaviour.PlayAnimation("Up dash");
        }
        else if (dash_direction.x == 0 && dash_direction.y == -1)
        {
            _playerBehaviour.PlayAnimation("Down dash");
        }

        yield return new WaitForSeconds(_dashingTime);
        _playerBehaviour._isDashing = false;
        _rb.gravityScale = originalGravity;
        _isDashOnCooldown = true;

        yield return new WaitForSeconds(_dashCooldown);
        _isDashOnCooldown = false;
    }

    private void JumpAndDashUpdate()
    {
        if (_playerBehaviour._isGrounded && _playerBehaviour._rb.linearVelocityY == 0)
        {
            _isJumping = false;
            if (!_playerBehaviour._isDashing)
            {
                _currentDashCount = _maxDashCount;
            }
        }
    }

    private void ApplyJumpPhysics()
    {
        if (_rb.linearVelocity.y < 0)
        {
            // Falling: Apply fall multiplier to make descent faster
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * _fallMultiplier * Time.fixedDeltaTime;

        }
        else if (_rb.linearVelocity.y > 0)
        {
            // Rising: Change multiplier to make player reach peak of jump faster
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * _ascendMultiplier * Time.fixedDeltaTime;
        }
    }

    public float GetMoveSpeed()
    {
        return _moveSpeed;
    }

    public void SetMoveSpeed(float new_speed)
    {
        _moveSpeed = new_speed;
    }
}
