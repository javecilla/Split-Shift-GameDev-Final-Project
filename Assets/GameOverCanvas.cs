using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    public void Restart()
    {
        Debug.Log("Restart button clicked!");
        Time.timeScale = 1;
        
        // Reload the scene to reset all enemies and game state
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
