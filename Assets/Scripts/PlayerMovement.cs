using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float ballSlowModifier = 0.7f;
    private bool isGrounded;
    private float movementMultiplier = 1f;

    private Vector2 moveInput;

    private Rigidbody2D rb;
    public PlayerInputActions input;
    private PlayerBehaviour playerBehaviour;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = new PlayerInputActions();
        playerBehaviour = GetComponent<PlayerBehaviour>();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        input.Player.Jump.performed -= Jump;

        input.Disable();
    }

    private void Update()
    {
        movementMultiplier = !playerBehaviour.isHoldingBall ? 1f : ballSlowModifier;
        moveInput = input.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.AddForce(new Vector2(
            moveInput.x * moveSpeed * movementMultiplier,
            rb.linearVelocity.y
        ));
    }

    private void Jump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!isGrounded)
            return;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce * movementMultiplier
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
