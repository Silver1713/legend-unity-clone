using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum DIRECTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public GameObject owner; // The owner of the projectile, e.g., the player or an enemy
    public DIRECTION direction;
    public float speed = 30f;
    public float lifetime = 3f;
    private Rigidbody2D rb;
    private Collider2D collider2D;

    private Vector2 _dir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the projectile after a certain time
    }

    // Update is called once per frame
    void Update()
    {
        switch (direction)
        {
            case DIRECTION.UP:
                // Set the projectile's velocity based on the direction
                transform.rotation = Quaternion.Euler(0, 0, 90);
                rb.linearVelocity = Vector2.up * speed;
                _dir = Vector2.up;
                break;
            case DIRECTION.DOWN:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                rb.linearVelocity = Vector2.down * speed;
                _dir = Vector2.down;
                break;
            case DIRECTION.LEFT:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                rb.linearVelocity = Vector2.left * speed;
                _dir = Vector2.left;
                break;
            case DIRECTION.RIGHT:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                rb.linearVelocity = Vector2.right * speed;

                _dir = Vector2.right;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Assuming the enemy has a method to handle being hit
            other.GetComponent<EnemyStateManager>().Hit(_dir, 20.0f);
            GameManager.Instance.Ranged(20.0f); // Notify the game manager about the hit
            Destroy(gameObject); // Destroy the projectile on hit
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject); // Destroy the projectile on wall collision
        }
    }
}
