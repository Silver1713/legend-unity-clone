using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float damage = 1.0f;
    private float lifetime = 3.0f;
    private float timer = 0f;
    
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    
    public void SetLifetime(float lifetime)
    {
        this.lifetime = lifetime;
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateManager player = other.GetComponent<PlayerStateManager>();
            if (player != null)
            {
                // Apply damage to player
                // player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}