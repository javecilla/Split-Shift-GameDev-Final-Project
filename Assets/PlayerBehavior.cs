using System;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    float horizontalInput;
    bool isFacingRight = true;

    [Header("Jump")]
    public float jumpPower = 6f;
    public bool isGrounded = false;
    bool canDoubleJump = false;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    float dashCooldown = 1f;
    bool isDashing = false;
    bool canDash = true;
    float dashCooldownTimer = 0f;

    [Header("Projectile")]
    public GameObject ProjectilePrefab;
    public Transform LaunchOffset;

    Rigidbody2D rb;
    Animator animator;
    bool isRangedAttacking = false;

    bool IsJax => PlayerManager.Instance.isJax;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing) return;

        horizontalInput = Input.GetAxis("Horizontal");
        FlipPlayerDirection();
        HandleJump();
        HandleAttack();
        HandleDash();
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        if (isRangedAttacking) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                isGrounded = false; 
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                animator.SetBool("isJumping", true);

                if (IsJax)
                    canDoubleJump = true;
            }
            else if (IsJax && canDoubleJump)
            {
                canDoubleJump = false;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                animator.SetBool("isJumping", true);
            }
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (IsJax)
            {
                animator.SetTrigger("Attack 0");
            }
            else
            {
                FireProjectile();
                isRangedAttacking = true;
                animator.SetTrigger("RangedAttack");
            }
        }

        if (isRangedAttacking && animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
        {
            isRangedAttacking = false;
        }
    }

    void FireProjectile()
    {
        var proj = Instantiate(ProjectilePrefab, LaunchOffset.position, Quaternion.identity);
        ProjectileBehavior projScript = proj.GetComponent<ProjectileBehavior>();
        if (projScript != null) projScript.direction = isFacingRight ? Vector2.right : Vector2.left;

        if (!isFacingRight)
            proj.transform.localScale = new Vector3(-1, 1, 1);
    }

    void HandleDash()
    {
        if (!IsJax)
        {
            if (!canDash)
            {
                dashCooldownTimer -= Time.deltaTime;
                if (dashCooldownTimer <= 0f)
                    canDash = true;
            }

            if (Input.GetKeyDown(KeyCode.W) && canDash)
            {
                animator.SetBool("isJumping", false);
                StartCoroutine(PerformDash());
            }
        }
    }

    System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;
        dashCooldownTimer = dashCooldown;

        float dashDirection = horizontalInput != 0
            ? Mathf.Sign(horizontalInput)
            : (isFacingRight ? 1f : -1f);

        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
        animator.SetBool("isDashing", true);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("isDashing", false);

        if (!isGrounded)
            animator.SetBool("isJumping", true);
    }

    void FlipPlayerDirection()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    // Refactored to ping the PlayerManager metrics
    public void TakeDamage(float damage)
    {
        PlayerManager.Instance.PlayerHealth -= damage;
        Debug.Log("Player took " + damage + " damage! Health: " + PlayerManager.Instance.PlayerHealth);

        if (PlayerManager.Instance.PlayerHealth <= 0)
            Debug.Log("Your dead");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
            canDoubleJump = false;
            animator.SetBool("isJumping", false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            isGrounded = false;
    }
}