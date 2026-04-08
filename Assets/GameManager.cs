using UnityEngine;


public class GameManager : MonoBehaviour
{
    // A single ton game manager to hold global state and references
    public static GameManager Instance;
    public GameObject pauseCanvas;
    
    // Star-based loss canvas variants
    [SerializeField] private GameObject lossCanvas0Stars;
    [SerializeField] private GameObject lossCanvas1Star;
    [SerializeField] private GameObject lossCanvas2Stars;
    
    GameObject tempCanvas;
    
    // Flag to skip loading screen on restart
    public bool isRestartingScene = false;

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

    public void ShowGameOver()
    {
        Time.timeScale = 0;
        
        // Get the star rating from GameStateTracker
        int starRating = 0;
        GameObject selectedLossCanvas = null;
        
        if (GameStateTracker.Instance != null)
        {
            starRating = GameStateTracker.Instance.CalculateStarRating();
        }

        // Select the appropriate loss canvas based on star rating
        switch (starRating)
        {
            case 0:
                selectedLossCanvas = lossCanvas0Stars;
                Debug.Log("📊 Showing Loss Canvas: 0 Stars");
                break;
            case 1:
                selectedLossCanvas = lossCanvas1Star;
                Debug.Log("📊 Showing Loss Canvas: 1 Star");
                break;
            case 2:
                selectedLossCanvas = lossCanvas2Stars;
                Debug.Log("📊 Showing Loss Canvas: 2 Stars");
                break;
        }

        if (selectedLossCanvas == null)
        {
            Debug.LogError("❌ Loss canvas prefab not assigned for star rating: " + starRating);
            return;
        }

        tempCanvas = Instantiate(selectedLossCanvas);
        Debug.Log("Game Over - Star Rating: " + starRating);
    }

    public void CleanupBeforeReload()
    {
        if (tempCanvas != null)
        {
            Destroy(tempCanvas);
            tempCanvas = null;
        }
        Time.timeScale = 1;
    }

}
