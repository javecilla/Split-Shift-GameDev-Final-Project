using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverCanvas : MonoBehaviour
{
    public void Restart()
    {
        Debug.Log("Restart button clicked!");
        Time.timeScale = 1;
        
        // Clean up GameManager's tempCanvas before reloading scene
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CleanupBeforeReload();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
