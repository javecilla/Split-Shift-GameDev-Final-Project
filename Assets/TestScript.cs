using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float movementSpeed = 10f;
    public float jumpForce = 7.5f;
    private bool isGrounded = true;
    private bool canDoubleJump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(moveHorizontal * movementSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                //first jump (from ground)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false;
                canDoubleJump = true; //enable the double jump
            }
            else if (canDoubleJump)
            {
                //second jump (in air)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false; //disable further double jumps until we land again
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        canDoubleJump = false;
    }
}