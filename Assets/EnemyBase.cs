// base class (shared logic: patrol, chase, flip, take damage)
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Patrol")]
    public float moveSpeed = 4f;
    public float patrolDistance = 4f;
    public float pauseTime = 2f;

    [Header("Sprite")]
    public bool flipInitialSprite = false;

    [Header("Detection")]
    public float visionDistance = 6f;
    public float attackRange = 1.5f;
    public float chaseSpeedMultiplier = 2.5f;

    [Header("Attack")]
    public float normalAttackDamage = 10f;
    public float attackCooldown = 1f;

    [Header("Health")]
    public float maxHealth = 100f;
    protected float currentHealth;
    protected bool isDead = false;

    [Header("Layers")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Vector2 startPos;
    protected bool movingRight = true;
    protected bool isPaused = false;
    protected float pauseCounter;
    protected float attackTimer = 0f;

    protected enum State { Patrol, Chase, Attack }
    protected State currentState;

    protected Transform Player => PlayerManager.Instance.CurrentPlayer;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPos = transform.position;
        currentState = State.Patrol;
        currentHealth = maxHealth;

        // Flip initial sprite to face left
        if (flipInitialSprite)
        {
            movingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x); // force facing left
            transform.localScale = scale;
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;
        if (PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        if (CanSeePlayer())
        {
            if (distanceToPlayer <= attackRange)
                currentState = State.Attack;
            else
                currentState = State.Chase;
        }
        else
        {
            currentState = State.Patrol;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isDead) return;

        switch (currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase:  Chase();  break;
            case State.Attack: Attack(); break;
        }
    }

    protected virtual void Patrol()
    {
        if (isPaused)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            pauseCounter += Time.deltaTime;

            if (pauseCounter >= pauseTime)
            {
                pauseCounter = 0f;
                isPaused = false;
                movingRight = !movingRight;
                Flip();
            }
            return;
        }

        float distanceFromStart = transform.position.x - startPos.x;
        float currentSpeed = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        if ((movingRight && distanceFromStart >= patrolDistance) ||
            (!movingRight && distanceFromStart <= -patrolDistance))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            isPaused = true;
            pauseCounter = 0f;
        }
    }

    protected virtual void Chase()
    {
        Vector2 direction = (Player.position - transform.position).normalized;
        float currentSpeed = moveSpeed * chaseSpeedMultiplier;
        rb.linearVelocity = new Vector2(direction.x * currentSpeed, rb.linearVelocity.y);

        if ((direction.x > 0 && !movingRight) || (direction.x < 0 && movingRight))
        {
            movingRight = !movingRight;
            Flip();
        }
    }

    protected virtual void Attack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            TriggerAttack();
        }
    }

    protected abstract void TriggerAttack();

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Damaged");
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Death");
        // Destroy after death animation plays
        Destroy(gameObject, 2f);
    }

    public virtual void NormalAttackDamage()
    {
        if (PlayerManager.Instance == null || Player == null) return;
        Debug.Log("Normal Attack! Damage: " + normalAttackDamage);
        Player.GetComponent<PlayerBehavior>().TakeDamage(normalAttackDamage);
    }

    protected bool CanSeePlayer()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            visionDistance,
            playerLayer | groundLayer
        );

        if (hit.collider != null)
        {
            if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                return true;
        }

        return false;
    }

    protected void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(direction * visionDistance));
    }
}