using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 4.5f;
    public Vector2 direction;
    public float damage = 25f;

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // Destroy on anything except the player itself
        if (!collision.CompareTag("Player"))
            Destroy(gameObject);
    }
}