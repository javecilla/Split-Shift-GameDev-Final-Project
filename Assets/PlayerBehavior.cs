using System;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    float horizontalInput;
    bool isFacingRight = true;
    // public bool isMobileControls;

    [Header("Jump")]
    public float jumpPower = 8f;
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

    [Header("Melee")]
    public float meleeDamage = 30f;
    public float meleeRange = 1.5f;
    public Vector2 meleeBoxSize = new Vector2(1.5f, 1f);
    public LayerMask enemyLayer;

    [Header("Fall Death")]
    public float offScreenOffset = 2f; // Extra distance below camera view before death
    private float calculatedFallThreshold;

    [Header("Audio")]
    AudioSource aud;
    public AudioClip jumpSFX, dashSFX, runSFX, punchSFX, projectileSFX, deathSFX, damagedSFX;

    Rigidbody2D rb;
    Animator animator;
    bool isRangedAttacking = false;

    bool IsJax => PlayerManager.Instance.isJax;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();
        CalculateFallThreshold();
    }

    private void CalculateFallThreshold()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calculate the bottom of the camera view
            float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
            
            // Set threshold below the camera view with an offset
            calculatedFallThreshold = cameraBottom - offScreenOffset;
            
            Debug.Log($"Fall Threshold calculated: {calculatedFallThreshold} (Camera bottom: {cameraBottom})");
        }
        else
        {
            Debug.LogWarning("Main Camera not found! Fall death may not work correctly.");
        }
    }

    void Update()
    {
        if (isDashing) return;

        // Check if player fell below threshold
        CheckFallDeath();

        float keyboardInput = Input.GetAxis("Horizontal");
        if (keyboardInput != 0f)
            horizontalInput = keyboardInput;


        FlipPlayerDirection();
        HandleJump();
        HandleAttack();
        HandleDash();
    }

    public void MoveX(float x)
    {
        horizontalInput = x;
    }

    public void StopMovement()
    {
        horizontalInput = 0f;
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        if (isRangedAttacking) return;

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        // Walking sound
        bool isWalking = Mathf.Abs(horizontalInput) > 0.1f && isGrounded;
        if (isWalking)
        {
            if (!aud.isPlaying || aud.clip != runSFX)
            {
                aud.clip = runSFX;
                aud.loop = true;
                aud.Play();
            }
        }
        else
        {
            if (aud.isPlaying && aud.clip == runSFX)
            {
                aud.Stop();
            }
        }
    }

    public void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
            PerformJump();

    }

    public void PerformJump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            animator.SetBool("isJumping", true);
            if (jumpSFX != null)
            {
                aud.clip = jumpSFX;
                aud.Play();    
            } 
            if (IsJax) canDoubleJump = true;
        }
        else if (IsJax && canDoubleJump)
        {
            canDoubleJump = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            animator.SetBool("isJumping", true);
            if (jumpSFX != null)
            {
                aud.clip = jumpSFX;
                aud.Play();    
            } 
        }
    }

    public void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        PerformAttack();

        if (isRangedAttacking && animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
            isRangedAttacking = false;

    }

    public void PerformAttack()
    {
        if (IsJax)
        {
            animator.SetTrigger("Attack 0");
            if (punchSFX != null)
            {
                aud.clip = punchSFX;
                aud.Play();    
            } 
        }
        else
        {
            FireProjectile();
            isRangedAttacking = true;
            animator.SetTrigger("RangedAttack");
            if (projectileSFX != null)
            {
                aud.clip = projectileSFX;
                aud.Play();    
            } 
        }
    }


    // Called via animation event on Jax attack clip
    public void JaxMeleeHit()
    {
        // Detect hit direction based on facing
        Vector2 hitOrigin = (Vector2)transform.position +
            (isFacingRight ? Vector2.right : Vector2.left) * (meleeRange / 2);

        Collider2D[] hits = Physics2D.OverlapBoxAll(hitOrigin, meleeBoxSize, 0f, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
                Debug.Log("Jax hit " + hit.gameObject.name + " for " + meleeDamage);
            }
        }
    }

    void FireProjectile()
    {
        var proj = Instantiate(ProjectilePrefab, LaunchOffset.position, Quaternion.identity);
        ProjectileBehavior projScript = proj.GetComponent<ProjectileBehavior>();
        if (projScript != null)
            projScript.direction = isFacingRight ? Vector2.right : Vector2.left;

        if (!isFacingRight)
            proj.transform.localScale = new Vector3(-1, 1, 1);
    }

    public void HandleDash()
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
                TriggerDash();
        }
    }

    public void TriggerDash() // called by UI
    {
        if (!IsJax && canDash)
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

        if (dashSFX != null)
            {
                aud.clip = dashSFX;
                aud.Play();    
            } 

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

    public void TakeDamage(float damage)
    {
        PlayerManager.Instance.PlayerHealth -= damage;
        PlayerManager.Instance.UpdateHUD();
        Debug.Log("Player took " + damage + " damage! Health: " + PlayerManager.Instance.PlayerHealth);

        animator.SetTrigger("Damaged");

        if (deathSFX != null) aud.PlayOneShot(deathSFX);

        if (PlayerManager.Instance.PlayerHealth <= 0)
        {
            
            Debug.Log("Your dead");
            if (deathSFX != null) aud.PlayOneShot(deathSFX);
        }
    }

    private void CheckFallDeath()
    {
        if (transform.position.y < calculatedFallThreshold)
        {
            Debug.Log("Player fell off-screen!");
            PlayerManager.Instance.PlayerHealth = 0;
            PlayerManager.Instance.UpdateHUD();
        }
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

    void OnDrawGizmosSelected()
    {
        Vector2 hitOrigin = (Vector2)transform.position +
            (isFacingRight ? Vector2.right : Vector2.left) * (meleeRange / 2);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(hitOrigin, meleeBoxSize);
    }
}