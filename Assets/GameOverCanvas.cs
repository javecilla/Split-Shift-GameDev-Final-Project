using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    public void Restart()
    {
        Debug.Log("Restart button clicked!");
        Time.timeScale = 1;
        
        // Reset game state without reloading scene
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ResetGameState();
        }

        // Clean up GameManager's overlay
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CleanupBeforeReload();
        }
        
        // Destroy the game over canvas
        Destroy(gameObject);
    }
}
