---
name: update-ui
description: "Toolkit for modifying UI components in Split Shift—HUD elements, menus, canvases, buttons, and overlays. Use when working on any visual interface element."
---

# Update UI Toolkit

## Core Architecture Context

- **HUDManager:** Controls the on-screen HUD (health bars, stress meter, character indicator). Updated via `PlayerManager.Instance.UpdateHUD()`.
- **Canvas System:** Multiple canvases exist—`PauseCanvas`, `GameOverCanvas` (LossCanvas variants), `WinCanvas`, `LoadingStartCanvas`, and `MenuCanvas`.
- **GameManager:** Manages canvas instantiation and `Time.timeScale` for pause/game-over states.
- **TextMeshPro:** All text rendering uses TextMeshPro. Never use Unity's default `Text` component.

## Strict Coding Constraints

- **Encapsulation:** NEVER use `public` fields for Inspector variables. Strictly use `[SerializeField] private`.
- **Naming:** All private fields MUST use the `_camelCase` prefix.
- **Canvas Safety:** Never destroy canvases directly—use `GameManager.Instance.CleanupBeforeReload()` or `Destroy(tempCanvas)`.
- **Time Scaling:** Always set `Time.timeScale = 0` when showing overlays (pause, game over) and `Time.timeScale = 1` when dismissing them.
- **Scene Reloads:** When reloading the scene, set `GameManager.Instance.isRestartingScene = true` to skip loading screen effects.

## Execution Steps

1. **Identify Canvas Type:** Determine if the UI is a HUD element (in-scene), menu (main menu), overlay (pause/game-over), or result screen (win/loss).
2. **Locate Manager:** Check if the UI is controlled by `HUDManager`, `GameManager`, `PauseCanvas`, `GameOverCanvas`, `WinCanvas`, or `LoadingStartCanvas`.
3. **Implement Changes:** Add/modify UI elements, bind events to buttons, update text/sprites.
4. **Wire Events:** Ensure button `OnClick()` events are properly connected to manager methods (e.g., `Restart()`, `Resume()`).
5. **Refactor Tech Debt:** Auto-convert any `public` Inspector variables to `[SerializeField] private _camelCase`. Update all references.
6. **Validate:** Test that the UI appears/disappears at the correct game states and doesn't leave orphaned GameObjects.

## Common Patterns

### HUD Updates
```csharp
// Always update HUD through PlayerManager
PlayerManager.Instance.UpdateHUD();
```

### Showing an Overlay Canvas
```csharp
Time.timeScale = 0;
tempCanvas = Instantiate(canvasPrefab);
```

### Dismissing an Overlay
```csharp
Time.timeScale = 1;
if (tempCanvas != null) Destroy(tempCanvas);
```

### Scene Restart
```csharp
GameManager.Instance.isRestartingScene = true;
SceneManager.LoadScene(SceneManager.GetActiveScene().name);
```

## Anti-Patterns to Avoid

- ❌ Directly modifying HUD elements without going through the proper manager.
- ❌ Instantiating multiple canvases without tracking/cleanup.
- ❌ Forgetting to reset `Time.timeScale` when dismissing overlays.
- ❌ Using Unity's built-in `Text` instead of TextMeshPro.
- ❌ Leaving `public` fields for Inspector-exposed values.
