using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Menu Canvas that displays during gameplay.
/// Menu remains open without pausing the game (Time.timeScale stays 1).
/// </summary>
public class MenuCanvas : MonoBehaviour
{
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // Set up Close Menu button
        if (closeMenuButton != null)
        {
            closeMenuButton.onClick.AddListener(OnCloseMenuClicked);
            Debug.Log("Close Menu button initialized");
        }
        else
        {
            Debug.LogWarning("⚠️ Close Menu Button not assigned in MenuCanvas Inspector!");
        }

        // Set up Pause button
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseClicked);
            Debug.Log("Pause button initialized");
        }
        else
        {
            Debug.LogWarning("⚠️ Pause Button not assigned in MenuCanvas Inspector!");
        }

        // Set up Quit button
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
            Debug.Log("Quit button initialized");
        }
        else
        {
            Debug.LogWarning("⚠️ Quit Button not assigned in MenuCanvas Inspector!");
        }
    }

    private void OnCloseMenuClicked()
    {
        Debug.Log("Close Menu clicked - destroying menu canvas");
        Destroy(gameObject);
    }

    private void OnPauseClicked()
    {
        Debug.Log("Pause button clicked from menu - hiding menu canvas");
        // Hide this menu canvas
        gameObject.SetActive(false);
        
        // Store reference in GameManager so Resume can show it back
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetActiveMenuCanvas(gameObject);
            GameManager.Instance.TogglePause();
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance is null!");
        }
    }

    private void OnQuitClicked()
    {
        Debug.Log("⏹️ Quit button clicked!");
        
        // Resume the game
        Time.timeScale = 1f;

        // Reload the scene WITHOUT restart flag so loading canvas shows
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        Debug.Log("🔄 Returning to loading screen");
    }
}
