using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    [System.NonSerialized] public bool hasBall = false;
    [SerializeField] private float ropeStiffness = 10f;
    public float ballThrowForce = 10f;
    public float ballPickupDelay = 1f;
    public float ropeLength = 1f;
    private float ballPickupTimer = 0f;
    private GameObject heldBall;
    private Vector3 playerPullAdjustment = new Vector3(1f, 0.8f, 1f);
    
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

        if (!hasBall && ballPickupTimer <= ballPickupDelay)
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
        if (hasBall)
        {
            ballPickupTimer = 0f;
            hasBall = false;
            heldBall.transform.position = transform.position;

            heldBall.SetActive(true);

            ballRb.linearVelocity = Vector2.zero;
            
            ballRb.AddForce(CheckThrowingDiagonal() * ballThrowForce, ForceMode2D.Impulse);

            StartCoroutine(DisableBallCollision());
        }
    }

    private void ApplyRopeForce()
    {
        if (hasBall) return;

        Vector2 difference = ballRb.position - playerRb.position;
        float distance = difference.magnitude;

        if (distance <= ropeLength)
            return;

        Vector2 direction = difference.normalized;

        float stretch = distance - ropeLength;
        float force = stretch * ropeStiffness;

        playerRb.AddForce(direction * force * playerPullAdjustment);
        ballRb.AddForce(-direction * force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBall"))
        {
            if (ballPickupTimer < ballPickupDelay) return;
            hasBall = true;
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
