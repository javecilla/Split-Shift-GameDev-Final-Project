using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
    public void TogglePause()
    {
        GameManager.Instance.TogglePause();
    }
}
