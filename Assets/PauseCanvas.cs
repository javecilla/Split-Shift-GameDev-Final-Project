using UnityEngine;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour
{
    public void Resume()
    {
        if (GameManager.Instance != null)
        {
            // Clear the menu canvas reference so it doesn't show
            GameManager.Instance.SetActiveMenuCanvas(null);
            GameManager.Instance.TogglePause();
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance is null!");
        }
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
