using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    public GameObject dashButton;
    public GameObject jaxProfile;
    public GameObject axelProfile;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (PlayerManager.Instance == null) return;

        bool isJax = PlayerManager.Instance.isJax;

        dashButton.SetActive(!isJax);
        jaxProfile.SetActive(isJax);
        axelProfile.SetActive(!isJax);
    }
}