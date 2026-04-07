using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the pause button setup to ensure onClick listener persists across scene reloads.
/// Attach this to the pause button GameObject in your scene.
/// </summary>
public class PauseButtonManager : MonoBehaviour
{
    private void Start()
    {
        // Programmatically set up the onClick listener to ensure it works after scene reloads
        Button pauseButton = GetComponent<Button>();
        
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(() => GameManager.Instance.TogglePause());
            Debug.Log("Pause button onClick listener set up successfully");
        }
        else
        {
            Debug.LogError("PauseButtonManager: No Button component found on this GameObject!");
        }
    }
}
