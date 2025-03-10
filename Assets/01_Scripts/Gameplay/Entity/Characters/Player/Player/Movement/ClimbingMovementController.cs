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


    public void Initialize(InputHandler input, GameObject mainCamera, StatHandler statHandler)
    {
        _input = input;
        _mainCamera = mainCamera;
        _controller = GetComponent<CharacterController>();
        _statHandler = statHandler;

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

        // 이동 방향을 정규화 (이제 Y축도 포함)
        Vector3 inputDirection = new Vector3(_input.move.x, _input.move.y, 0.0f).normalized;

        // 플레이어가 이동 방향을 바라보도록 회전 (이제 상하 이동도 고려)
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.z, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // 플레이어를 벽 표면을 기준으로 회전
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
        }

        // 목표 이동 방향 설정 (이제 위/아래 이동도 가능)
        Vector3 targetDirection = new Vector3(inputDirection.x, inputDirection.y, 0.0f);

        // 이동 적용 (이제 Y축 이동도 포함)
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));
    }
}