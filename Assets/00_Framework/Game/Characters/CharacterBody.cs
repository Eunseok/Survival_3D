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

        [SerializeField] private Collider characterCollider;

        private void OnValidate()
        {
            characterCollider = GetComponent<Collider>();
        }

        public bool IsGrounded()
        {
            // 1. Collider Bounds에서 계산
            Bounds bounds = characterCollider.bounds;
            Vector3 basePosition = bounds.center;
            basePosition.y = bounds.min.y + groundCheckOffsetY; // 바닥에서 offset 만큼 위로 올림

            // 2. Ray 발사 간격 계산 (Radius Multiplier 적용)
            float xOffset = bounds.extents.x * groundCheckRadiusMultiplier;
            float zOffset = bounds.extents.z * groundCheckRadiusMultiplier;

            // Ray의 시작 지점 설정
            Vector3[] rayOrigins = new Vector3[]
            {
                basePosition + new Vector3(xOffset, 0, zOffset), // 앞 오른쪽
                basePosition + new Vector3(-xOffset, 0, zOffset), // 앞 왼쪽
                basePosition + new Vector3(xOffset, 0, -zOffset), // 뒤 오른쪽
                basePosition + new Vector3(-xOffset, 0, -zOffset) // 뒤 왼쪽
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
                    return true; // 충돌이 감지되면 착지 상태
                }
            }

            // 4. 모든 Ray가 충돌하지 않을 경우 공중 상태로 간주
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            if (characterCollider == null) return;

            Gizmos.color = IsGrounded() ? Color.red : Color.green;

            // Bounds 및 Ray 시작 위치
            Bounds bounds = characterCollider.bounds;
            Vector3 basePosition = bounds.center;
            basePosition.y = bounds.min.y + groundCheckOffsetY;

            // Ray 발사 간격 계산 (Radius Multiplier 적용)
            float xOffset = bounds.extents.x * groundCheckRadiusMultiplier;
            float zOffset = bounds.extents.z * groundCheckRadiusMultiplier;

            // Ray의 시작 지점 설정 (시각화용)
            Vector3[] rayOrigins = new Vector3[]
            {
                basePosition + new Vector3(xOffset, 0, zOffset), // 앞 오른쪽
                basePosition + new Vector3(-xOffset, 0, zOffset), // 앞 왼쪽
                basePosition + new Vector3(xOffset, 0, -zOffset), // 뒤 오른쪽
                basePosition + new Vector3(-xOffset, 0, -zOffset) // 뒤 왼쪽
            };

            // Ray를 Gizmos로 표시
            foreach (var origin in rayOrigins)
            {
                Gizmos.DrawRay(origin, Vector3.down * groundCheckDistance);
            }
        }
    }
}