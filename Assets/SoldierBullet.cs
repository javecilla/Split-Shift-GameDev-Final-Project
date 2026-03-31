using UnityEngine;

public class SoldierBullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 20f;
    public Vector2 direction;

    void Start()
    {
        // Force bullet to always move left (since soldier never flips)
        direction = Vector2.left;
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerBehavior>()?.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (!collision.CompareTag("Enemy"))
            Destroy(gameObject);

        // Destroy on anything except the player itself
        if (!collision.CompareTag("Player"))
            Destroy(gameObject);
    }
}
