using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Manages all input button setup to ensure onClick listeners persist across scene reloads.
/// Attach this to a parent GameObject that contains all the input buttons (e.g., "In Game Canvas" or "Basic Controls").
/// </summary>
public class InputButtonManager : MonoBehaviour
{
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button dashButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button switchButton;

    private PlayerManager playerManager;

    private void Start()
    {
        // Cache reference to player manager
        playerManager = PlayerManager.Instance;

        if (playerManager == null)
        {
            Debug.LogError("InputButtonManager: PlayerManager not found in scene!");
            return;
        }

        // Set up movement buttons with press and release events
        SetupMovementButton(moveLeftButton, () => GetActivePlayerBehavior()?.MoveX(-1f), "MoveLeft");
        SetupMovementButton(moveRightButton, () => GetActivePlayerBehavior()?.MoveX(1f), "MoveRight");

        // Set up action buttons - use dynamic player lookup
        SetupButton(jumpButton, () => GetActivePlayerBehavior()?.PerformJump(), "Jump");
        SetupButton(dashButton, () => GetActivePlayerBehavior()?.TriggerDash(), "Dash");
        SetupButton(attackButton, () => GetActivePlayerBehavior()?.PerformAttack(), "Attack");
        SetupButton(switchButton, () => playerManager.SwitchCharacter(), "Switch");

        Debug.Log("InputButtonManager: All input buttons set up successfully");
    }

    /// <summary>
    /// Get the active player's PlayerBehavior component.
    /// This ensures we always call methods on the currently active character.
    /// </summary>
    private PlayerBehavior GetActivePlayerBehavior()
    {
        if (playerManager?.CurrentPlayer != null)
        {
            return playerManager.CurrentPlayer.GetComponent<PlayerBehavior>();
        }
        
        Debug.LogWarning("InputButtonManager: Could not get active player behavior!");
        return null;
    }

    /// <summary>
    /// Helper method to set up a movement button with both press and release handlers.
    /// </summary>
    private void SetupMovementButton(Button button, UnityEngine.Events.UnityAction pressAction, string buttonName)
    {
        if (button == null)
        {
            Debug.LogWarning($"InputButtonManager: {buttonName} button not assigned!");
            return;
        }

        // Get or create EventTrigger for handling pointer events
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Pointer Down - Player starts moving
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => pressAction.Invoke());
        trigger.triggers.Add(pointerDownEntry);

        // Pointer Up - Player stops moving
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => GetActivePlayerBehavior()?.StopMovement());
        trigger.triggers.Add(pointerUpEntry);

        Debug.Log($"InputButtonManager: {buttonName} button press/release listeners set up successfully");
    }

    /// <summary>
    /// Helper method to set up a button with a click listener.
    /// </summary>
    private void SetupButton(Button button, UnityEngine.Events.UnityAction action, string buttonName)
    {
        if (button == null)
        {
            Debug.LogWarning($"InputButtonManager: {buttonName} button not assigned!");
            return;
        }

        button.onClick.AddListener(action);
        Debug.Log($"InputButtonManager: {buttonName} button onClick listener set up successfully");
    }
}
