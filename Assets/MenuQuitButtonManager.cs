using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the quit button inside the Menu Canvas.
/// When clicked: returns to Loading Start Canvas (restart the level).
/// Attach this script to the Quit Button inside Menu Canvas prefab.
/// </summary>
public class MenuQuitButtonManager : MonoBehaviour
{
    private void Start()
    {
        Button quitButton = GetComponent<Button>();
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            Debug.Log("✓ Menu Quit Button set up - will return to Loading Start Canvas");
        }
        else
        {
            Debug.LogError("❌ MenuQuitButtonManager: No Button component found!");
        }
    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("🔄 Menu Quit button clicked - returning to Loading Start Canvas");
        
        // Make sure restart flag is NOT set (normal reload, not a restart)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isRestartingScene = false;
        }
        
        // Resume time so loading canvas can pause properly
        Time.timeScale = 1f;
        
        // Reload the scene - this will show the Loading Start Canvas
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
