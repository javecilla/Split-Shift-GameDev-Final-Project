using UnityEngine;

/// <summary>
/// KillZone Trigger - Alternative to fall threshold
/// Place this script on a trigger collider at the bottom of your map.
/// When the player touches it, they instantly die.
/// </summary>
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerBehavior>() != null)
        {
            Debug.Log("Player entered KillZone!");
            PlayerManager.Instance.PlayerHealth = 0;
            PlayerManager.Instance.UpdateHUD();
        }
    }
}
