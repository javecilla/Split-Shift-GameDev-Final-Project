// Enemy1Behavior.cs
using UnityEngine;

public class Enemy3Bhavior1 : MonoBehaviour
{

    /*
    public void NormalAttackDamage()
    {
        // need lng maaccess method na to para sa event frame sa animation
        }

    public void ChargedAttackDamage()
    {
        // need lng maaccess method na to para sa event frame sa animation
    }
    */
    public float moveSpeed = 4f;
    public float patrolDistance = 4f;
    public float pauseTime = 2f;

    public float visionDistance = 6f;
    public float attackRange = 1.5f;
    public float normalAttackDamage = 10f;
    public float chargedAttackDamage = 25f;
    public float attackCooldown = 1f;
    public float chaseSpeedMultiplier = 2.5f;

    // public Transform player;
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private bool movingRight = true;
    private bool isPaused = false;
    private float pauseCounter;

    private enum State { Patrol, Chase, Attack }
    private State currentState;

    private float attackTimer = 0f;

    Transform Player => PlayerManager.Instance.CurrentPlayer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        currentState = State.Patrol;
    }

    void Update()
    {

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

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        // Pause handling
        if (isPaused)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            pauseCounter += Time.deltaTime;

            if (pauseCounter >= pauseTime)
            {
                pauseCounter = 0f;
                isPaused = false;

                // After pause ends, flip to start walking in the new direction
                movingRight = !movingRight;
                Flip();
            }
            return;
        }

        // Move in current direction
        float distanceFromStart = transform.position.x - startPos.x;
        float currentSpeed = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        // Start pause when reaching patrol limit
        if ((movingRight && distanceFromStart >= patrolDistance) ||
            (!movingRight && distanceFromStart <= -patrolDistance))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // stop immediately
            isPaused = true;
            pauseCounter = 0f;
        }
    }

    void Chase()
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

    void Attack()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            NormalAttackDamage();

        }
    }

    public void NormalAttackDamage()
    {
        Debug.Log("Normal Attack! Damage: " + normalAttackDamage);
        Player.GetComponent<TestScriptwAnimation>().TakeDamage(normalAttackDamage);
    }

    void ChargedAttack()
    {
        Debug.Log("Charged Attack! Damage: " + chargedAttackDamage);
        Player.GetComponent<TestScriptwAnimation>().TakeDamage(chargedAttackDamage);
    }


    bool CanSeePlayer()
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

    void Flip()
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

