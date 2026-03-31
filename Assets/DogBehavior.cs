using UnityEngine;

public class DogBehavior : EnemyBase
{
    [Header("Charged Attack")]
    public float chargedAttackDamage = 45f;
    public float chargedAttackCooldown = 4f;
    public float chargedAttackRange = 3f;
    float chargedAttackTimer = 4f;

    [Header("Patrol Attack")]
    public float patrolAttackPauseTime = 1f;

    bool isPatrolAttacking = false;
    float patrolAttackPauseTimer = 0f;
    bool attackFromPatrol = false;

    protected override void Start()
    {
        base.Start();
        chargedAttackTimer = chargedAttackCooldown; // ready on spawn
    }

    // protected override void Update()
    // {
    //     base.Update();
    //     chargedAttackTimer += Time.deltaTime;
    // }

    protected override void Patrol()
    {
        if (isPatrolAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            patrolAttackPauseTimer += Time.deltaTime;

            if (patrolAttackPauseTimer >= patrolAttackPauseTime)
            {
                patrolAttackPauseTimer = 0f;
                isPatrolAttacking = false;
                attackFromPatrol = false;
                movingRight = !movingRight;
                Flip();
            }
            return;
        }

        bool playerIsAhead = (movingRight && Player.position.x > transform.position.x) ||
                     (!movingRight && Player.position.x < transform.position.x);


        // Check if player is blocking the way during patrol
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        if (playerIsAhead && distanceToPlayer <= chargedAttackRange && chargedAttackTimer >= chargedAttackCooldown)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            chargedAttackTimer = 0f;
            isPatrolAttacking = true;
            attackFromPatrol = false; // charged attack deals damage
            patrolAttackPauseTimer = 0f;
            animator.SetTrigger("ChargedAttack");
            return;
        }

        float distanceFromStart = transform.position.x - startPos.x;
        float currentSpeed = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        if ((movingRight && distanceFromStart >= patrolDistance) ||
            (!movingRight && distanceFromStart <= -patrolDistance))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            isPatrolAttacking = true;
            attackFromPatrol = true;
            patrolAttackPauseTimer = 0f;
            animator.SetTrigger("Attack");
        }
    }

    protected override void TriggerAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        if (distanceToPlayer <= chargedAttackRange && chargedAttackTimer >= chargedAttackCooldown)
        {
            chargedAttackTimer = 0f;
            animator.SetTrigger("ChargedAttack");
        }
        else
        {
            animator.SetTrigger("Attack");
        }
    }

    public override void NormalAttackDamage()
    {
        // Debug.Log("NormalAttackDamage called! attackFromPatrol: " + attackFromPatrol);

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        // Debug.Log("Distance to Player: " + distanceToPlayer);
        // Debug.Log("Attack Range: " + attackRange);
        if (distanceToPlayer > attackRange) return;

        // Debug.Log("Dog Normal Attack! Damage: " + normalAttackDamage);
        Player.GetComponent<PlayerBehavior>().TakeDamage(normalAttackDamage);
    }

    public void ChargedAttackDamage()
    {
        if (PlayerManager.Instance == null || Player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        if (distanceToPlayer > chargedAttackRange) return;

        // Debug.Log("Dog Charged Attack! Damage: " + chargedAttackDamage);
        Player.GetComponent<PlayerBehavior>().TakeDamage(chargedAttackDamage);
    }
}