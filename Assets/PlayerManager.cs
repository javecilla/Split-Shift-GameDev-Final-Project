using UnityEngine;
using Unity.Cinemachine; 

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public Transform CurrentPlayer;
    public GameObject Jax;
    public GameObject Axel;
    public GameObject cameraObject; 

    public bool isJax = true;

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

    void Update()
    {
        HandlePauseInput();

        if (PlayerHealth <= 0) return; // Halt logic if dead

        ExecuteResourceLifecycle();
        HandleSwitchInput();
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.TogglePause();
        }
    }

    private void ExecuteResourceLifecycle()
    {
        resourceCycleTimer += Time.deltaTime;

        if (resourceCycleTimer >= 3f)
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
        }
    }

    private void HandleSwitchInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
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

}