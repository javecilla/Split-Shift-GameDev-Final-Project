---
name: integrate-animation
description: "Toolkit for integrating animation events, state machines, and animation-driven logic in Split Shift. Use when working with Animator controllers, animation clips, or animation-triggered gameplay events."
---

# Animation Integration Toolkit

## Core Architecture Context

- **Animator Usage:** All animated entities (`PlayerBehavior`, `EnemyBase` derivatives) cache an `Animator` component in `Start()`.
- **Animation Parameters:**
  - **Player:** `xVelocity` (float), `yVelocity` (float), `isJumping` (bool), `isDashing` (bool), `Attack 0` (trigger), `RangedAttack` (trigger), `Damaged` (trigger).
  - **Enemies:** `Speed` (float), `Attack` (trigger), `ChargedAttack` (trigger), `Damaged` (trigger), `Death` (trigger). Enemy-specific triggers vary (e.g., `FirstAttack`, `SecondAttack` for Ninja).
- **Animation Events:** Gameplay logic triggered mid-animation (e.g., `JaxMeleeHit()` in `PlayerBehavior` is called by the melee animation clip).
- **State Machine Flow:** Unity's Mecanim handles transitions; scripts set parameters, Animator drives visual state.

## Strict Coding Constraints

- **Encapsulation:** NEVER use `public` fields for Inspector variables. Strictly use `[SerializeField] private`.
- **Naming:** All private fields MUST use the `_camelCase` prefix.
- **Animator Caching:** Always cache `Animator` in `Awake()` or `Start()`. Never call `GetComponent<Animator>()` in update loops.
- **State Verification:** When checking current animation state, use `animator.GetCurrentAnimatorStateInfo(0).IsName("StateName")`.
- **Animation Events:** Methods called by animation events MUST be `public` and have the correct signature (typically no parameters, or a single `AnimationEvent` parameter).

## Execution Steps

1. **Identify Entity Type:** Determine if working on Player, Enemy (which type), or other animated entity.
2. **Review Animator Controller:** Check the `.controller` asset and its clips to understand existing states and parameters.
3. **Locate Integration Point:**
   - **Setting Parameters:** Add `animator.SetFloat()`, `animator.SetBool()`, or `animator.SetTrigger()` in the appropriate method.
   - **Animation Events:** Create a `public` method in the behavior script, then wire it to the animation clip's event timeline in Unity's Animation window.
   - **State Checks:** Use `GetCurrentAnimatorStateInfo(0).IsName("StateName")` to gate logic behind animation states.
4. **Implement Logic:** Add the parameter triggers, event methods, or state-dependent logic.
5. **Validate Transitions:** Ensure Animator transitions have correct conditions and no conflicting overrides.
6. **Refactor Tech Debt:** Auto-convert any `public` Inspector variables to `[SerializeField] private _camelCase`.

## Common Patterns

### Setting Animation Parameters
```csharp
// In Update() or FixedUpdate()
animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
animator.SetBool("isJumping", true);
animator.SetTrigger("Attack");
```

### Animation Event Method
```csharp
// Called by animation event on the clip
public void MeleeHitEvent()
{
    Collider2D[] hits = Physics2D.OverlapBoxAll(hitOrigin, meleeBoxSize, 0f, enemyLayer);
    foreach (Collider2D hit in hits)
    {
        EnemyBase enemy = hit.GetComponent<EnemyBase>();
        enemy?.TakeDamage(meleeDamage);
    }
}
```

### State-Gated Logic
```csharp
// Only perform action if in a specific animation state
if (animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
{
    // Allow movement input
}
```

### Transitioning After Animation
```csharp
// Wait for animation to finish before resetting state
if (isRangedAttacking && animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
    isRangedAttacking = false;
```

## Anti-Patterns to Avoid

- ❌ Calling `GetComponent<Animator>()` in `Update()` or `FixedUpdate()`.
- ❌ Hardcoding animation state names without verifying they match the `.controller`.
- ❌ Forgetting to wire animation events in Unity's Animation window after creating the method.
- ❌ Setting conflicting triggers (e.g., triggering `Attack` while already in an attack state).
- ❌ Using `public` fields for Inspector-exposed animation timing values.
- ❌ Not handling animation cancellation or interruption gracefully.
