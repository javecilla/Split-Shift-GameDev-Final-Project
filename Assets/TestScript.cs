using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float movementSpeed = 10f;
    public float jumpForce = 7.5f;

    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float dashDuration = 1.25f;
    public float dashCooldown = 0.5f;

    private bool isGrounded = true;
    private bool canDoubleJump = false;
    private bool isDashing = false;
    private float dashTimeLeft;
    private float lastDashTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        // Prevent normal movement during dash
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveHorizontal * movementSpeed, rb.linearVelocity.y);
        }

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false;
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                canDoubleJump = false;
            }
        }

        // Dash input (Right Mouse Click)
        if (Input.GetMouseButtonDown(1))
        {
            TryDash(moveHorizontal);
        }

        HandleDash();
    }

    private void TryDash(float moveHorizontal)
    {
        // Only dash if cooldown passed and not already dashing
        if (Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            isDashing = true;
            dashTimeLeft = dashDuration;
            lastDashTime = Time.time;

            // Determine dash direction
            float dashDirection = moveHorizontal != 0 ? Mathf.Sign(moveHorizontal) : transform.localScale.x;

            rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);
        }
    }

    private void HandleDash()
    {
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;

            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        canDoubleJump = false;
    }
}
