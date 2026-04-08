using UnityEngine;
using UnityEngine.SceneManagement;

public class LossCanvas : MonoBehaviour
{
    private int currentStarRating = 0;

    private void Start()
    {
        // Get star rating from GameStateTracker
        if (GameStateTracker.Instance != null)
        {
            currentStarRating = GameStateTracker.Instance.CalculateStarRating();
            
            // Log performance metrics for debugging
            Debug.Log($"📊 Loss Canvas Stats:");
            Debug.Log($"   Enemies Killed: {GameStateTracker.Instance.GetEnemiesKilled()}/{GameStateTracker.Instance.GetTotalEnemies()}");
            Debug.Log($"   Boss Defeated: {GameStateTracker.Instance.IsBossDefeated()}");
            Debug.Log($"   Time: {GameStateTracker.Instance.GetGameTimeElapsed():F1}s");
            Debug.Log($"   Star Rating: {currentStarRating}⭐");
        }
        else
        {
            Debug.LogWarning("⚠️ GameStateTracker not found!");
        }
    }

    public void Restart()
    {
        Debug.Log("🔄 Restart button clicked!");
        
        // Resume the game
        Time.timeScale = 1;

        // Set flag to skip loading screen effects on restart
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isRestartingScene = true;
        }

        // Reload the scene to reset all enemies and game state
        // LoadingStartCanvas will detect isRestartingScene flag and skip pause/fade
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        Debug.Log("🔄 Scene reloading - all enemies will respawn");
    }

    public void Quit()
    {
        Debug.Log("⏹️ Quit button clicked!");
        
        // Resume the game
        Time.timeScale = 1;

        // Reload the scene WITHOUT restart flag so loading canvas shows
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        Debug.Log("🔄 Returning to loading screen");
    }

    /// <summary>
    /// Get the current star rating for external display (e.g., UI).
    /// </summary>
    public int GetStarRating()
    {
        return currentStarRating;
    }
}

