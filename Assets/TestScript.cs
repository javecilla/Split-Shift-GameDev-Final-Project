using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float movementSpeed = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(moveHorizontal * movementSpeed, moveVertical * movementSpeed);
    }
}
