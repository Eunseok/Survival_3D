using Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float checkRate = 0.05f;
    private float _lastCheckTime;
    public float maxCheckDistance;
    public LayerMask layerMask;

    public GameObject curInteractGameObject;
    private IInteractable _curInteractable;

    public TextMeshProUGUI promptText;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        InputManager.Instance.OnInteractionPressed += OnInteractPressed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _lastCheckTime > checkRate)
        {
            _lastCheckTime = Time.time;

            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    curInteractGameObject = hit.collider.gameObject;
                    _curInteractable = hit.collider.GetComponent<IInteractable>();
                    SetPromptText();
                }
            }
            else
            {
                curInteractGameObject = null;
                _curInteractable = null;
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        promptText.gameObject.SetActive(true);
        promptText.text = _curInteractable.GetInteractPrompt();
    }

    public void OnInteractPressed()
    {
        if (_curInteractable == null) return;

        _curInteractable.OnInteract();
        curInteractGameObject = null;
        _curInteractable = null;
        promptText.gameObject.SetActive(false);
    }
}