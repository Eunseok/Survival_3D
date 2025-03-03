using Framework.Characters;
using UnityEngine;

[ RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private CharacterBody _characterBody;
    private StatHandler statHandler;

    private Vector2 moveDirection;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _characterBody = GetComponent<CharacterBody>();
        statHandler = GetComponent<StatHandler>();
        
        InputManager.Instance.OnMoveInput += ctx => moveDirection = ctx;
        InputManager.Instance.OnJumpPressed += Jump;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 direction = transform.forward * moveDirection.y + transform.right * moveDirection.x;
        direction *= statHandler.Speed;
        direction.y = _rigidbody.velocity.y;

        _rigidbody.velocity = direction;
    }

    private void Jump()
    {
        if (_characterBody && _characterBody.IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * statHandler.JumpForce, ForceMode.Impulse);
        }
    }
}