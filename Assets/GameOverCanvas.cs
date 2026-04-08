using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    public void Restart()
    {
        Debug.Log("Restart button clicked!");
        
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
}

