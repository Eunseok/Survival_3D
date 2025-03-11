## 유니티 심화 개인과제 (스파르타 던전 탐험 만들기)

---

### 필수 기능

1. **기본 이동 및 점프 Input System, Rigidbody ForceMode (난이도 : ★★☆☆☆)**

   - RigidBody

     ![Rigidbody이동](ReadMe/Rigidbody이동.gif)

     <details markdown="1"> <summary>코드스니펫</summary>

     ```csharp
     //PlayerMovement.cs

     //이동
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
     //점프
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
     ```

     </details>

   - CharacterController

     ![CharacterController이동](ReadMe/CharacterController이동.gif)

     <details markdown="1"> <summary>코드스니펫</summary>

     ```csharp
     //MovementController.cs

     //이동
     public void Move()
     {

         bool isSprinting = _input.sprint;

         // 간단한 가속 및 감속 시스템 (제거, 교체 또는 변경이 용이함)

         // 참고: Vector2의 == 연산자는 근사 비교를 사용하므로 부동소수점 오류에 안전하며, magnitude보다 성능이 우수함.
         // 입력이 없으면 목표 속도를 0으로 설정
         float targetSpeed = 0f;
         if (_input.move != Vector2.zero)
         {
             // 이동 속도를 설정 (걷기 속도 또는 질주 속도)
             targetSpeed = _statHandler.HandleMovementSpeed(isSprinting);
         }

         // 플레이어의 현재 수평 이동 속도를 가져옴
         float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

         float speedOffset = 0.1f;
         InputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

         // 목표 속도에 맞춰 가속 또는 감속
         if (currentHorizontalSpeed < targetSpeed - speedOffset ||
             currentHorizontalSpeed > targetSpeed + speedOffset)
         {
             // 선형 변화보다 부드러운 가속 곡선을 만들기 위해 Lerp 사용
             // Lerp의 T 값은 이미 0~1 범위로 제한되므로 추가적인 클램핑이 필요 없음
             _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * InputMagnitude,
                                 Time.deltaTime * SpeedChangeRate);

             // 속도를 소수점 3자리까지 반올림
             _speed = Mathf.Round(_speed * 1000f) / 1000f;
         }
         else
         {
             _speed = targetSpeed;
         }

         // 애니메이션 블렌딩 속도를 보간하여 부드럽게 변경
         AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
         if (AnimationBlend < 0.01f) AnimationBlend = 0f;

         // 입력 방향을 정규화 (magnitude가 1이 되도록)
         Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

         // 참고: Vector2의 != 연산자는 근사 비교를 사용하므로 부동소수점 오류에 안전하며, magnitude보다 성능이 우수함.
         // 이동 입력이 있을 경우 플레이어가 이동 방향을 바라보도록 회전
         if (_input.move != Vector2.zero)
         {
             _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                 _mainCamera.transform.eulerAngles.y;
             float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                                                    RotationSmoothTime);

             // 카메라 방향을 기준으로 이동 방향을 바라보도록 회전 적용
             transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
         }

         // 목표 이동 방향 설정 (카메라 방향 기준으로 변환)
         Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

         // 플레이어 이동 적용
         _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                          new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

     }

     //점프
     public void JumpAndGravity(bool grounded)
     {
         if (grounded) // 플레이어가 지면에 있는 경우
         {
             // 낙하 타이머 초기화
             _fallTimeoutDelta = FallTimeout;

             IsJumping = false;
             IsFreeFalling = false;


             // 지면에 있을 때, 중력이 무한히 누적되지 않도록 초기화
             if (_verticalVelocity < 0.0f)
             {
                 _verticalVelocity = -2f; // 살짝 음수 값을 줘서 지면에 붙어 있게 함
             }

             // 점프 처리
             if (_input.jump && _jumpTimeoutDelta <= 0.0f)
             {
                 // 점프 공식: H = 점프 높이, G = 중력 값
                 // H * -2 * G
                 // 필요한 초기 속도를 계산하여 점프 높이를 설정
                 _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                 IsJumping = true;
             }

             // 점프 쿨타임 처리
             if (_jumpTimeoutDelta >= 0.0f)
             {
                 _jumpTimeoutDelta -= Time.deltaTime;
             }
         }
         else // 공중에 있는 경우 (낙하 상태)
         {
             // 점프 타이머 초기화
             _jumpTimeoutDelta = JumpTimeout;


             // 낙하 타이머 처리
             if (_fallTimeoutDelta >= 0.0f)
             {
                 _fallTimeoutDelta -= Time.deltaTime;
             }
             else
             {
                 IsFreeFalling = true;
             }

             // 공중에 있을 때 점프 입력을 초기화 (더블 점프 방지)
             _input.jump = false;
         }

         // 중력 적용 (터미널 속도까지 증가)
         if (_verticalVelocity < _terminalVelocity)
         {
             _verticalVelocity += Gravity * Time.deltaTime;
         }
     }
     ```

     </details>

2. 체력바 UI (난이도 : ★★☆☆☆)

   ![HealthBar](ReadMe/HealthBar.gif)

     <details markdown="1"> <summary>코드스니펫</summary>

   ```csharp
   //UICondition.cs
   private void Start()
   {
       Player player = CharacterManager.Instance.Player;

       playerStatHandler.health.OnValueChangedUnityEvent += healthBar.SetCondition;// UI에서 OnValueChangedUnityEvent 구독
   }

   public void SetCondition(float curValue, float maxValue)
   {
       // maxValue가 0일 경우 fillAmount를 0으로 설정하여 나눗셈을 방지
       if (maxValue == 0)
       {
           imgProgress.fillAmount = 0;
           return;
       }

       imgProgress.fillAmount = Mathf.Clamp01(curValue/ maxValue); //Progress 이미지 Fill조정
   }

   //Condition.cs
   public void Apply(float amount)
   {
       curValue = Mathf.Clamp(curValue + amount, 0f, maxValue);

       OnValueChangedUnityEvent?.Invoke(curValue, maxValue); // 체력의 변화가 있을때 OnValueChangedUnityEvent호출
   }


   ```

   </details>

3. 동적 환경 조사 Raycast UI (난이도: ★★★☆☆)

   ![Interaction](ReadMe/Interaction.gif)

     <details markdown="1"> <summary>코드스니펫</summary>

   ```csharp
   //Interaction.cs

   //카메라 중심에서 Ray를 발사해 Interacterble 오브젝트 탐지
   private void CheckForInteractableObject()
   {
       // 카메라와 캐릭터 사이의 거리 기반으로 Ray 길이 조정
       float zoomAdjustedDistance = (_mainCamera.transform.position - transform.position).magnitude + maxCheckDistance;


       var screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f);
       var ray = _mainCamera.ScreenPointToRay(screenCenterPoint);

       if (Physics.Raycast(ray, out var hit, zoomAdjustedDistance, layerMask))
       {
           if (hit.collider.gameObject != _currentInteractableObject)
           {
               _currentInteractableObject = hit.collider.gameObject;
               _currentInteractable = hit.collider.GetComponent<IInteractable>();
               UpdatePromptText();
           }
       }
       else
       {
           ClearInteractionData();
       }
   }

   //프롬프트 텍스트를 Interacterble 오브젝트의 GetInteractPrompt()로 변경
   private void UpdatePromptText()
   {
       if (_currentInteractable != null && promptText)
       {
           promptText.gameObject.SetActive(true);
           promptText.text = _currentInteractable.GetInteractPrompt();
       }
   }
   ```

   </details>

4. 점프대 Rigidbody ForceMode (난이도 : ★★★☆☆)

   ![Trampoline](ReadMe/Trampoline.gif)

   <details markdown="1"> <summary>코드스니펫</summary>

   ```csharp
   //Trampoline.cs
   private void OnTriggerEnter(Collider other) //CharacterContoller는 Rigidbody가 없어 OnTrigger로 대체
   {
       if (!other.CompareTag("Player")) return;

       // Ray를 캐릭터 아래로 발사
       Ray ray = new Ray(other.bounds.center - new Vector3(0, other.bounds.extents.y, 0), Vector3.down);

       // 충돌 결과
       if (Physics.Raycast(ray, out RaycastHit hit, 1f))
       {
           if (hit.collider.gameObject == this.gameObject)
           {
               Debug.Log("위에서 트리거에 닿음");
               // 충돌한 객체가 플레이어 태그인지 확인
               if (!other.gameObject.CompareTag("Player")) return;

               // Rigidbody 가져오기
               if (!other.transform.TryGetComponent<MovementController>(out var movement)) return;

               movement.ApplytForceY(-jumpForce);


               // Bounce 애니메이션 실행
               if(_bounceCoroutine != null)
                   StopCoroutine(_bounceCoroutine);

               //애니메이션 코루틴 시작
               _bounceCoroutine = StartCoroutine(BounceAnimation());

               // 사운드 효과 재생
               SoundManager.PlaySFX(jumpSound);
           }
       }
       else
       {
           Debug.Log("아래에서 트리거에 닿음");
       }
   }

   //MovementController.cs
   public void ApplytForceY(float force)
   {
       _verticalVelocity += force;
       _input.jump = true;
   }
   ```

   </details>

5. 아이템 데이터 ScriptableObject (난이도 : ★★★☆☆)
   <img width="179" alt="image" src="https://github.com/user-attachments/assets/05cb7ba6-bb48-4b44-b175-51f01c9b04c1" />

     <details markdown="1"> <summary>코드스니펫</summary>

   ```csharp
   //ItemData.cs
   // Common/BaseItemData.cs

   using UnityEngine;

   namespace Scripts.Items
   {
       public enum ItemType
       {
           Etc,
           Equip,
           Use
       }

       public interface IStackable
       {
           int MaxStackAmount { get; }
       }


       public abstract class ItemData : ScriptableObject
       {

           protected const int DefaultMaxStackAmount = 100;

           [Header("Info")]
           [SerializeField] private string itemName;
           public string ItemName => itemName;

           [SerializeField] private string itemDescription;
           public string ItemDescription => itemDescription;

           [SerializeField] private Sprite icon;
           public Sprite Icon => icon;

           [SerializeField] private GameObject dropPrefab;
           public GameObject DropPrefab => dropPrefab;

           public abstract ItemType Type { get; }
       }

   }
   //Equip/ItemDataEquip.cs

   using UnityEngine;

   namespace Scripts.Items
   {
       [CreateAssetMenu(fileName = "New UEquipItem", menuName = "Items/Equip")]
       public class EquipItemData : ItemData
       {
           public override ItemType Type => ItemType.Equip;

           [Header("Equip")]
           public GameObject equipPrefab;
       }
   }

   //Etc/ItemDataEtc.cs
   using UnityEngine;

   namespace Scripts.Items
   {
       [CreateAssetMenu(fileName = "New UEtcItem", menuName = "Items/Etc")]
       public class EtcItemData : ItemData, IStackable
       {
           [Header("Stacking")]
           [SerializeField] private int maxStackAmount = DefaultMaxStackAmount;
           public int MaxStackAmount => maxStackAmount;

           public override ItemType Type => ItemType.Etc;
       }
   }

   // Use/ItemDataUse.cs
   using System;
   using UnityEngine;

   namespace Scripts.Items
   {
       public enum ConsumableType
       {
           Hunger,
           Health,
           Speed,
       }

       [Serializable]
       public class ItemDataConsumable
       {
           public ConsumableType type;
           public float value;
           public float duration;
       }

       [CreateAssetMenu(fileName = "New UseItem", menuName = "Items/Use")]
       public class UseItemData : ItemData, IStackable
       {
           [Header("Consumable")]
           [SerializeField] private ItemDataConsumable[] consumables;
           public ItemDataConsumable[] Consumables => consumables;

           [Header("Stacking")]
           [SerializeField] private int maxStackAmount = DefaultMaxStackAmount;
           public int MaxStackAmount => maxStackAmount;

           public override ItemType Type => ItemType.Use;
       }
   }
   ```

   </details>

6. 아이템 사용 Coroutine (난이도 : ★★★☆☆)

   ![UseItem](ReadMe/UseItem.gif)

    <details markdown="1"> <summary>코드스니펫</summary>

   ```csharp
   //UseItemStrategyUI.cs
   public void ConfigureButtonAction(Button useButton, ItemSlotData slotData)
   {
       useButton.onClick.AddListener(() =>
       {
           var useItemData = slotData.Item as UseItemData;
           if (useItemData != null)
               foreach (var effect in useItemData.Consumables)
               {
                   switch (effect.type)
                   {
                       case ConsumableType.Health:
                           SignalManager.Instance.EmitSignal("OnPlayerHeal", effect.value);
                           break;
                       case ConsumableType.Hunger:
                           SignalManager.Instance.EmitSignal("OnPlayerEat", effect.value);
                           break;
                       case ConsumableType.Speed:
                           //스피드물약 사용시 Action<T1, T2>형태의 신호를 호출
                           SignalManager.Instance.EmitSignal("OnApplySpeedBuff", effect.value, effect.duration);
                           break;
                   }
               }
           slotData.RemoveSelectedItem();
       });
   }

   //ResourceHandler.cs
   private void Awake()
   {
       //OnApplySpeedBuff 구독
       SignalManager.Instance.ConnectSignal<float, float>("OnApplySpeedBuff", ApplySpeedBuff);
   }

   private void ApplySpeedBuff(float boostAmount, float duration)
   {
       if (!_isSpeedBoosted)
       {
           StartCoroutine(SpeedBuffRoutine(boostAmount, duration)); //속도 증가 코루틴 실행
       }
   }

   private IEnumerator SpeedBuffRoutine(float boostAmount, float duration)
   {
       _isSpeedBoosted = true;
       _statHandler.MoveSpeed += boostAmount; // 속도 증가
       _statHandler.SprintSpeed += boostAmount; // 대시속도 증가
       yield return new WaitForSeconds(duration); // 버프 지속 시간 대기
       _statHandler.MoveSpeed -= boostAmount; // 원래 속도로 복구
       _statHandler.SprintSpeed -= boostAmount;
       _isSpeedBoosted = false;
   }


   ```

   </details>

## 도전 기능

1. 추가 UI (난이도 : ★★☆☆☆) (스태미나)

   ![Stamina](https://github.com/user-attachments/assets/550ac33a-b88a-4fc8-ab8a-6707dc3aa06a)

    <details markdown="1"> <summary>코드스니펫</summary>

   ```csharp
   //ResourceHandler.cs

   SignalManager.Instance.ConnectSignal<float, bool>("OnUseStamina", UseStamina);//OnUseStamina구독

   private bool UseStamina(float amount)
   {
       if(_statHandler.stamina.CurValue - amount < 0)
       {
           return false;
       }
       _statHandler.stamina.Apply(-amount);
       return true;
   }

   //MovementController.cs
   public void Move()
   {
       targetSpeed = _statHandler.HandleMovementSpeed(isSprinting); //_statHandler에서 속도를 받아옴
   }

   //StatHandler.cs
   public float HandleMovementSpeed(bool isSprinting)
   {
       if (!isSprinting) return MoveSpeed;

       bool canDash = SignalManager.Instance.EmitSignal<float, bool>( //Func<T, TResult> 형태로 신호를 호출
           "OnUseStamina", SprintStaminaCost * Time.deltaTime);	 // 사용 가능한 스태미나가 있을 시 질주 시작
       if (canDash)
           return SprintSpeed;

       return MoveSpeed;
   }

   //UI는 체력바랑 동일
   ```

   </details>

2. 3인칭 시점 (난이도 : ★★★☆☆)

   ​ ![ThirdCamera](ReadMe/ThirdCamera.gif)

   - 기본 카메라

        <details markdown="1"> <summary>코드스니펫</summary>

     ```csharp

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

             // 카메라 위치 업데이트
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

     ```

     </details>

   - Cinemachine 활용

        <details markdown="1"> <summary>코드스니펫</summary>

     ```csharp
     using DefaultNamespace;
     using UnityEngine;


     public class ThirdPersonCameraController : MonoBehaviour
     {
         [Header("Cinemachine 카메라 설정")]
         [Tooltip("Cinemachine Virtual Camera가 따라갈 대상 오브젝트")]
         public GameObject CinemachineCameraTarget;

         [Tooltip("카메라가 위쪽으로 이동할 수 있는 최대 각도")]
         public float TopClamp = 70.0f;

         [Tooltip("카메라가 아래쪽으로 이동할 수 있는 최대 각도")]
         public float BottomClamp = -30.0f;

         [Tooltip("카메라의 기본 회전값을 보정하는 추가 각도 (고정된 시점에서 미세 조정 가능)")]
         public float CameraAngleOverride = 0.0f;

         [Tooltip("카메라 회전 속도")]
         public float DeltaTimeMultiplier = 1f;

         [Tooltip("카메라의 위치를 모든 축에서 고정할지 여부")] public bool LockCameraPosition = false;

         // Cinemachine 관련 변수
         private float _cinemachineTargetYaw;
         private float _cinemachineTargetPitch;


         private const float _threshold = 0.01f; // 작은 입력값 무시하는 임계값


         private void Start()
         {
             // 게임 시작 시 카메라의 초기 (좌우 회전)Yaw 값을 가져와서 설정
             _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
         }


         public void CameraRotation(Vector2 look)
         {
             // 입력 값이 존재하고, 카메라 위치가 고정되지 않은 경우 실행
             if (look.sqrMagnitude >= _threshold && !LockCameraPosition)
             {

                 // 입력된 값(look)을 기반으로 카메라의 Yaw(좌우 회전) 및 Pitch(상하 회전) 값을 업데이트
                 _cinemachineTargetYaw += look.x * DeltaTimeMultiplier;
                 _cinemachineTargetPitch -= look.y * DeltaTimeMultiplier;
             }

             // 회전 값이 일정 범위를 넘지 않도록 제한 (Yaw는 제한 없이 회전 가능, Pitch는 위/아래 범위 설정)
             _cinemachineTargetYaw = Util.ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
             _cinemachineTargetPitch = Util.ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

             // Cinemachine이 따라갈 대상 오브젝트(CinemachineCameraTarget)의 회전을 설정
             CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
                 _cinemachineTargetPitch + CameraAngleOverride, // 상하 회전 (Pitch)
                 _cinemachineTargetYaw,                         // 좌우 회전 (Yaw)
                 0.0f                                           // Z축 회전은 없음
             );
         }
     }
     ```

     </details>

3. 움직이는 플랫폼 구현 (난이도 : ★★★☆☆)

   ![MovingPlatform](ReadMe/MovingPlatform.gif)

    <details markdown="1"> <summary>코드스니펫</summary>

   ```csharp
   //MovingPlatform.cs
   private void MovePlatform()
   {
       // 현재 위치에서 목표 지점으로 이동
       transform.position = Vector3.MoveTowards(transform.position, _targetPoint, speed * Time.deltaTime);

       // 목표 지점에 도달하면 방향 변경
       if (!(Vector3.Distance(transform.position, _targetPoint) < 0.1f)) return;
       _targetPoint = _movingToB ? pointA.position : pointB.position;
       _movingToB = !_movingToB; // 방향 전환
   }

   private void OnTriggerEnter(Collider other)
   {
       if (!other.CompareTag("Player")) return;

       // Ray를 캐릭터 아래로 발사
       Ray ray = new Ray(other.bounds.center - new Vector3(0, other.bounds.extents.y, 0), Vector3.down);

       // 충돌 결과
       if (Physics.Raycast(ray, out RaycastHit hit, 1f))
       {
           if (hit.collider.gameObject == this.gameObject)
           {
               other.transform.SetParent(transform);
               Debug.Log("위에서 트리거에 닿음");
           }
       }
       else
       {
           Debug.Log("아래에서 트리거에 닿음");
       }
   }


   private void OnTriggerExit(Collider other)
   {
       if (other.CompareTag("Player"))
       {
           other.transform.SetParent(null); // 플랫폼에서 내려올 때 자식 관계 해제
       }
   }
   ```

   </details>

4. 벽 타기 및 매달리기 (난이도 : ★★★★☆)

   ![Climb](https://github.com/user-attachments/assets/29215ec1-e92a-4066-a4e3-ae213c340a55)

    <details markdown="1"> <summary>코드스니펫</summary>

   ```csharp
   //Characterbody.cs

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
               if (wallAngle is >= 45f and <= 135f) // 벽 조건: 법선이 Vector3.up과 45~135인 경우
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


   //PlayerMovent.cs

   private void HandleClimbing()
   {
       if (!CanClimb()) //벽에 붙어있는지 체크
           return;

       UseStaminaOrStopClimbing(_climbStaminaCostPerSecond); // 스테미나가 있는지 체크

       Vector3 wallNormal = _characterBody.WallHitNormal; //노말을 뒤집어 벽을 향하는 벡터를 구함



       Vector3 rightVector = Vector3.Cross(Vector3.up, wallNormal).normalized; //내적을 통해 벽의 오른쪽 벡터를 구함
       Vector3 climbAxis = Vector3.Cross(wallNormal, rightVector).normalized; //구해진 오른쪽벡터와 벽 벡터와 내적을 구해 수직 축을 구함

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
   ```
