using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float speed = 4.5f;
    public Vector2 direction;

    private void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
