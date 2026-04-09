using UnityEngine;

public class SawTrapBehavior : MonoBehaviour
{
    [SerializeField] private float damageAmount = 5f;
    [SerializeField] private float damageCooldown = 0.5f;

    private float damageTimer = 0f;

    [Header("Audio")]
    public AudioClip damageSFX;
    private AudioSource aud;

    void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (damageTimer > 0)
            damageTimer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (damageTimer <= 0)
            {
                PlayerBehavior player = collision.GetComponent<PlayerBehavior>();
                if (player != null)
                {
                    player.TakeDamage(damageAmount);

                    // 🔊 Play trap damage sound
                    if (damageSFX != null && aud != null)
                        aud.PlayOneShot(damageSFX);

                    damageTimer = damageCooldown;
                }
            }
        }
    }
}
