using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    public int health = 2;
    public GameObject destroyEffect;
    public bool dropLoot = false;
    public GameObject[] lootPrefabs;
    
    private void Start()
    {
        // Ensure the obstacle has a collider
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        // Visual feedback for damage
        StartCoroutine(FlashRed());
        
        if (health <= 0)
        {
            DestroyObstacle();
        }
    }
    
    private System.Collections.IEnumerator FlashRed()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.color;
            renderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            renderer.color = originalColor;
        }
    }
    
    private void DestroyObstacle()
    {
        // Spawn destroy effect
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }
        
        // Drop loot
        if (dropLoot && lootPrefabs.Length > 0)
        {
            GameObject loot = lootPrefabs[Random.Range(0, lootPrefabs.Length)];
            Instantiate(loot, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle projectile collisions
        if (other.CompareTag("PlayerProjectile"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }
}