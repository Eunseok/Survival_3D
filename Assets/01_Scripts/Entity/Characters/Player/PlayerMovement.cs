using Framework.Characters;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float staminaPerSecForDash = 20f;
    
    private Rigidbody _rigidbody;
    private CharacterBody _characterBody;
    private StatHandler _statHandler;

    private Vector2 _moveDirection;

    private bool _isDashing;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _characterBody = GetComponent<CharacterBody>();
        _statHandler = GetComponent<StatHandler>();

        InputManager.Instance.OnMoveInput += ctx => _moveDirection = ctx;
        InputManager.Instance.OnJumpPressed += Jump;
        InputManager.Instance.OnDashInput += OnDashInput;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = transform.forward * _moveDirection.y + transform.right * _moveDirection.x;
        float moveSpeed = _statHandler.Speed;

        if (_isDashing && _characterBody.IsMoving())
        {
            bool canDash = SignalManager.Instance.EmitSignal<float, bool>("OnUseStamina", staminaPerSecForDash * Time.deltaTime);
            if (canDash)
                moveSpeed = _statHandler.DashSpeed;
        }

        direction *= moveSpeed;
        direction.y = _rigidbody.velocity.y;
        _rigidbody.velocity = direction;
    }


    private void OnDashInput(bool isDashing)
    {
        _isDashing = isDashing;
    }

    private void Jump()
    {
        if (_characterBody && _characterBody.IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * _statHandler.JumpForce, ForceMode.Impulse);
        }
    }
}