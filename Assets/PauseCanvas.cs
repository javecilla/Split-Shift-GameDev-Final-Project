using UnityEngine;
using UnityEngine.UI;

public class PauseCanvas : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.TogglePause();
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
