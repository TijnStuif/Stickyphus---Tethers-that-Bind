using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public bool isHoldingBall = false;
    
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.input.Player.Throw.WasPressedThisFrame())
        {
            Debug.Log("Left mouse was pressed!");
        }
    }

    private void ThrowBall()
    {
        if (isHoldingBall)
        {
            isHoldingBall = false;
            // Add logic to throw the ball here
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBall"))
        {
            isHoldingBall = true;
            collision.gameObject.SetActive(false);
        }
    }
}
