using UnityEngine;

namespace Framework.Characters
{
    [RequireComponent(typeof(Collider))]
    public class CharacterBody : MonoBehaviour
    {
        [Header("Ground Check Settings")] public LayerMask groundLayerMask;
        [SerializeField] private float groundCheckDistance = 0.2f; // Ray를 통해 지면 검사거리
        [SerializeField] private float groundCheckOffsetY = 0.1f; // Ray 시작 위치를 바닥에서 약간 올림
        [SerializeField] private float groundCheckRadiusMultiplier = 1.0f; // Ray의 간격 조절 (1.0 = 기본 크기)

        [Header("ForwardWall Check Settings")] public LayerMask wallLayerMask;
        [SerializeField] private float wallCheckDistance = 0.2f; // Ray를 통해 지면 검사거리
        [SerializeField] private float wallCheckOffsetZ = 0.1f; // Ray 시작 위치를 바닥에서 약간 올림
        [SerializeField] private float wallCheckRadiusMultiplier = 1.0f; // Ray의 간격 조절 (1.0 = 기본 크기)
        
        [Header("Movement Settings")] [SerializeField]
        private float footstepThreshold = 0.3f;

        [Header("Debug Settings")] [SerializeField]
        private bool drawGizmos = true;

        
        public bool IsWallDetected => _wallHitNormal != Vector3.zero;
        private Vector3 _wallHitNormal;
        public Vector3 WallHitNormal => _wallHitNormal;
        
        private Collider _characterCollider;
        private Rigidbody _rigidbody;

        private void OnValidate()
        {
            _characterCollider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public bool IsGrounded()
        {
            // 1. Collider Bounds에서 계산
            Bounds bounds = _characterCollider.bounds;
            Vector3 basePosition = bounds.center;
            basePosition.y = bounds.min.y + groundCheckOffsetY; // 바닥에서 offset 만큼 위로 올림

            // 2. Ray 발사 간격 계산 (Radius Multiplier 적용)
            float xOffset = bounds.extents.x * groundCheckRadiusMultiplier;
            float zOffset = bounds.extents.z * groundCheckRadiusMultiplier;

            // Ray의 시작 지점 설정
            Vector3[] rayOrigins = new Vector3[]
            {
                basePosition + transform.TransformDirection(new Vector3(xOffset, 0, zOffset)), // 앞 오른쪽
                basePosition + transform.TransformDirection(new Vector3(-xOffset, 0, zOffset)), // 앞 왼쪽
                basePosition + transform.TransformDirection(new Vector3(xOffset, 0, -zOffset)), // 뒤 오른쪽
                basePosition + transform.TransformDirection(new Vector3(-xOffset, 0, -zOffset)) // 뒤 왼쪽
            };

            // 3. 각각의 Raycast 검사
            foreach (var rayOrigin in rayOrigins)
            {
                if (Physics.Raycast(
                        rayOrigin,
                        Vector3.down,
                        groundCheckDistance,
                        groundLayerMask,
                        QueryTriggerInteraction.Ignore)) // 지면과 충돌하면
                {
                    if (drawGizmos)
                        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.red);
                    return true; // 충돌이 감지되면 착지 상태
                }

                if (drawGizmos)
                    Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.green);
            }

            // 4. 모든 Ray가 충돌하지 않을 경우 공중 상태로 간주
            return false;
        }

        public bool IsForwardWall()
        {
            // 1. Collider Bounds에서 계산
            Bounds bounds = _characterCollider.bounds;
            Vector3 basePosition = bounds.center;
            basePosition.z = bounds.max.z + wallCheckOffsetZ;

            // 2. Ray 발사 간격 계산 (Radius Multiplier 적용)
            float xOffset = bounds.extents.x * wallCheckRadiusMultiplier;
            float yOffset = bounds.extents.y * wallCheckRadiusMultiplier;

            // Ray의 시작 지점 설정
            Vector3[] rayOrigins = new Vector3[]
            {
                basePosition - transform.up * yOffset, // 위
                basePosition + transform.up * yOffset, // 아래
                basePosition - transform.right * xOffset, //  왼쪽
                basePosition + transform.right * xOffset //  오른쪽
            };
            
            // 3. 각각의 Raycast 검사
            foreach (var rayOrigin in rayOrigins)
            {
                Ray ray = new Ray(rayOrigin, transform.forward);
                if (Physics.Raycast(
                        ray,
                        out var hit,
                        wallCheckDistance,
                        wallLayerMask)) // 지면과 충돌하면
                {
                    if (drawGizmos)
                        Debug.DrawRay(rayOrigin, transform.forward * wallCheckDistance, Color.red);
                    _wallHitNormal = hit.normal;
                    return true; // 충돌이 감지되면 착지 상태
                }

                if (drawGizmos)
                    Debug.DrawRay(rayOrigin, transform.forward * wallCheckDistance, Color.green);
            }

            // 4. 모든 Ray가 충돌하지 않을 경우 공중 상태로 간주
            _wallHitNormal = Vector3.zero;
            return false;
        }

        public bool IsMoving()
        {
            return _rigidbody.velocity.magnitude > footstepThreshold;
        }

        private void OnDrawGizmosSelected()
        {
            if (_characterCollider == null) return;
            
            IsForwardWall();
        }
    }
}