---
name: create-enemy
description: "Toolkit for generating new enemy AI in Split Shift. Use when asked to create an enemy, scaffold a mob, or implement an antagonist."
---

# Create Enemy Workflow

## Core Architecture Context

- The game uses a centralized `EnemyBase` abstract class.
- `EnemyBase` handles health, target detection (`CanSeePlayer`), flipping, and a `FixedUpdate` state machine (`State.Patrol`, `State.Chase`, `State.Attack`).

## Strict Coding Constraints

- **Encapsulation:** NEVER use `public` fields for Inspector variables. Strictly use `[SerializeField] private`.
- **Naming:** All private and protected fields MUST use the `_camelCase` prefix.
- **Inheritance:** The generated script MUST inherit from `EnemyBase`.

## Execution Steps

1. **Scaffold Class:** Create the C# script matching the enemy name (PascalCase).
2. **Implement Mandatory Overrides:** You MUST override the abstract `TriggerAttack()` method to execute damage or spawn projectiles.
3. **Extend State Machine (Optional):** If the enemy requires custom movement, override the `virtual` methods `Patrol()` or `Chase()`.
4. **Damage Dealing:** To damage the player, execute `Player.GetComponent<PlayerBehavior>().TakeDamage(damageAmount)`.
5. **Code Review:** Verify zero `public` variables exist in the final output.
