using Capsule;
using Managers;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Constants
    private const float _dashStaminaCostPerSecond = 20f;
    private const float _climbStaminaCostPerSecond = 20f;

    // Components
    private Rigidbody _playerRigidbody;
    private CharacterBody _characterBody;
    private StatHandler _statHandler;
    private Camera _mainCamera;

    // Movement State
    private Vector2 _moveDirection;
    private bool _isDashing;
    private bool _isClimbing;

    // Climbing State
    public float alignmentThreshold = 0.8f;
    public float requiredClimbTime = 0.5f;
    private float _climbTimer;

    private void Awake()
    {
        CacheComponents();
        RegisterInputCallbacks();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_isClimbing)
            HandleClimbing();
        else
            HandleMovement();
    }

    // --- Core Logic ---
    private void HandleClimbing()
    {
        if (!CanClimb())
            return;

        UseStaminaOrStopClimbing(_climbStaminaCostPerSecond);

        Vector3 wallNormal = _characterBody.WallHitNormal;

        Vector3 rightVector = Vector3.Cross(Vector3.up, wallNormal).normalized;
        Vector3 climbAxis = Vector3.Cross(wallNormal, rightVector).normalized;

        Vector3 climbDirection = climbAxis * _moveDirection.y - rightVector * _moveDirection.x;

        if (_characterBody.IsOnGrounded && Vector3.Dot(Vector3.down, climbDirection) > 0)
        {
            ResetClimbingState();
            return;
        }

        Vector3 wallForward = -_characterBody.WallHitNormal;
        climbDirection *= _statHandler.ClimbSpeed * Time.deltaTime;

        _playerRigidbody.AddForce(climbDirection, ForceMode.VelocityChange);
        
        SmoothlyRotateTowardsDirection(wallForward);
    }

    private void HandleMovement()
    {
        Transform cameraTransform = _mainCamera.transform;

        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 direction = forward * _moveDirection.y + right * _moveDirection.x;
        float speed = _statHandler.MoveSpeed;

        if (_isDashing && _characterBody.IsMoving())
            speed = HandleDash(speed);

        if (_moveDirection != Vector2.zero)
            SmoothlyRotateTowardsDirection(direction);

        direction *= speed * Time.deltaTime;
        _playerRigidbody.AddForce(direction, ForceMode.VelocityChange);
        
        AttachToWallIfPossible();
    }

    // --- Helper Methods ---
    private void CacheComponents()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _characterBody = GetComponent<CharacterBody>();
        _statHandler = GetComponent<StatHandler>();
    }

    private void RegisterInputCallbacks()
    {
        InputManager.Instance.OnMoveInput += OnMoveInput;
        InputManager.Instance.OnJumpPressed += Jump;
        InputManager.Instance.OnDashInput += OnDashInput;
    }

    private bool CanClimb()
    {
        if (!_characterBody.IsOnWall || !_isClimbing)
        {
            ResetClimbingState();
            return false;
        }

        return true;
    }

    private void UseStaminaOrStopClimbing(float costPerSecond)
    {
        if (_characterBody.IsMoving())
        {
            bool hasStamina = SignalManager.Instance.EmitSignal<float, bool>(
                "OnUseStamina", costPerSecond * Time.deltaTime);
            if (!hasStamina)
            {
                ResetClimbingState();
            }
        }
    }
    

    private void ApplyMovementPhysics(Vector3 direction)
    {
        float speed = _statHandler.MoveSpeed;

        if (_isDashing && _characterBody.IsMoving())
            speed = HandleDash(speed);

        if (_moveDirection != Vector2.zero)
            SmoothlyRotateTowardsDirection(direction);

        direction *= speed * Time.deltaTime;
        _playerRigidbody.AddForce(direction, ForceMode.VelocityChange);
    }

    private float HandleDash(float baseSpeed)
    {
        bool canDash = SignalManager.Instance.EmitSignal<float, bool>(
            "OnUseStamina", _dashStaminaCostPerSecond * Time.deltaTime);
        if (canDash)
            return baseSpeed + _statHandler.SprintSpeed;

        _isDashing = false;
        return baseSpeed;
    }

    private void AttachToWallIfPossible()
    {
        if (_characterBody.IsOnWall && _characterBody.IsOnGrounded && _moveDirection != Vector2.zero)
        {
            Vector3 wallNormal = _characterBody.WallHitNormal;
            float alignment = Vector3.Dot(transform.forward, -wallNormal);

            if (alignment >= alignmentThreshold)
            {
                _climbTimer += Time.deltaTime;
                if (_climbTimer >= requiredClimbTime)
                    StartClimbing();
            }
            else
            {
                _climbTimer = 0f;
            }
        }
        else
        {
            _climbTimer = 0f;
        }
    }

    private void StartClimbing()
    {
        _isClimbing = true;
        _playerRigidbody.velocity = Vector3.zero;
        _playerRigidbody.useGravity = false;
    }
    
    private void SmoothlyRotateTowardsDirection(Vector3 lookDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    private void ResetClimbingState()
    {
        _isClimbing = false;
        _playerRigidbody.useGravity = true;
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    // --- Event Handlers ---
    private void OnMoveInput(Vector2 input) => _moveDirection = input;
    private void OnDashInput(bool isDashing) => _isDashing = isDashing;

    private void Jump(bool isJumping)
    {
        if(!isJumping) return;
        if (_isClimbing)
        {
            Vector3 jumpDirection = (_characterBody.WallHitNormal * 2f + Vector3.up).normalized;
            _playerRigidbody.AddForce(jumpDirection * _statHandler.JumpHeight, ForceMode.Impulse);
            transform.forward = new Vector3(jumpDirection.x, 0, jumpDirection.z);
            ResetClimbingState();
        }
        else if (_characterBody.IsOnGrounded)
        {
            _playerRigidbody.AddForce(Vector3.up * _statHandler.JumpHeight, ForceMode.Impulse);
        }
    }
}