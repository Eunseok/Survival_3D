using Managers;
using UnityEngine;

namespace Framework.Characters
{
    public class ClimbingMovementController : MovementController
    {
        // --- Climbing 관련 변수 ---
        [Header("Climbing Settings")]
        [Tooltip("벽에 평행한 정렬 임계값")]
        public float AlignmentThreshold = 0.8f;
        [Tooltip("클라밍 시작까지 소요 시간")]
        public float RequiredClimbTime = 0.5f;

        public float ClimbStaminaCost = 20f;

        private bool _isClimbing;
        private float _climbTimer;

        private StatHandler _statHandler;

        /// <summary>
        /// 클라밍 관련 초기화
        /// </summary>
        public override void  Initialize(InputHandler input, GameObject mainCamera, StatHandler statHandler)
        {
            base.Initialize(input, mainCamera, statHandler);
            _statHandler = statHandler;
        }

        public override void Move()
        {
            
        }

        public void MoveWithClimbing(bool grounded, bool isOnWall, Vector3 wallNormal)
        {
            if (_isClimbing)
            {
                HandleClimbing(isOnWall, wallNormal);
            }
            else
            {
                AttachToWallIfPossible(isOnWall, wallNormal);
                base.Move();
            }

            JumpAndGravity(grounded);
        }

        /// <summary>
        /// 클라밍 상태 처리
        /// </summary>
        private void HandleClimbing(bool isOnWall, Vector3 wallNormal)
        {
            if (!CanClimb(isOnWall))
                return;

            UseStaminaOrStopClimbing();

            // 벽 위로 이동 계산
            Vector3 climbDirection = CalculateClimbDirection(wallNormal);
            MoveAndRotateForClimbing(climbDirection, wallNormal);
        }

        /// <summary>
        /// 클라밍 가능 여부 판정
        /// </summary>
        private bool CanClimb(bool isOnWall)
        {
            if (!isOnWall || !_isClimbing)
            {
                ResetClimbingState();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 클라밍 중 스태미나 소모
        /// </summary>
        private void UseStaminaOrStopClimbing()
        {
            bool hasStamina = SignalManager.Instance.EmitSignal<float, bool>("OnUseStamina", ClimbStaminaCost * Time.deltaTime);
            if (!hasStamina)
                ResetClimbingState();
        }

        /// <summary>
        /// 벽에 붙을 수 있는지 평가 및 처리
        /// </summary>
        private void AttachToWallIfPossible(bool isOnWall, Vector3 wallNormal)
        {
            if (isOnWall && !_isClimbing && _input.move != Vector2.zero)
            {
                float alignment = Vector3.Dot(transform.forward, -wallNormal);

                if (alignment >= AlignmentThreshold)
                {
                    _climbTimer += Time.deltaTime;
                    if (_climbTimer >= RequiredClimbTime)
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

        /// <summary>
        /// 클라밍 시작 처리
        /// </summary>
        private void StartClimbing()
        {
            _isClimbing = true;
            _verticalVelocity = 0f;
        }

        /// <summary>
        /// 클라밍 이동 및 회전 처리
        /// </summary>
        private void MoveAndRotateForClimbing(Vector3 direction, Vector3 wallNormal)
        {
            if (IsFreeFalling && Vector3.Dot(Vector3.down, direction) > 0f)
            {
                ResetClimbingState();
                return;
            }

            // 벽의 법선 벡터 활용하여 벽 방향 조정
            Vector3 wallForward = -wallNormal;
            _controller.Move(direction.normalized * _statHandler.ClimbSpeed * Time.deltaTime);
            SmoothlyRotateTowards(wallForward);
        }

        /// <summary>
        /// 벽을 기준으로 이동 방향 계산
        /// </summary>
        private Vector3 CalculateClimbDirection(Vector3 wallNormal)
        {
            return Vector3.Cross(wallNormal, Vector3.Cross(Vector3.up, wallNormal)).normalized * _input.move.y -
                   Vector3.Cross(Vector3.up, wallNormal).normalized * _input.move.x;
        }

        /// <summary>
        /// 지정된 방향으로 부드럽게 회전
        /// </summary>
        private void SmoothlyRotateTowards(Vector3 lookDirection)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSmoothTime * Time.deltaTime);
        }

        /// <summary>
        /// 클라밍 상태 초기화
        /// </summary>
        private void ResetClimbingState()
        {
            _isClimbing = false;
            _verticalVelocity = 0f;
        }

        /// <summary>
        /// 점프 동작 (클라밍 중 점프 포함)
        /// </summary>
        public void JumpWithClimbing(Vector3 wallNormal)
        {
            if (_isClimbing)
            {
                Vector3 jumpDirection = (wallNormal * 2f + Vector3.up).normalized;
                ApplytForceY(_statHandler.JumpForce);
                ResetClimbingState();
            }
            else
            {
                ApplytForceY(_statHandler.JumpForce);
            }
        }
    }
}