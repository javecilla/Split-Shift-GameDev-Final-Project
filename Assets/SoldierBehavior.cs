using UnityEngine;

public class SoldierBehavior : EnemyBase
{
    [Header("Soldier - Ranged Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletDamage = 20f;

    [Header("Soldier - Charge Attack")]
    public float chargedAttackDamage = 40f;
    public float chargeDashSpeed = 12f;
    public float chargeDuration = 0.4f;
    public float chargedAttackCooldown = 6f;
    float chargedAttackTimer = 0f;

    bool isCharging = false;
    float chargeDashTimer = 0f;
    bool hasChargedOnce = false;

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {
        firePoint.localScale = new Vector3(-1, 1, 1);

        if (isDead) return;
        if (PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null) return;

        chargedAttackTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        if (isCharging) return;

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
            hasChargedOnce = false; // reset when player leaves vision
        }
    }

    protected override void FixedUpdate()
    {
        if (isDead) return;

        if (isCharging)
        {
            ChargeDash();
            return;
        }

        switch (currentState)
        {
            case State.Patrol:
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // stationary
                break;
            case State.Chase:
                ShootAtPlayer(); // player in vision but outside melee range
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    // Stationary — no patrol override needed
    protected override void Patrol()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // Repurposed Chase — soldier shoots when player is in vision
    protected override void Chase()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void ShootAtPlayer()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            // First time seeing player — charged attack first
            if (!hasChargedOnce && chargedAttackTimer >= chargedAttackCooldown)
            {
                chargedAttackTimer = 0f;
                hasChargedOnce = true;
                isCharging = true;
                chargeDashTimer = 0f;
                animator.SetTrigger("ChargedAttack");
            }
            else
            {
                animator.SetTrigger("Attack");
                // FireBullet() called via animation event
            }
        }
    }

    protected override void TriggerAttack()
    {
        if (!hasChargedOnce && chargedAttackTimer >= chargedAttackCooldown)
        {
            chargedAttackTimer = 0f;
            hasChargedOnce = true;
            isCharging = true;
            chargeDashTimer = 0f;
            animator.SetTrigger("ChargedAttack");
        }
        else
        {
            animator.SetTrigger("Attack");
        }
    }

    void ChargeDash()
    {
        chargeDashTimer += Time.deltaTime;

        Vector2 direction = (Player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chargeDashSpeed, rb.linearVelocity.y);

        if (chargeDashTimer >= chargeDuration)
        {
            isCharging = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Called via animation event
    public void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        SoldierBullet bulletScript = bullet.GetComponent<SoldierBullet>();

        if (bulletScript != null)
        {
            bulletScript.direction = movingRight ? Vector2.right : Vector2.left;
            bulletScript.damage = bulletDamage;
        }
    }

    // Called via animation event
    public void ChargedAttackDamage()
    {
        if (PlayerManager.Instance == null || Player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        if (distanceToPlayer > attackRange + 1f) return;

        Debug.Log("Soldier Charge Hit! Damage: " + chargedAttackDamage);
        Player.GetComponent<PlayerBehavior>().TakeDamage(chargedAttackDamage);
    }
}