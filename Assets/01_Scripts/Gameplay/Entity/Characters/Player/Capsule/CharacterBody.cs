using UnityEngine;

namespace Capsule
{
    [RequireComponent(typeof(Collider))]
    public class CharacterBody : MonoBehaviour
    {
        [Header("Ground Check Settings")]
        public bool isGroundCheck = true;
        public LayerMask groundLayerMask = ~0;
        [SerializeField] private float groundCheckDistance = 0.2f; // Ray를 통해 지면 검사거리
        [SerializeField] private float groundCheckOffsetY = 0.1f; // Ray 시작 위치를 바닥에서 약간 올림
        [SerializeField] private float groundCheckRadiusMultiplier = 1.0f; // Ray의 간격 조절 (1.0 = 기본 크기)

        [Header("ForwardWall Check Settings")]
        public bool isForwardWallCheck = true;
        public LayerMask wallLayerMask = 0;
        [SerializeField] private float wallCheckDistance = 0.2f; // Ray를 통해 지면 검사거리
        [SerializeField] private float wallCheckOffsetZ = 0.1f; // Ray 시작 위치를 바닥에서 약간 올림
        [SerializeField] private float wallCheckRadiusMultiplier = 1.0f; // Ray의 간격 조절 (1.0 = 기본 크기)

        [Header("Movement Settings")] [SerializeField]
        private float footstepThreshold = 0.3f;

        [Header("Debug Settings")] [SerializeField]
        private bool drawGizmos = true;


        public bool IsOnGrounded => _groundHitNormal != Vector3.zero;
        private Vector3 _groundHitNormal;
        public Vector3 GroundHitNormal => _groundHitNormal;
        public bool IsOnWall => _wallHitNormal != Vector3.zero;
        private Vector3 _wallHitNormal;
        public Vector3 WallHitNormal => _wallHitNormal;

        private Collider _characterCollider;
        private Rigidbody _rigidbody;

        private void OnValidate()
        {
            _characterCollider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            GroundCheck();
            ForwardWallCheck();
        }

        private void GroundCheck()
        {
            if(!isGroundCheck) return;
            
            // 1. Collider Bounds에서 계산
            var bounds = _characterCollider.bounds;
            var basePosition = bounds.center;
            basePosition.y = bounds.min.y + groundCheckOffsetY; // 바닥에서 offset 만큼 위로 올림

            // 2. Ray 발사 간격 계산 (Radius Multiplier 적용)
            float xOffset = bounds.extents.x * groundCheckRadiusMultiplier;
            float zOffset = bounds.extents.z * groundCheckRadiusMultiplier;

            // Ray의 시작 지점 설정
            var rayOrigins = new[]
            {
                basePosition + transform.TransformDirection(new Vector3(xOffset, 0, zOffset)), // 앞 오른쪽
                basePosition + transform.TransformDirection(new Vector3(-xOffset, 0, zOffset)), // 앞 왼쪽
                basePosition + transform.TransformDirection(new Vector3(xOffset, 0, -zOffset)), // 뒤 오른쪽
                basePosition + transform.TransformDirection(new Vector3(-xOffset, 0, -zOffset)) // 뒤 왼쪽
            };

            // 3. 각각의 Raycast 검사
            foreach (var rayOrigin in rayOrigins)
            {
                var ray = new Ray(rayOrigin, Vector3.down);
                if (Physics.Raycast(
                        ray,
                        out var hit,
                        groundCheckDistance,
                        groundLayerMask)) // 지면과 충돌하면G
                {
                    if (Vector3.Angle(hit.normal, Vector3.up) <= 45f) // 바닥 각도가 45도 이하인지 확인
                    {
                        if (drawGizmos)
                            Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.red);
            
                        _groundHitNormal = hit.normal; // 바닥 법선 정보 저장
                        return;
                    }
                }

                if (drawGizmos)
                    Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.green);
            }

            // 4. 모든 Ray가 충돌하지 않을 경우 공중 상태로 간주
            _groundHitNormal = Vector3.zero;
        }

        private void ForwardWallCheck()
        {
            if(!isForwardWallCheck) return;
            
            // 1. Collider Bounds에서 계산
            var bounds = _characterCollider.bounds;
            var basePosition = bounds.center;
            basePosition.z = bounds.max.z + wallCheckOffsetZ;

            // 2. Ray 발사 간격 계산 (Radius Multiplier 적용)
            var xOffset = bounds.extents.x * wallCheckRadiusMultiplier;
            var yOffset = bounds.extents.y * wallCheckRadiusMultiplier;

            // Ray의 시작 지점 설정
            var rayOrigins = new[]
            {
                basePosition - transform.up * yOffset, // 위
                basePosition + transform.up * yOffset, // 아래
                basePosition - transform.right * xOffset, //  왼쪽
                basePosition + transform.right * xOffset //  오른쪽
            };

            var averageNormal = Vector3.zero;
            // 3. 각각의 Raycast 검사
            foreach (var rayOrigin in rayOrigins)
            {
                var ray = new Ray(rayOrigin, transform.forward);
                if (Physics.Raycast(
                        ray,
                        out var hit,
                        wallCheckDistance,
                        wallLayerMask))
                {
                    float wallAngle = Vector3.Angle(hit.normal, Vector3.up); 
                    if (wallAngle is >= 45f and <= 135f) // 벽 조건: 법선이 Vector3.up과 60~120인 경우
                    {
                        if (drawGizmos)
                            Debug.DrawRay(rayOrigin, transform.forward * wallCheckDistance, Color.red);

                        averageNormal += hit.normal; // 법선을 평균에 추가
                    }

                }

                if (drawGizmos)
                    Debug.DrawRay(rayOrigin, transform.forward * wallCheckDistance, Color.green);
            }

            // 4. 모든 Ray가 충돌하지 않을 경우 공중 상태로 간주
            _wallHitNormal = averageNormal.normalized;
        }

        public bool IsMoving()
        {
            return _rigidbody.velocity.magnitude > footstepThreshold;
        }

        private void OnDrawGizmosSelected()
        {
            if (_characterCollider == null) return;

            GroundCheck();
            ForwardWallCheck();
        }
    }
}