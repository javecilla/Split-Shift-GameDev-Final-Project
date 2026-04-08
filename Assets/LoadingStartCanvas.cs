using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingStartCanvas : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject inGameCanvasPrefab;
    [SerializeField] private CanvasGroup fadePanel;
    
    private float fadeDuration = 0.8f;

    void Start()
    {
        // Check if this is a scene restart - skip loading screen effects
        if (GameManager.Instance != null && GameManager.Instance.isRestartingScene)
        {
            GameManager.Instance.isRestartingScene = false;
            Debug.Log("🔄 Game restarting - skipping loading screen, resuming gameplay");
            
            // Instantiate the InGameCanvas prefab on restart
            if (inGameCanvasPrefab != null)
            {
                GameObject inGameCanvas = Instantiate(inGameCanvasPrefab);
                Debug.Log("✓ InGameCanvas prefab instantiated");

                if (PlayerManager.Instance != null)
                {
                    PlayerManager.Instance.InitializeSliders(inGameCanvas);
                }
            }
            else
            {
                Debug.LogError("❌ InGameCanvas prefab NOT assigned in LoadingStartCanvas Inspector!");
            }
            
            Time.timeScale = 1f;
            gameObject.SetActive(false);
            return;
        }

        // Normal loading screen flow
        // Pause the game while loading canvas is showing
        Time.timeScale = 0f;
        Debug.Log("⏸️ Game paused - Loading canvas active");

        // Initialize fade panel to transparent
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
        }
        else
        {
            Debug.LogWarning("⚠️ Fade Panel not assigned - transitions will skip fade effect");
        }

        // Set up the start button listener
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
            Debug.Log("Start Canvas initialized - waiting for player input");
        }
        else
        {
            Debug.LogError("Start Button not assigned in LoadingStartCanvas Inspector!");
        }
    }

    private void OnStartButtonClicked()
    {
        startButton.interactable = false;
        Debug.Log("Start button clicked - beginning game transition");
        StartCoroutine(TransitionToGame());
    }

    private IEnumerator TransitionToGame()
    {
        // Fade to black
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeCanvasAlpha(0f, 1f, fadeDuration));
        }

        if (inGameCanvasPrefab == null)
        {
            Debug.LogError("❌ InGameCanvas prefab NOT assigned in LoadingStartCanvas Inspector!");
            yield break;
        }

        // Instantiate the InGameCanvas prefab
        GameObject inGameCanvas = Instantiate(inGameCanvasPrefab);
        Debug.Log("✓ InGameCanvas prefab instantiated");

        if (PlayerManager.Instance == null)
        {
            Debug.LogError("❌ PlayerManager.Instance is null!");
            yield break;
        }

        // Initialize sliders
        PlayerManager.Instance.InitializeSliders(inGameCanvas);

        // Resume the game
        Time.timeScale = 1f;
        Debug.Log("▶️ Game resumed - gameplay started");

        // Fade from black to transparent (reveal game)
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeCanvasAlpha(1f, 0f, fadeDuration));
        }

        // Hide the loading canvas
        gameObject.SetActive(false);
    }

    private IEnumerator FadeCanvasAlpha(float startAlpha, float endAlpha, float duration)
    {
        if (fadePanel == null) yield break;

        float startTime = Time.realtimeSinceStartup;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.realtimeSinceStartup - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            fadePanel.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        fadePanel.alpha = endAlpha;
    }
}
