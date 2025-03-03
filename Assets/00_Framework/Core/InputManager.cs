using System;
using Framework.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerInputActions playerInput;

    InputActionMap basic;

    public event Action<Vector2> OnMoveInput;
    public event Action<Vector2> OnLookInput;
    public event Action OnJumpPressed;

    
    protected override void InitializeManager()
    {
        InitializeInputs();
    }

    private void InitializeInputs()
    {
        playerInput = new PlayerInputActions();

        playerInput.Player.Move.performed += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
        playerInput.Player.Move.canceled += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
        playerInput.Player.Look.performed += ctx => OnLookInput?.Invoke(ctx.ReadValue<Vector2>());
        playerInput.Player.Jump.started +=_ => OnJumpPressed?.Invoke();

    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
    }
}