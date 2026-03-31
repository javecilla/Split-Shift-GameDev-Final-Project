using UnityEngine;


public class GameManager : MonoBehaviour
{
    // A single ton game manager to hold global state and references
    public static GameManager Instance;
    public GameObject pauseCanvas;
    GameObject tempCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TogglePause()
    {
        if(Time.timeScale == 1)
        {
            Time.timeScale = 0;
            tempCanvas = Instantiate(pauseCanvas);
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1;
            if (tempCanvas != null)
                Destroy(tempCanvas);
            Debug.Log("Game Unpaused");
        }
    }

}
