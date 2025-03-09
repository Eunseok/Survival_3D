using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{

    [Header("Camera Settings")]
    public Transform target; // 카메라가 따라갈 캐릭터 (플레이어)
    public LayerMask layerMask; // 레이캐스트용 레이어 마스크
    
    [Header("Distance Settings")]
    public float distance = 3.0f; // 캐릭터와 카메라 사이의 기본 거리
    public float minDistance = 1.0f; // 카메라와 캐릭터 사이의 최소 거리
    public float maxDistance = 5.0f; // 카메라와 캐릭터 사이의 최대 거리
    public float height = 1.5f; // 카메라의 높이
    [Range(0f, 0.1f)] public float zoomSensitivity = 0.01f; // 줌 인/아웃 감도
    
    [Header("Rotation & Position Settings")]
    public Vector2 rotationLimits = new Vector2(-40, 85); // 카메라 상/하 회전 제한
    [Range(0f, 2f)] public float lookSensitivity = 5.0f; // 마우스 감도
    public float smoothSpeed = 10.0f; // 카메라 이동 부드러움 정도

    [Header("Other Settings")]
    public bool isOverShoulder = true; // 어깨 너머 시야 활성화 여부
    public float shoulderOffset = 0.5f; // 어깨 중심의 오프셋
    public bool allowCharacterRotation = true; // 카메라 회전에 따라 타겟 회전 영향을 받을지 결정

    // 회전값 처리
    private float _horizontalRotation; // 카메라의 수평 회전값
    private float _verticalRotation; // 카메라의 수직 회전값

    // 움직임 및 위치 계산
    private Vector3 _currentVelocity = Vector3.zero; // SmoothDamp 속도
    private Vector3 _desiredPosition; // 카메라의 목표 위치 계산값

    void Start()
    {
        if (!target)
        {
            Debug.LogError("Camera Target is not set!");
            return;
        }

        // 초기 회전값 설정
        var angles = transform.eulerAngles;
        _horizontalRotation = angles.y;
        _verticalRotation = angles.x;
        
        // 마우스 커서를 숨기고 고정
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnLookInput += HandleMouseLook;
        InputManager.Instance.OnZoomInput += HandleMouseZoom;
        
        
    }

    private void OnDisable()
    {
        if (InputManager.Instance)
        {
            InputManager.Instance.OnLookInput -= HandleMouseLook;
            InputManager.Instance.OnZoomInput -= HandleMouseZoom;
        }
    }

    private void FixedUpdate()
    {
        UpdateCameraPosition();
        UpdateCameraRotation();
    }

    private void UpdateCameraPosition()
    {
        if (!target) return;

        // 카메라 위치 업데이트 (물리 기반 이동 동기화)
        var rotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);

        // 목표 위치 계산
        _desiredPosition = target.position - (rotation * Vector3.forward * distance) + Vector3.up * height;

        // 레이캐스트로 충돌 감지
        if (Physics.Linecast(target.position, _desiredPosition, out var hit, layerMask))
        {
            _desiredPosition = hit.point;
        }

        // 부드럽게 카메라 위치 이동
        transform.position =
            Vector3.SmoothDamp(transform.position, _desiredPosition, ref _currentVelocity, 1f / smoothSpeed);
    }

    private void UpdateCameraRotation()
    {
        if (!target) return;

        // 캐릭터의 회전을 카메라의 수평 회전에 맞춤
        if (allowCharacterRotation)
        {
            Debug.Log("Allow Character Rotation");
            target.eulerAngles = new Vector3(0, _horizontalRotation, 0);
        }

        // 어깨 중심 오프셋 계산
        var offset = transform.right * (isOverShoulder ? shoulderOffset : 0);

        // 카메라의 회전 적용
        transform.LookAt(target.position + offset + Vector3.up * (height * 0.5f));
    }


    private void HandleMouseLook(Vector2 mouseLook)
    {
        if (!target) return;

        // 카메라 회전 처리 (사용자 입력 기반)
        _horizontalRotation += mouseLook.x * lookSensitivity;
        _verticalRotation -= mouseLook.y * lookSensitivity;

        // 상/하 회전 제한
        _verticalRotation = Mathf.Clamp(_verticalRotation, rotationLimits.x, rotationLimits.y);
    }

    private void HandleMouseZoom(float zoom)
    {
        distance -= zoom * zoomSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }
}