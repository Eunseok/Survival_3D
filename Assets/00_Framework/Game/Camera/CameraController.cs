using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")] [Tooltip("카메라가 따라가는 타겟 (플레이어 캐릭터)")]
    public Transform playerBody; // 플레이어 본체
    public Transform cameraContainer;

    [Tooltip("마우스 민감도")] public float lookSensitivity = 2f; // 마우스 민감도

    [Tooltip("위/아래 회전에 대한 제한(X축 피치 제한)")] 
    public float minXLook;
    public float maxXLook;

    private float currentPitch; // 현재 X축(Pitch)의 회전 값

    
    void Start()
    {
        currentPitch = cameraContainer.localEulerAngles.x;
        
        InputManager.Instance.OnLookInput += HandleMouseLook;
        
        // 마우스 커서를 숨기고 고정
        Cursor.lockState = CursorLockMode.Locked;
    }
    

    private void HandleMouseLook(Vector2 mouseLook)
    {
        currentPitch += mouseLook.y * lookSensitivity;
        currentPitch = Mathf.Clamp(currentPitch, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-currentPitch, 0, 0);
        
        playerBody.eulerAngles += new Vector3(0, mouseLook.x * lookSensitivity, 0);
    }
}