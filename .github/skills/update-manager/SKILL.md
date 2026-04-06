---
name: update-manager
description: "Runbook for modifying global controllers (GameManager, HUDManager, PlayerManager, etc.) in Split Shift."
---

# Global Manager Workflow

## Core Architecture Context

- Managers handle global state (pausing, UI updates, character switching).
- They MUST utilize the Singleton pattern initialized in `Awake()`.

## Strict Coding Constraints

- **Encapsulation:** NEVER use `public` fields for Inspector variables. Strictly use `[SerializeField] private`.
- **Naming:** All private fields MUST use the `_camelCase` prefix.
- **Efficiency:** Cache all components in `Awake()` or `Start()`. Do NOT use `GetComponent()` inside `Update()`.

## Execution Steps

1. **Singleton Check:** Verify the script contains `public static ClassName Instance;` and initializes it properly in `Awake()`.
2. **Refactor Tech Debt:** Automatically scan the file for any `public` variables and convert them to `[SerializeField] private _camelCase`. Update all internal references to match the new variable names.
3. **Implement Logic:** Add the requested feature (e.g., UI toggles, scene loading, time scaling).
4. **Code Review:** Ensure no empty default Unity methods (`Start`, `Update`) are left behind if unused.
