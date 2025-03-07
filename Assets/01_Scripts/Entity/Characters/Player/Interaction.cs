using _01_Scripts.UI;
using Scripts.Items;
using Scripts.UI;
using TMPro;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    public float maxCheckDistance;
    public LayerMask layerMask;
    public TextMeshProUGUI promptText;
    
    public float height = 1.7f; // 테스트 머리 위치

    private float _lastCheckTime;
    private GameObject _currentInteractableObject;
    private IInteractable _currentInteractable;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        InputManager.Instance.OnInteractionPressed += OnInteractPressed;
        promptText = UIManager.Instance.HudUI.GetTextFromGameObject((int)HUDGame.HudType.PromptText);
    }

    void Update()
    {
        if (Time.time - _lastCheckTime > checkRate)
        {
            _lastCheckTime = Time.time;
            CheckForInteractableObject();
        }
    }

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


    private void UpdatePromptText()
    {
        if (_currentInteractable != null && promptText)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = _currentInteractable.GetInteractPrompt();
        }
    }

    private void ClearInteractionData()
    {
        _currentInteractableObject = null;
        _currentInteractable = null;
        if (promptText)
            promptText.gameObject.SetActive(false);
    }

    private void OnInteractPressed()
    {
        if (_currentInteractable == null) return;

        _currentInteractable.OnInteract();
        ClearInteractionData();
    }
}