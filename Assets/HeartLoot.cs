using UnityEngine;

public class HeartLoot : MonoBehaviour
{
    private const float HEALTH_RESTORE = 10f;
    private const float MAX_HEALTH = 100f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if collision is with the player
        PlayerBehavior player = collision.GetComponent<PlayerBehavior>();
        if (player != null)
        {
            RestorePlayerHealth();
            Destroy(gameObject);
        }
    }

    private void RestorePlayerHealth()
    {
        PlayerManager.Instance.PlayerHealth += HEALTH_RESTORE;

        // Cap health at maximum
        if (PlayerManager.Instance.PlayerHealth > MAX_HEALTH)
        {
            PlayerManager.Instance.PlayerHealth = MAX_HEALTH;
        }

        PlayerManager.Instance.UpdateHUD();
        Debug.Log("Player collected heart loot! Health restored: +" + HEALTH_RESTORE + ". Current health: " + PlayerManager.Instance.PlayerHealth);
    }
}
