using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the pause button inside the Menu Canvas.
/// When clicked: pauses game + hides menu canvas.
/// Attach this script to the Pause Button inside Menu Canvas prefab.
/// </summary>
public class PauseButtonInMenuManager : MonoBehaviour
{
    private GameObject menuCanvas;

    private void Start()
    {
        Button pauseButton = GetComponent<Button>();
        
        if (pauseButton != null)
        {
            // Find the root Menu Canvas by looking for the highest Canvas component parent
            Canvas[] canvases = GetComponentsInParent<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.name.Contains("Menu Canvas") || canvas.name.Contains("menu"))
                {
                    menuCanvas = canvas.gameObject;
                    break;
                }
            }

            // If not found by name, use the root canvas
            if (menuCanvas == null && canvases.Length > 0)
            {
                menuCanvas = canvases[canvases.Length - 1].gameObject;
            }

            if (menuCanvas != null)
            {
                Debug.Log($"✓ Found Menu Canvas: {menuCanvas.name}");

                // Set up two onClick listeners:
                // 1. Pause the game + show Pause Canvas
                pauseButton.onClick.AddListener(() => 
                {
                    Debug.Log("Pause button clicked - pausing game");
                    GameManager.Instance.TogglePause();
                });
                
                // 2. Hide the menu canvas (delayed slightly to ensure pause canvas appears first)
                pauseButton.onClick.AddListener(() => 
                {
                    Debug.Log($"Hiding Menu Canvas: {menuCanvas.name}");
                    menuCanvas.SetActive(false);
                });
                
                Debug.Log("✓ Pause Button (in Menu) set up - will pause game + hide menu");
            }
            else
            {
                Debug.LogError("❌ Could not find parent Menu Canvas!");
            }
        }
        else
        {
            Debug.LogError("❌ PauseButtonInMenuManager: No Button component found!");
        }
    }
}
