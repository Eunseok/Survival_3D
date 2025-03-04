using System;
using Framework.Utilities;
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

    
    protected override void InitializeManager()
    {
        InitializeInputs();
    }

    private void InitializeInputs()
    {
        _playerInput = new PlayerInputActions();

        _playerInput.Player.Move.performed += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
        _playerInput.Player.Move.canceled += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
        _playerInput.Player.Look.performed += ctx => OnLookInput?.Invoke(ctx.ReadValue<Vector2>());
        _playerInput.Player.Jump.started +=_ => OnJumpPressed?.Invoke();
        _playerInput.Player.Interaction.started += _ => OnInteractionPressed?.Invoke();

    }

    private void OnEnable()
    {
        _playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Disable();
    }

}