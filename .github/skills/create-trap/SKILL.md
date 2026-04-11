---
name: create-trap
description: "Toolkit for creating traps, hazards, and environmental dangers in Split Shift. Use when adding saw blades, spike pits, instant-death zones, or moving hazards."
---

# Create Trap Toolkit

## Core Architecture Context

- **Trap Pattern:** Traps are simple MonoBehaviour scripts that detect player collision and deal damage over time.
- **Kill Zones:** Instant-death triggers used for bottomless pits or lethal hazards. Set `PlayerManager.Instance.PlayerHealth = 0` and call `UpdateHUD()`.
- **Player Detection:** Use `OnTriggerStay2D` for continuous hazards (saws, fire) and `OnTriggerEnter2D` for one-shot traps (spikes, crushers).
- **Tag System:** Player is identified by `CompareTag("Player")` or `GetComponent<PlayerBehavior>()`.

## Strict Coding Constraints

- **Encapsulation:** NEVER use `public` fields for Inspector variables. Strictly use `[SerializeField] private`.
- **Naming:** All private fields MUST use the `_camelCase` prefix.
- **Damage Cooldown:** All continuous traps MUST have a cooldown timer to prevent instant-death from frame-by-frame damage.
- **Player Damage:** Call `player.TakeDamage(amount)`—never directly modify `PlayerManager.Instance.PlayerHealth` except in KillZones.
- **Visual Feedback:** Traps should have clear visual/audio cues so players can read the danger.

## Execution Steps

1. **Identify Trap Type:**
   - **Continuous Hazard** (saw blades, fire): Use `OnTriggerStay2D` with a cooldown timer.
   - **One-Shot Trap** (spikes, crusher): Use `OnTriggerEnter2D`—damage once per contact.
   - **Kill Zone** (bottomless pit): Use `OnTriggerEnter2D` to instantly set health to 0.
   - **Moving Hazard** (pendulum, patrolling saw): Add movement logic (`Patrol()` pattern similar to `EnemyBase`).

2. **Scaffold Class:** Create the C# script matching the trap name (PascalCase). File name must match class name.

3. **Implement Core Logic:**
   - `[SerializeField] private float _damageAmount`
   - `[SerializeField] private float _damageCooldown`
   - `_damageTimer` that decrements in `Update()`
   - Trigger method checks player tag, checks cooldown, deals damage

4. **Add Movement (Optional):** For moving traps, follow the `EnemyBase.Patrol()` pattern—use `Rigidbody2D.linearVelocity`, flip direction, and use distance bounds.

5. **Code Review:** Verify zero `public` variables, cooldown logic is present, and damage flows through `PlayerBehavior.TakeDamage()`.

## Template: Continuous Hazard

```csharp
using UnityEngine;

public class TrapName : MonoBehaviour
{
    [SerializeField] private float _damageAmount = 5f;
    [SerializeField] private float _damageCooldown = 0.5f;

    private float _damageTimer = 0f;

    void Update()
    {
        if (_damageTimer > 0)
            _damageTimer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_damageTimer <= 0)
            {
                PlayerBehavior player = collision.GetComponent<PlayerBehavior>();
                if (player != null)
                {
                    player.TakeDamage(_damageAmount);
                    _damageTimer = _damageCooldown;
                }
            }
        }
    }
}
```

## Template: Kill Zone

```csharp
using UnityEngine;

public class KillZoneName : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerBehavior>() != null)
        {
            PlayerManager.Instance.PlayerHealth = 0;
            PlayerManager.Instance.UpdateHUD();
        }
    }
}
```

## Anti-Patterns to Avoid

- ❌ Dealing damage every frame without a cooldown (instant death).
- ❌ Using `public` fields for Inspector-exposed values.
- ❌ Directly modifying `PlayerManager.Instance.PlayerHealth` in non-lethal traps.
- ❌ Missing visual/audio feedback for the hazard.
- ❌ Not checking the player tag before applying damage.
