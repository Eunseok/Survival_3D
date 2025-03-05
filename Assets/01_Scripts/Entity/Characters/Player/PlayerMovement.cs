using Framework.Characters;
using UnityEngine;

[ RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private CharacterBody _characterBody;
    private StatHandler _statHandler;

    private Vector2 _moveDirection;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _characterBody = GetComponent<CharacterBody>();
        _statHandler = GetComponent<StatHandler>();
        
        InputManager.Instance.OnMoveInput += ctx => _moveDirection = ctx;
        InputManager.Instance.OnJumpPressed += Jump;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = transform.forward * _moveDirection.y + transform.right * _moveDirection.x;
        direction *= _statHandler.Speed;
        direction.y = _rigidbody.velocity.y;

        _rigidbody.velocity = direction;
    }

    private void Jump()
    {
        if (_characterBody && _characterBody.IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * _statHandler.JumpForce, ForceMode.Impulse);
        }
    }
}