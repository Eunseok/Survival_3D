using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerInputActions _playerInput;

    public InputActionMap Player => _playerInput.Player;

    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2> OnLookInput;
    public event Action OnInteractionPressed;
    public event Action OnJumpPressed;
    public event Action OnInventoryPressed;
    public event Action OnAttackPressed;


    protected  void Awake()
    {
        InitializeInputs();
    }

    private void InitializeInputs()
    {
        _playerInput = new PlayerInputActions();

        _playerInput.Player.Move.performed += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
        _playerInput.Player.Move.canceled += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
        _playerInput.Player.Look.performed += ctx => OnLookInput?.Invoke(ctx.ReadValue<Vector2>());
        _playerInput.Player.Jump.started += _ => OnJumpPressed?.Invoke();
        _playerInput.Player.Interaction.started += _ => OnInteractionPressed?.Invoke();
        _playerInput.Player.Attack.started += _ => OnAttackPressed?.Invoke();
        
        
        _playerInput.Shorcut.Inventory.started += _ =>
        {
            OnInventoryPressed?.Invoke();
            ToggleCursor();
        };
    }

    private void OnEnable()
    {
        _playerInput.Player.Enable();
        _playerInput.Shorcut.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Disable();
        _playerInput.Shorcut.Disable();
    }

    void ToggleCursor()
    {
        if (IsCursorLocked())
        {
            DisableCursorInteraction();
        }
        else
        {
            EnableCursorInteraction();
        }
    }

    private bool IsCursorLocked() => Cursor.lockState == CursorLockMode.Locked;

    private void EnableCursorInteraction()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerInput.Player.Enable();
    }

    private void DisableCursorInteraction()
    {
        Cursor.lockState = CursorLockMode.None;
        _playerInput.Player.Disable();
    }

    
}