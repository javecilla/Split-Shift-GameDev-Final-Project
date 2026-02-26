using System;
using UnityEngine;
using UnityEngine.Windows;

public class TestScriptwAnimation : MonoBehaviour
{
    [Header("Movement")]
    float horizontalInput;
    public float moveSpeed = 8f;
    bool isFacingRight = true;

    [Header("Jump")]
    public float jumpPower = 6f;
    public bool isGrounded = false;
    bool canDoubleJump = false;      // tracks if double jump is available

    [Header("Dash")]
    float dashSpeed = 15f;           // how fast the dash is
    public float dashDuration = 0.2f;       // how long the dash lasts in seconds
    float dashCooldown = 1f;         // seconds before you can dash again
    bool isDashing = false;
    bool canDash = true;
    float dashTimer = 0f;
    float dashCooldownTimer = 0f;

    bool isAttacking = false;


    Rigidbody2D rb;
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Start attack while holding
        if (UnityEngine.Input.GetMouseButton(0) && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }

        // Stop attack when released
        if (UnityEngine.Input.GetMouseButtonUp(0))
        {
            isAttacking = false;
            animator.Play("Movement");
        }


        // Don't allow movement input while dashing
        if (isDashing) return;

        horizontalInput = UnityEngine.Input.GetAxis("Horizontal");
        FlipSprite();
        HandleJump();
        HandleDash();
    }

    private void FixedUpdate()
    {
        // Don't override velocity while dashing
        if (isDashing) return;
        if (isAttacking) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (UnityEngine.Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // Normal jump
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                isGrounded = false;
                canDoubleJump = true;           // enable double jump after first jump
                animator.SetBool("isJumping", true);
            }
            else if (canDoubleJump)
            {
                // Double jump
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                canDoubleJump = false;          // consume the double jump
                animator.SetBool("isJumping", true);
            }
        }
    }

    void HandleDash()
    {
        // Count down the cooldown
        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
                canDash = true;
        }

        // Trigger dash with Left Shift
        if (UnityEngine.Input.GetMouseButton(1) && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;
        dashCooldownTimer = dashCooldown;

        // Determine dash direction (use facing direction if no input)
        float dashDirection = horizontalInput != 0 ? Mathf.Sign(horizontalInput) : (isFacingRight ? 1f : -1f);

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);  // zero Y for a clean horizontal dash
        animator.SetBool("isDashing", true);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("isDashing", false);
    }

    void FlipSprite()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
            canDoubleJump = false;
            animator.SetBool("isJumping", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

}