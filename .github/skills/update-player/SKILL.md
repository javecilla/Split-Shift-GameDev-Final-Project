---
name: update-player
description: Use when adding features, refactoring, or updating the PlayerBehavior controller for Jax and Axel.
---

# Modify Player Controller Workflow

## Before Starting

- Verify if the requested feature applies to Jax (melee/double jump), Axel (ranged/dash), or both.
- Determine if the logic requires input detection (`Update()`) or physics manipulation (`FixedUpdate()`).

## Rules & Constraints

- **Encapsulation:** Never use `public` fields for variables (e.g., replace existing `public float moveSpeed` with `[SerializeField] private float _moveSpeed`).
- **Naming:** All private fields must use `_camelCase`.
- **Separation of Concerns:** Input detection must reside in `Update()`. Rigidbody velocity/forces must reside in `FixedUpdate()`.
- **State Verification:** Logic specific to a character must check the `IsJax` boolean pulled from `PlayerManager.Instance.isJax`.

## Step-by-Step Instructions

1. **Analyze:** Check `PlayerBehavior.cs` to locate where the new logic belongs (e.g., `HandleJump`, `HandleAttack`, `HandleDash`).
2. **Implement Input:** Add keyboard/button detection in the `Update()` method.
3. **Implement Physics:** Add movement or forces to `FixedUpdate()` and multiply by `Time.deltaTime` if outside of rigid body physics.
4. **Refactor:** If you modify an existing section of `PlayerBehavior.cs` that violates the standards (like public fields), automatically refactor it to `[SerializeField] private _camelCase`.
5. **UI Updates:** If the change affects health, mana, or switching, ensure `PlayerManager.Instance.UpdateHUD()` is called.
