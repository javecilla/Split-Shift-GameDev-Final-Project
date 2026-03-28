using System;
using UnityEngine;

public class TestScriptwAnimation : MonoBehaviour
{
    [Header("Movement")]
    float horizontalInput;
    public float moveSpeed = 8f;
    bool isFacingRight = true;

    [Header("Jump")]
    public float jumpPower = 6f;
    public bool isGrounded = false;
    bool canDoubleJump = false;

    [Header("Dash")]
    float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    float dashCooldown = 1f;
    bool isDashing = false;
    bool canDash = true;
    float dashCooldownTimer = 0f;

    [Header("Ranged Attack")]
    bool isRangedAttacking = false;

    [Header("Projectile")]
    public ProjectileBehavior ProjectilePrefab;
    public Transform LaunchOffset;

    Rigidbody2D rb;
    Animator animator;
    public float PlayerHealth = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleAttack();
        HandleRangedAttack();
        HandleDash();

        if (isDashing) return;

        horizontalInput = Input.GetAxis("Horizontal");
        FlipSprite();
        HandleJump();
    }

    private void FixedUpdate()
    {
        if (isDashing) return;
        if (isRangedAttacking) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    // -------------------------
    // ATTACK (uses "Attack 0")
    // -------------------------
    void HandleAttack()
    {
        // Every click sends the trigger, Animator handles combo (JaxAtk1 -> 2 -> 3)
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack 0");
        }
    }

    // -------------------------
    // RANGED ATTACK
    // -------------------------
    void HandleRangedAttack()
    {
        if (Input.GetMouseButtonDown(1) && !isRangedAttacking)
        {
            FireProjectile();
            isRangedAttacking = true;
            animator.SetTrigger("RangedAttack");
        }

        if (isRangedAttacking && animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
        {
            isRangedAttacking = false;
        }
    }

    void FireProjectile()
    {
        var proj = Instantiate(ProjectilePrefab, LaunchOffset.position, Quaternion.identity);
        proj.direction = isFacingRight ? Vector2.right : Vector2.left;

        if (!isFacingRight)
            proj.transform.localScale = new Vector3(-1, 1, 1);
    }

    // -------------------------
    // JUMP
    // -------------------------
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                isGrounded = false;
                canDoubleJump = true;
                animator.SetBool("isJumping", true);
            }
            else if (canDoubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                canDoubleJump = false;
                animator.SetBool("isJumping", true);
            }
        }
    }

    // -------------------------
    // DASH
    // -------------------------
    void HandleDash()
    {
        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
                canDash = true;
        }

        if (Input.GetKeyDown(KeyCode.CapsLock) && canDash)
        {
            animator.SetBool("isJumping", false);
            StartCoroutine(PerformDash());
        }
    }

    System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;
        dashCooldownTimer = dashCooldown;

        float dashDirection = horizontalInput != 0 ? Mathf.Sign(horizontalInput) : (isFacingRight ? 1f : -1f);

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
        animator.SetBool("isDashing", true);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("isDashing", false);

        if (!isGrounded)
            animator.SetBool("isJumping", true);
    }

    // -------------------------
    // UTILITIES
    // -------------------------
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
}
