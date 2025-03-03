using System;
using Framework.Utilities;
using UnityEngine;

namespace Framework.Core
{
    /// <summary>
    /// 게임 입력을 통합 관리하는 매니저 클래스.
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        private PlayerInputActions _inputActions;

        // Input Data
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsDashing { get; private set; }
        public bool IsPaused { get; private set; }

        // Events for external systems
        public event Action<Vector2> OnMoveInput;
        public event Action<Vector2> OnLookInput;
        public event Action OnJumpPressed;
        public event Action OnDashPressed;
        public event Action<bool> OnPauseToggled;

        protected override void InitializeManager()
        {
            InitializeInputs();
        }
        

        private void InitializeInputs()
        {
            _inputActions = new PlayerInputActions();

            _inputActions.Player.Move.performed += ctx =>
            {
                MoveInput = ctx.ReadValue<Vector2>();
                OnMoveInput?.Invoke(MoveInput);
            };
            _inputActions.Player.Look.performed += ctx =>
            {
                LookInput = ctx.ReadValue<Vector2>();
                OnLookInput?.Invoke(LookInput);
            };
            _inputActions.Player.Jump.performed += _ =>
            {
                IsJumping = true;
                OnJumpPressed?.Invoke();
            };
            _inputActions.Player.Jump.canceled += _ => IsJumping = false;

            _inputActions.Player.Dash.performed += _ =>
            {
                IsDashing = true;
                OnDashPressed?.Invoke();
            };
            _inputActions.Player.Dash.canceled += _ => IsDashing = false;

            _inputActions.Player.Pause.started += _ =>
            {
                IsPaused = !IsPaused;
                OnPauseToggled?.Invoke(IsPaused);
            };
        }

        private void OnEnable() => _inputActions.Player.Enable();
        private void OnDisable() => _inputActions.Player.Disable();
    }
}