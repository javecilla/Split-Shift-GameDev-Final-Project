using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the menu button to display the Menu Canvas without pausing the game.
/// Attach this to the menu button GameObject in your scene.
/// Game continues running (Time.timeScale remains 1) while menu is open.
/// </summary>
public class MenuButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvasPrefab;

    private void Start()
    {
        // Programmatically set up the onClick listener
        Button menuButton = GetComponent<Button>();
        
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButtonClicked);
            Debug.Log("Menu button onClick listener set up successfully");
        }
        else
        {
            Debug.LogError("MenuButtonManager: No Button component found on this GameObject!");
        }
    }

    private void OnMenuButtonClicked()
    {
        if (menuCanvasPrefab == null)
        {
            Debug.LogError("❌ Menu Canvas prefab NOT assigned in MenuButtonManager Inspector!");
            return;
        }

        // Instantiate Menu Canvas WITHOUT pausing the game
        GameObject menuCanvas = Instantiate(menuCanvasPrefab);
        Debug.Log("✓ Menu Canvas instantiated - game continues running");
    }
}
