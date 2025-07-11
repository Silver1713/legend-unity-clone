using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public float spawnWeight = 1.0f;
    public int minLevel = 1;
    public int maxLevel = 10;
    public bool canSwarm = false;
    public bool isRanged = false;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    public List<EnemySpawnData> enemyTypes = new List<EnemySpawnData>();
    public int minEnemiesPerRoom = 1;
    public int maxEnemiesPerRoom = 4;
    public float spawnRadius = 2.0f;
    public int roomLevel = 1;
    
    [Header("Spawn Areas")]
    public List<Vector2> spawnPoints = new List<Vector2>();
    
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    public void SpawnEnemiesInRoom(RectInt roomBounds, int currentRoomLevel)
    {
        roomLevel = currentRoomLevel;
        ClearExistingEnemies();
        
        int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
        List<Vector2> validSpawnPositions = GenerateSpawnPositions(roomBounds, enemyCount);
        
        for (int i = 0; i < enemyCount && i < validSpawnPositions.Count; i++)
        {
            EnemySpawnData selectedEnemy = SelectEnemyType();
            if (selectedEnemy?.enemyPrefab != null)
            {
                GameObject spawnedEnemy = SpawnEnemy(selectedEnemy, validSpawnPositions[i]);
                if (spawnedEnemy != null)
                {
                    spawnedEnemies.Add(spawnedEnemy);
                    ConfigureEnemyBehavior(spawnedEnemy, selectedEnemy);
                }
            }
        }
    }
    
    private List<Vector2> GenerateSpawnPositions(RectInt roomBounds, int count)
    {
        List<Vector2> positions = new List<Vector2>();
        int attempts = 0;
        int maxAttempts = count * 10;
        
        while (positions.Count < count && attempts < maxAttempts)
        {
            attempts++;
            
            Vector2 candidatePosition = new Vector2(
                Random.Range(roomBounds.x + 1, roomBounds.x + roomBounds.width - 1),
                Random.Range(roomBounds.y + 1, roomBounds.y + roomBounds.height - 1)
            );
            
            if (IsValidSpawnPosition(candidatePosition, positions))
            {
                positions.Add(candidatePosition);
            }
        }
        
        return positions;
    }
    
    private bool IsValidSpawnPosition(Vector2 position, List<Vector2> existingPositions)
    {
        foreach (Vector2 existingPos in existingPositions)
        {
            if (Vector2.Distance(position, existingPos) < spawnRadius)
            {
                return false;
            }
        }
        
        Collider2D obstacle = Physics2D.OverlapCircle(position, 0.5f, LayerMask.GetMask("Walls", "Obstacles"));
        return obstacle == null;
    }
    
    private EnemySpawnData SelectEnemyType()
    {
        List<EnemySpawnData> validEnemies = new List<EnemySpawnData>();
        float totalWeight = 0f;
        
        foreach (EnemySpawnData enemy in enemyTypes)
        {
            if (roomLevel >= enemy.minLevel && roomLevel <= enemy.maxLevel)
            {
                validEnemies.Add(enemy);
                totalWeight += enemy.spawnWeight;
            }
        }
        
        if (validEnemies.Count == 0) return null;
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (EnemySpawnData enemy in validEnemies)
        {
            currentWeight += enemy.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return enemy;
            }
        }
        
        return validEnemies[validEnemies.Count - 1];
    }
    
    private GameObject SpawnEnemy(EnemySpawnData enemyData, Vector2 position)
    {
        GameObject enemy = Instantiate(enemyData.enemyPrefab, position, Quaternion.identity);
        enemy.transform.SetParent(transform);
        return enemy;
    }
    
    private void ConfigureEnemyBehavior(GameObject enemy, EnemySpawnData enemyData)
    {
        EnemyStateManager stateManager = enemy.GetComponent<EnemyStateManager>();
        if (stateManager != null)
        {
            if (enemyData.canSwarm)
            {
                // Check if there are other enemies nearby to form a swarm
                Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(enemy.transform.position, 4.0f);
                int nearbyCount = 0;
                
                foreach (Collider2D col in nearbyEnemies)
                {
                    if (col.GetComponent<EnemyStateManager>() != null && col.gameObject != enemy)
                    {
                        nearbyCount++;
                    }
                }
                
                // If there are 2 or more enemies nearby, use swarm behavior
                if (nearbyCount >= 1)
                {
                    // Add swarm state to enemy if not already present
                    if (stateManager.GetComponent<EnemySwarmState>() == null)
                    {
                        // You would need to modify EnemyStateManager to support swarm state
                        // For now, we'll use the existing walk state
                    }
                }
            }
            
            if (enemyData.isRanged)
            {
                // Configure for ranged behavior
                // You would need to modify EnemyStateManager to support ranged state
                // For now, we'll use the existing walk state
            }
            
            // Scale stats based on room level
            if (stateManager.Stats != null)
            {
                float levelMultiplier = 1.0f + (roomLevel - 1) * 0.2f;
                stateManager.Stats.health *= levelMultiplier;
                stateManager.Stats.damage *= levelMultiplier;
                stateManager.Stats.speed *= Mathf.Min(levelMultiplier, 1.5f); // Cap speed increase
            }
        }
    }
    
    public void ClearExistingEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }
    
    public int GetSpawnedEnemyCount()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        return spawnedEnemies.Count;
    }
    
    public List<GameObject> GetSpawnedEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        return new List<GameObject>(spawnedEnemies);
    }
}