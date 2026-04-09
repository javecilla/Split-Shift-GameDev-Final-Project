using UnityEngine;

public class NinjaBehaviour : EnemyBase
{
    [Header("Ninja - First Attack (Patrol Endpoint)")]
    [SerializeField] private float _firstAttackDamage = 20f;
    [SerializeField] private float _firstAttackRange = 1.5f;
    [SerializeField] private float _firstAttackCooldown = 3f;
    [SerializeField] private float _patrolAttackPauseTime = 1f;

    [Header("Ninja - Second Attack (Chase Range)")]
    [SerializeField] private float _secondAttackDamage = 35f;
    [SerializeField] private float _secondAttackRange = 2.5f;
    [SerializeField] private float _secondAttackCooldown = 2.5f;
    [SerializeField] private float _attackAnimationDelay = 0.4f;

    private float _firstAttackTimer = 0f;
    private float _secondAttackTimer = 0f;
    private float _attackAnimationTimer = 0f;
    private bool _hasDealtDamageInCurrentAttack = false;
    private bool _isPatrolAttacking = false;
    private float _patrolAttackPauseTimer = 0f;

    [Header("Audio")]
    AudioSource aud;
    public AudioClip attackSFX, deathSFX, damagedSFX;

    protected override void Start()
    {
        base.Start();
        aud = GetComponent<AudioSource>();
        _firstAttackTimer = _firstAttackCooldown;   // Ready on spawn
        _secondAttackTimer = _secondAttackCooldown; // Ready on spawn
    }

    protected override void Update()
    {
        base.Update();
        _firstAttackTimer += Time.deltaTime;
        _secondAttackTimer += Time.deltaTime;

        // Override state machine for better player detection (omnidirectional during patrol)
        if (isDead || PlayerManager.Instance == null || PlayerManager.Instance.CurrentPlayer == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        // Check if player is within vision distance (regardless of facing direction)
        if (distanceToPlayer <= visionDistance)
        {
            if (distanceToPlayer <= _secondAttackRange)
                currentState = State.Attack;
            else
                currentState = State.Chase;
        }
        else if (currentState != State.Patrol || !_isPatrolAttacking)
        {
            // Only go back to patrol if not currently executing a patrol attack
            currentState = State.Patrol;
        }
    }

    protected override void FixedUpdate()
    {
        // Handle attack animation timing (bypass animator events)
        if (currentState == State.Attack)
        {
            _attackAnimationTimer += Time.fixedDeltaTime;

            // Deal damage once per attack, after the delay
            if (!_hasDealtDamageInCurrentAttack && _attackAnimationTimer >= _attackAnimationDelay)
            {
                // Determine which attack to apply based on trigger
                DealAttackDamage();
                _hasDealtDamageInCurrentAttack = true;
            }
        }
        else
        {
            // Reset for next attack
            _attackAnimationTimer = 0f;
            _hasDealtDamageInCurrentAttack = false;
        }

        // Update animator Speed parameter for Idle to Walking transition
        float currentSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", currentSpeed);

        // Call base FixedUpdate for state machine
        base.FixedUpdate();
    }

    protected override void Patrol()
    {
        // If paused from patrol attack, wait before continuing
        if (_isPatrolAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            _patrolAttackPauseTimer += Time.deltaTime;

            if (_patrolAttackPauseTimer >= _patrolAttackPauseTime)
            {
                _patrolAttackPauseTimer = 0f;
                _isPatrolAttacking = false;
                movingRight = !movingRight;
                Flip();
            }
            return;
        }

        // During patrol, check if player is within vision distance (regardless of facing direction)
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        if (distanceToPlayer <= visionDistance)
        {
            // Player is close! Transition to Chase via Update's state machine
            return; // Base.Update() will handle the transition
        }

        // Continue walking patrol pattern if player not visible
        float distanceFromStart = transform.position.x - startPos.x;
        float currentSpeed = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        // Check if reached patrol endpoint
        if ((movingRight && distanceFromStart >= patrolDistance) ||
            (!movingRight && distanceFromStart <= -patrolDistance))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            _isPatrolAttacking = true;
            _patrolAttackPauseTimer = 0f;
            _attackAnimationTimer = 0f;
            _hasDealtDamageInCurrentAttack = false;
            _firstAttackTimer = 0f; // Reset cooldown after triggering
            animator.SetTrigger("FirstAttack");
        }
    }

    protected override void Chase()
    {
        Vector2 direction = (Player.position - transform.position).normalized;
        float currentSpeed = moveSpeed * chaseSpeedMultiplier;
        rb.linearVelocity = new Vector2(direction.x * currentSpeed, rb.linearVelocity.y);

        if ((direction.x > 0 && !movingRight) || (direction.x < 0 && movingRight))
        {
            movingRight = !movingRight;
            Flip();
        }

        // Check if player is within SecondAttack range during chase
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        if (distanceToPlayer <= _secondAttackRange && _secondAttackTimer >= _secondAttackCooldown)
        {
            currentState = State.Attack; // Transition to attack
            _secondAttackTimer = 0f;
            _attackAnimationTimer = 0f;
            _hasDealtDamageInCurrentAttack = false;
        }
    }

    protected override void TriggerAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        // Check if can do SecondAttack (long range, during chase)
        if (distanceToPlayer <= _secondAttackRange && _secondAttackTimer >= _secondAttackCooldown)
        {
            _secondAttackTimer = 0f;
            animator.SetTrigger("SecondAttack");
        }
        else
        {
            // Fallback to FirstAttack
            animator.SetTrigger("FirstAttack");
        }
    }

    private void DealAttackDamage()
    {
        if (PlayerManager.Instance == null || Player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        // Determine which attack animation is playing
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("FirstAttack"))
        {
            FirstAttackDamage();
        }
        else if (stateInfo.IsName("SecondAttack"))
        {
            SecondAttackDamage();
        }
    }

    public void FirstAttackDamage()
    {
        if (PlayerManager.Instance == null || Player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        if (distanceToPlayer > _firstAttackRange) return;

        Debug.Log("Ninja First Attack! Damage: " + _firstAttackDamage);
        Player.GetComponent<PlayerBehavior>().TakeDamage(_firstAttackDamage);
    }

    public void SecondAttackDamage()
    {
        if (PlayerManager.Instance == null || Player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        if (distanceToPlayer > _secondAttackRange) return;

        Debug.Log("Ninja Second Attack! Damage: " + _secondAttackDamage);
        Player.GetComponent<PlayerBehavior>().TakeDamage(_secondAttackDamage);
    }

    public void AttackSound()
    {
        if (attackSFX != null && aud != null) aud.PlayOneShot(attackSFX);
    }

    public void DeathSound()
    {
        if (deathSFX != null && aud != null) aud.PlayOneShot(deathSFX);
    }

    public void DamagedSound()
    {
        if (damagedSFX != null && aud != null) aud.PlayOneShot(damagedSFX);
    }
}