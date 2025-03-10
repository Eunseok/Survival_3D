using Framework.Characters;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ClimbingMovementController : MonoBehaviour
{
    [Tooltip("캐릭터가 이동 방향을 바라보는 속도")] [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("가속 및 감속 비율")] public float SpeedChangeRate = 10.0f;

    [Space(10)] [Tooltip("플레이어가 점프할 수 있는 최대 높이")]
    public float JumpHeight = 1.2f;

    [Tooltip("캐릭터가 사용할 자체 중력 값 (엔진 기본값은 -9.81f)")]
    public float Gravity = -15.0f;

    [Space(10)] [Tooltip("점프 후 다시 점프할 수 있기까지 필요한 시간 (0f으로 설정하면 즉시 점프 가능)")]
    public float JumpTimeout = 0.50f;

    [Tooltip("캐릭터가 낙하 상태로 진입하기 전까지 걸리는 시간 (계단 내려갈 때 유용)")]
    public float FallTimeout = 0.15f;

    // 플레이어 이동 관련 변수
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    protected float _verticalVelocity;
    private readonly float _terminalVelocity = 53.0f;

    //애니메이터로 전달할 값
    public float AnimationBlend { get; private set; }
    public float InputMagnitude { get; private set; }
    public bool IsJumping { get; private set; } // 점프 중인지 저장
    public bool IsFreeFalling { get; private set; } // 낙하 중인지 저장


    // 점프 및 낙하 시간 변수
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private CharacterController _controller;
    private InputHandler _input;
    private GameObject _mainCamera;
    private StatHandler _statHandler;

    private WallChecker _wallChecker;

    public void Initialize(InputHandler input, GameObject mainCamera, StatHandler statHandler)
    {
        _input = input;
        _mainCamera = mainCamera;
        _controller = GetComponent<CharacterController>();
        _statHandler = statHandler;
        _wallChecker = GetComponent<WallChecker>();

        // 점프 및 낙하 타이머 초기화
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    public void Move()
    {
        float targetSpeed = _statHandler.ClimbSpeed;
        if (_input.move == Vector2.zero) targetSpeed = 0f;

        // 현재 이동 속도 계산 (이제 Y축 이동도 포함)
        float currentSpeed = new Vector3(_controller.velocity.x, _controller.velocity.y, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        InputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // 목표 속도에 맞춰 가속 또는 감속
        if (currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentSpeed, targetSpeed * InputMagnitude, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // 애니메이션 블렌딩 처리
        AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (AnimationBlend < 0.01f) AnimationBlend = 0f;
        
        
        // --- 벽 정보를 기반으로 이동 방향 계산 ---
        Vector3 wallNormal = _wallChecker.WallHitNormal;
        
        
        // 벽 표면에 평행한 축 계산
        Vector3 wallRight = Vector3.Cross(Vector3.up, wallNormal).normalized;
        Vector3 wallUp = Vector3.Cross(wallNormal, wallRight).normalized;
        
        // 입력 방향을 벽 표면 기준으로 변환
        Vector3 direction = wallUp * _input.move.y + wallRight * _input.move.x;
        Debug.Log(direction);
        // 회전 로직: 벽 표면의 방향 기준으로 캐릭터 회전
        Vector3 wallForward = -wallNormal;
        Quaternion targetRotation = Quaternion.LookRotation(wallForward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

        // // 목표 이동 방향 설정 (이제 위/아래 이동도 가능)
        // Vector3 targetDirection = new Vector3(direction.x, direction.y, 0.0f);

        // 이동 적용 (이제 Y축 이동도 포함)
        _controller.Move(direction.normalized * (_speed * Time.deltaTime));
    }
    
    
    // public void Move()
    // {
    //     // 목표 클라밍 속도 설정
    //     float targetSpeed = _statHandler.ClimbSpeed;
    //     if (_input.move == Vector2.zero) targetSpeed = 0f;
    //
    //     // 현재 이동 속도 계산 (Y축 포함)
    //     float currentSpeed = new Vector3(_controller.velocity.x, _controller.velocity.y, _controller.velocity.z).magnitude;
    //
    //     // 속도 임계치
    //     float speedOffset = 0.1f;
    //     InputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
    //
    //     // 목표 속도를 기준으로 가속 또는 감속
    //     if (currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
    //     {
    //         _speed = Mathf.Lerp(currentSpeed, targetSpeed * InputMagnitude, Time.deltaTime * SpeedChangeRate);
    //         _speed = Mathf.Round(_speed * 1000f) / 1000f;
    //     }
    //     else
    //     {
    //         _speed = targetSpeed;
    //     }
    //
    //     // 애니메이션 블렌딩 처리
    //     AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
    //     if (AnimationBlend < 0.01f) AnimationBlend = 0f;
    //
    //     // --- 벽 정보를 기반으로 이동 방향 계산 ---
    //     Vector3 wallNormal = _wallChecker.WallHitNormal;
    //
    //     // 벽 표면에 평행한 축 계산
    //     Vector3 wallRight = Vector3.Cross(Vector3.up, wallNormal).normalized;
    //     Vector3 wallUp = Vector3.Cross(wallNormal, wallRight).normalized;
    //
    //     // 입력 방향을 벽 표면 기준으로 변환
    //     Vector3 direction = wallUp * _input.move.y + wallRight * _input.move.x;
    //
    //     // 이동 벡터 계산
    //     direction *= _speed * Time.deltaTime;
    //
    //     // 회전 로직: 벽 표면의 방향 기준으로 캐릭터 회전
    //     Vector3 wallForward = -wallNormal;
    //     Quaternion targetRotation = Quaternion.LookRotation(wallForward, Vector3.up);
    //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    //
    //     // 캐릭터 이동 적용
    //     _controller.Move(direction);
    // }
    //
    //
    // private Vector3 CalculateClimbDirection()
    // {
    //     Vector3 wallNormal = _wallChecker.WallHitNormal;
    //
    //     float direction = Vector3.Cross(wallNormal, Vector3.Cross(Vector3.up, wallNormal)).normalized * _moveDirection.y -
    //            Vector3.Cross(Vector3.up, wallNormal).normalized * _moveDirection.x;
    //     
    //     Vector3 wallForward = -_wallChecker.WallHitNormal;
    //     direction *= _statHandler.ClimbSpeed * Time.deltaTime;
    //
    //     _playerRigidbody.AddForce(direction, ForceMode.VelocityChange);
    //     Quaternion targetRotation = Quaternion.LookRotation(wallForward, Vector3.up);
    //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    // }
    
}