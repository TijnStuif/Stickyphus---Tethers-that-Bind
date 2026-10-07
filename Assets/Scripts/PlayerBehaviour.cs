using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    [System.NonSerialized] public bool isHoldingBall = false;
    [SerializeField] private float ropeStiffness = 500f;
    public float ballThrowForce = 10f;
    public float ballPickupDelay = 1f;
    public float ropeLength = 1f;

    private Vector3 throwOffset = new Vector3(0, .5f, 0);
    private float ballPickupTimer = 0f;
    private GameObject heldBall;
    
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRb;
    [SerializeField] private Rigidbody2D ballRb;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        ballThrowForce *= ballRb.mass;
        Debug.Log("Ball throw force: " + ballThrowForce);
    }

    private void Update()
    {
        if (playerMovement.input.Player.Throw.WasPressedThisFrame())
        {
            ThrowBall();
        }

        if (!isHoldingBall && ballPickupTimer <= ballPickupDelay)
        {
            ballPickupTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        ApplyRopeForce();
    }

    private void ThrowBall()
    {
        if (isHoldingBall)
        {
            ballPickupTimer = 0f;
            isHoldingBall = false;
            heldBall.transform.position = transform.position + throwOffset;

            heldBall.SetActive(true);

            ballRb.linearVelocity = Vector2.zero;
            
            ballRb.AddForce(CheckThrowingDiagonal() * ballThrowForce, ForceMode2D.Impulse);

            StartCoroutine(DisableBallCollision());
        }
    }

    private void ApplyRopeForce()
    {
        Vector2 playerPosition = playerRb.position;
        Vector2 ballPosition = ballRb.position;

        Vector2 difference = ballPosition - playerPosition;

        float distance = difference.magnitude;

        if (distance <= ropeLength)
            return;

        Vector2 direction = difference.normalized;

        float stretch = (distance - ropeLength) * ropeStiffness;

        playerRb.AddForce(direction * stretch);
        ballRb.AddForce(-direction * stretch);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBall"))
        {
            if (ballPickupTimer < ballPickupDelay) return;
            isHoldingBall = true;
            collision.gameObject.SetActive(false);
            heldBall = collision.gameObject;
            ballRb = heldBall.GetComponent<Rigidbody2D>();
        }
    }

    private Vector2 CheckThrowingDiagonal()
    {
        Vector2 mouseworldPosition = Camera.main.ScreenToWorldPoint(
            playerMovement.input.Player.Look.ReadValue<Vector2>());
        Vector2 direction = 
            (mouseworldPosition - (Vector2)transform.position).normalized;
        return direction;
    }

    private IEnumerator DisableBallCollision()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Collider2D ballCollider = heldBall.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(playerCollider, ballCollider, true);

        yield return new WaitForSeconds(ballPickupDelay);

        Physics2D.IgnoreCollision(playerCollider, ballCollider, false);
    }
}
