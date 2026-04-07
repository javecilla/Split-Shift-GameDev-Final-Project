using UnityEngine;

public class SawTrapBehavior : MonoBehaviour
{
    [SerializeField] private float damageAmount = 5f;
    [SerializeField] private float damageCooldown = 0.5f;

    private float damageTimer = 0f;

    void Update()
    {
        // Tick down the damage cooldown timer
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.CompareTag("Player"))
        {
            // Deal damage if cooldown has elapsed
            if (damageTimer <= 0)
            {
                PlayerBehavior player = collision.GetComponent<PlayerBehavior>();
                if (player != null)
                {
                    player.TakeDamage(damageAmount);
                    damageTimer = damageCooldown;
                }
            }
        }
    }
}
