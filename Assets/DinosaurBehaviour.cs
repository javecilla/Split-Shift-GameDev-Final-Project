using UnityEngine;

public class DinosaurBehaviour : EnemyBase
{
    [Header("Dinosaur - Red Attack")]
    [SerializeField] private float _redAttackDamage = 15f;
    [SerializeField] private float _redAttackRange = 2f;
    [SerializeField] private float _attackAnimationDelay = 0.3f; // When in the animation to apply damage

    private float _attackAnimationTimer = 0f;
    private bool _hasDealtDamageInCurrentAttack = false;

    [Header("Audio")]
    public AudioClip fireSFX, deathSFX, damagedSFX, walkSFX;
    private AudioSource aud;

    protected override void Start()
    {
        base.Start();  
        aud = GetComponent<AudioSource>();
    }


    protected override void Update()
    {
        base.Update(); // Call base Update to see state changes
        Debug.Log($"State: {currentState}, CanSeePlayer: {CanSeePlayer()}");
    }

    protected override void FixedUpdate()
    {
        // Update animator Speed parameter for Idle to Walking transition
        float currentSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", currentSpeed);

        // Handle attack animation timing (bypass animator events)
        if (currentState == State.Attack)
        {
            _attackAnimationTimer += Time.fixedDeltaTime;
            
            // Deal damage once per attack, after the delay
            if (!_hasDealtDamageInCurrentAttack && _attackAnimationTimer >= _attackAnimationDelay)
            {
                RedAttackDamage();
                _hasDealtDamageInCurrentAttack = true;
            }
        }
        else
        {
            // Reset for next attack
            _attackAnimationTimer = 0f;
            _hasDealtDamageInCurrentAttack = false;
        }

        // Call base FixedUpdate for state machine
        base.FixedUpdate();
    }

    protected override void TriggerAttack()
    {
        // Fire the RedAttack animator trigger
        animator.SetTrigger("RedAttack");
        // Reset damage flag so it can trigger again on next attack
        _hasDealtDamageInCurrentAttack = false;
        _attackAnimationTimer = 0f;
    }

    public void RedAttackDamage()
    {
        // Called programmatically when in Attack state, no longer waiting for animator event
        if (PlayerManager.Instance == null || Player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);

        if (distanceToPlayer <= _redAttackRange)
        {
            Debug.Log("Red Attack! Damage: " + _redAttackDamage);
            Player.GetComponent<PlayerBehavior>().TakeDamage(_redAttackDamage);
        }
    }

    public void WalkSounds()
    {
        if (walkSFX != null && aud != null) aud.PlayOneShot(walkSFX);
    }

    public void FireSounds()
    {
        if (fireSFX != null && aud != null) aud.PlayOneShot(fireSFX);
    }

    public void DamagedSounds()
    {
        if (damagedSFX != null && aud != null) aud.PlayOneShot(damagedSFX);
    }

    public void DeathSounds()
    {
        if (deathSFX != null && aud != null) aud.PlayOneShot(deathSFX);
    }
}

