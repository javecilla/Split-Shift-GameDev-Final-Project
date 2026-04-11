using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public Transform CurrentPlayer;
    public GameObject Jax;
    public GameObject Axel;
    public GameObject cameraObject; 

    public bool isJax = true;
    private bool isGameOver = false;

    public Slider healthBarSlider;
    public Slider manaBarSlider;

    // Centralized resource metrics
    public double PlayerHealth = 100;
    public double PlayerMana = 0;

    private float resourceCycleTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Axel.SetActive(false);
        CurrentPlayer = Jax.transform;
        UpdateTrackingTarget(Jax.transform);
    }

    public void InitializeSliders(GameObject inGameCanvas)
    {
        // Find Health Bar by searching for it by name
        Transform playerPanelTransform = inGameCanvas.transform.Find("Player Panel");
        if (playerPanelTransform == null)
        {
            Debug.LogError("Player Panel not found in InGameCanvas!");
            return;
        }

        // Find Health Bar Slider
        Transform healthBarTransform = playerPanelTransform.Find("Health Bar");
        if (healthBarTransform != null)
        {
            healthBarSlider = healthBarTransform.GetComponent<Slider>();
            if (healthBarSlider == null)
            {
                Debug.LogError("Slider component not found on Health Bar GameObject!");
            }
            else
            {
                Debug.Log("✓ Health Bar Slider found successfully");
            }
        }
        else
        {
            Debug.LogError("Health Bar GameObject not found under Player Panel!");
        }

        // Find Mana Bar Slider
        Transform manaBarTransform = playerPanelTransform.Find("Mana Bar");
        if (manaBarTransform != null)
        {
            manaBarSlider = manaBarTransform.GetComponent<Slider>();
            if (manaBarSlider == null)
            {
                Debug.LogError("Slider component not found on Mana Bar GameObject!");
            }
            else
            {
                Debug.Log("✓ Mana Bar Slider found successfully");
            }
        }
        else
        {
            Debug.LogError("Mana Bar GameObject not found under Player Panel!");
        }

        // Initialize sliders if found
        if (healthBarSlider != null && manaBarSlider != null)
        {
            healthBarSlider.minValue = 0;
            healthBarSlider.maxValue = 100;
            manaBarSlider.minValue = 0;
            manaBarSlider.maxValue = 100;

            UpdateHUD();
            Debug.Log("✓ All sliders initialized successfully!");
        }
        else
        {
            Debug.LogError("❌ Failed to initialize sliders - sliders are null!");
        }
    }

    public void UpdateHUD()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = (float)PlayerHealth;
        
        if (manaBarSlider != null)
            manaBarSlider.value = (float)PlayerMana;
    }


    void Update()
    {
        HandlePauseInput();

        if (PlayerHealth <= 0)
        {
            if (!isGameOver)
            {
                isGameOver = true;
                GameManager.Instance.ShowGameOver();
            }
            return; // Halt logic if dead
        }

        ExecuteResourceLifecycle();
        HandleSwitchInput();
    }

    private void HandlePauseInput()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GameManager.Instance.TogglePause();
        }
    }

    private void ExecuteResourceLifecycle()
    {
        resourceCycleTimer += Time.deltaTime;

        if (resourceCycleTimer >= 2f) //every 2s
        {
            resourceCycleTimer = 0f;

            if (PlayerMana < 100)
            {
                PlayerMana += 10;
                Debug.Log("Mana increased: " + PlayerMana);

                if (PlayerMana >= 100)
                {
                    PlayerMana = 100;
                    Debug.Log("need to switch");
                }
            }
            else if (PlayerMana >= 100)
            {
                PlayerHealth -= 10;
                Debug.Log("Health penalty applied. Current Health: " + PlayerHealth);

                if (PlayerHealth <= 0)
                {
                    Debug.Log("Your dead");
                }
            }

            UpdateHUD(); // update after every tick
        }
    }

    private void HandleSwitchInput()
    {
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            SwitchCharacter();
        }
    }

    public void UIJump()
    {
        CurrentPlayer?.GetComponent<PlayerBehavior>()?.PerformJump();
    }

    public void UIDash()
    {
        CurrentPlayer?.GetComponent<PlayerBehavior>()?.TriggerDash();
    }

    public void UIAttack()
    {
        CurrentPlayer?.GetComponent<PlayerBehavior>()?.PerformAttack();
    }

    public void UIMove(float x)
    {
        CurrentPlayer?.GetComponent<PlayerBehavior>()?.MoveX(x);
    }


    public void SwitchCharacter()
    {
        if (PlayerMana >= 100)
        {
            // Reset metrics upon successful switch
            PlayerMana = 0;
            resourceCycleTimer = 0f; 

            UpdateHUD();

            if (isJax)
            {
                ShiftToAxel();
                isJax = false;
            }
            else
            {
                ShiftToJax();
                isJax = true;
            }
        }
        else
        {
            Debug.Log("Switch denied: Mana is not 100");
        }
    }

    public void ShiftToAxel()
    {
        Vector3 currentPosition = Jax.transform.position;
        Jax.SetActive(false);
        Axel.transform.position = currentPosition; 
        Axel.SetActive(true);
        CurrentPlayer = Axel.transform;
        UpdateTrackingTarget(Axel.transform); 
    }

    public void ShiftToJax()
    {
        Vector3 currentPosition = Axel.transform.position;
        Axel.SetActive(false);
        Jax.transform.position = currentPosition; 
        Jax.SetActive(true);
        CurrentPlayer = Jax.transform;
        UpdateTrackingTarget(Jax.transform); 
    }

    private void UpdateTrackingTarget(Transform newTarget)
    {
        if (cameraObject != null)
        {
            CinemachineCamera cineCam = cameraObject.GetComponent<CinemachineCamera>();
            if (cineCam != null)
            {
                cineCam.Target.TrackingTarget = newTarget;
            }
        }
    }

    public void ResetGameState()
    {
        // Reset health and mana
        PlayerHealth = 100;
        PlayerMana = 0;
        resourceCycleTimer = 0f;
        isGameOver = false;

        // Reset to Jax
        if (!isJax)
        {
            ShiftToJax();
            isJax = true;
        }

        // Reset player position to spawn point (0, 0, 0)
        if (Jax != null)
        {
            Jax.transform.position = Vector3.zero;
        }
        if (Axel != null)
        {
            Axel.transform.position = Vector3.zero;
        }

        // Update HUD
        UpdateHUD();

        Debug.Log("✓ Game state reset - ready for new session");
    }
}