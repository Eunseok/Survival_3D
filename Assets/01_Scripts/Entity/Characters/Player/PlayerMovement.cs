using Framework.Characters;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float staminaPerSecForDash = 20f;

    private Rigidbody _rigidbody;
    private CharacterBody _characterBody;
    private StatHandler _statHandler;

    private Vector2 _moveDirection;

    private bool _isDashing;
    private bool _isClimbing;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _characterBody = GetComponent<CharacterBody>();
        _statHandler = GetComponent<StatHandler>();

        InputManager.Instance.OnMoveInput += ctx => _moveDirection = ctx;
        InputManager.Instance.OnJumpPressed += Jump;
        InputManager.Instance.OnDashInput += OnDashInput;
    }

    private void FixedUpdate()
    {
        if (_isClimbing)
            ClimbMove();
        else
            Move();
    }

    private void ClimbMove()
    {
        if (_characterBody.IsMoving())
        {
            bool canDash =
                SignalManager.Instance.EmitSignal<float, bool>("OnUseStamina", staminaPerSecForDash * Time.deltaTime);
            if (canDash)
             _isDashing = false;
        }

        // 2) 입력 받기
        float verticalInput = _moveDirection.y;   // WS
        float horizontalInput = _moveDirection.x; // AD

        if (!_characterBody.IsForwardWall(out var wallNormal))
        {
            _isClimbing = false;
        }
        
        // 3) 벽 노말 기반 이동 축 계산
        var rightOfWall = Vector3.Cross(Vector3.up, wallNormal).normalized;
        var upWall = Vector3.Cross(wallNormal, rightOfWall).normalized;

        // 4) 최종 이동 벡터
        var climbDir = (upWall * verticalInput + rightOfWall * horizontalInput);

        // 5) CharacterController로 이동 적용
        _rigidbody.velocity = climbDir * (_statHandler.ClimbSpeed * Time.deltaTime);
        
        // 등반 상태 On
        _isClimbing = true;

        // 캐릭터를 벽 방향으로 돌리기
        Vector3 wallForward = -wallNormal; 
        Quaternion targetRotation = Quaternion.LookRotation(wallForward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1f * Time.deltaTime);

        // 이전 속도 등 초기화 필요 시 처리
        // ex) Rigidbody를 사용한다면, rb.velocity = Vector3.zero; 등
    }
    void TryAttachToWall()
    {
       // if (isWallDetected)
        {
            // // 등반 상태 On
            // _isClimbing = true;
            //
            // // 캐릭터를 벽 방향으로 돌리기
            // Vector3 wallForward = -wallNormal; 
            // Quaternion targetRotation = Quaternion.LookRotation(wallForward, Vector3.up);
            // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, attachRotationSpeed * Time.deltaTime);
            //
            // // 이전 속도 등 초기화 필요 시 처리
            // // ex) Rigidbody를 사용한다면, rb.velocity = Vector3.zero; 등
        }
    }
    private void Move()
    {
        Vector3 direction = transform.forward * _moveDirection.y + transform.right * _moveDirection.x;
        float moveSpeed = _statHandler.Speed;

        if (_isDashing && _characterBody.IsMoving())
        {
            bool canDash =
                SignalManager.Instance.EmitSignal<float, bool>("OnUseStamina", staminaPerSecForDash * Time.deltaTime);
            if (canDash)
                moveSpeed = _statHandler.DashSpeed;
        }

        direction *= moveSpeed;
        direction.y = _rigidbody.velocity.y;
        _rigidbody.velocity = direction;
    }


    private void OnDashInput(bool isDashing)
    {
        _isDashing = isDashing;
    }

    private void Jump()
    {
        if (_characterBody && _characterBody.IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * _statHandler.JumpForce, ForceMode.Impulse);
        }
    }
}