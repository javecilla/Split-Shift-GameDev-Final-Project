using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the win screen (3-star victory) for the player.
/// </summary>
public class WinCanvas : MonoBehaviour
{
    private int starRating = 3;

    private void Start()
    {
        // Get star rating from GameStateTracker
        if (GameStateTracker.Instance != null)
        {
            starRating = GameStateTracker.Instance.CalculateStarRating();
            
            // Log victory metrics
            Debug.Log($"🏆 Victory Canvas Stats:");
            Debug.Log($"   Enemies Killed: {GameStateTracker.Instance.GetEnemiesKilled()}/{GameStateTracker.Instance.GetTotalEnemies()}");
            Debug.Log($"   Boss Defeated: {GameStateTracker.Instance.IsBossDefeated()}");
            Debug.Log($"   Time: {GameStateTracker.Instance.GetGameTimeElapsed():F1}s (Under 600s limit)");
            Debug.Log($"   Star Rating: {starRating}⭐⭐⭐");
        }
        else
        {
            Debug.LogWarning("⚠️ GameStateTracker not found!");
        }
    }

    public void NextLevel()
    {
        Debug.Log("➡️ Next Level button clicked!");
        
        // Resume the game
        Time.timeScale = 1;

        // Reload the scene WITHOUT restart flag so loading canvas shows
        // (Currently going back to loading canvas since Level 2 doesn't exist yet)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        Debug.Log("🔄 Returning to loading screen (next level will load from here)");
    }

    /// <summary>
    /// Get the victory star rating for display.
    /// </summary>
    public int GetStarRating()
    {
        return starRating;
    }
}
