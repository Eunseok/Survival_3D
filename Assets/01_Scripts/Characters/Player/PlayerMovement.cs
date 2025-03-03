using Framework.Characters;
using UnityEngine;

[ RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;

    private Rigidbody _rigidbody;
    private CharacterBody _characterBody;

    private Vector2 moveDirection;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _characterBody = GetComponent<CharacterBody>(); 

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
        direction *= moveSpeed;
        direction.y = _rigidbody.velocity.y;

        _rigidbody.velocity = direction;
    }

    private void Jump()
    {
        if (_characterBody && _characterBody.IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
}