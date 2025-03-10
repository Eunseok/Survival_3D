using UnityEngine;

namespace Framework.Characters
{
    public class WallChecker : MonoBehaviour
    {
        public bool IsOnWall { get; private set; } // 캐릭터가 벽에 붙어 있는지 여부
        public Vector3 WallHitNormal { get; private set; } // 벽의 법선 벡터

        // 충돌이 발생했을 때 호출 (CharacterController용)
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // 충돌한 표면의 normal을 기준으로 벽 체크
            float wallAngle = Vector3.Angle(hit.normal, Vector3.up);

            // 벽의 각도와 충돌 물체를 기준으로 벽 상태 판단 (예: 경사가 높고 플레이어 전방일 경우)
            if (wallAngle > 45f && wallAngle < 135f) // 특정 각도 이상이 벽으로 판단 (75 - 100도)
            {
                IsOnWall = true; // 벽 상태 true
                WallHitNormal = hit.normal; // 벽의 normal 저장
            }
            else
            {
                IsOnWall = false; // 벽 상태 false
            }
        }

        // Gizmos로 확인 (디버깅 용도)
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f); // 지면일 때 초록색
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f); // 공중일 때 빨간색

            Gizmos.color = IsOnWall ? transparentGreen : transparentRed;

            Gizmos.DrawRay(transform.position, transform.forward*10f);
        }
    }
}