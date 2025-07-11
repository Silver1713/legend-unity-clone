using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObstacleData
{
    public GameObject obstaclePrefab;
    public float spawnWeight = 1.0f;
    public Vector2Int size = Vector2Int.one;
    public bool blocksMovement = true;
    public bool blocksProjectiles = true;
    public bool isDestructible = false;
}

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Obstacle Configuration")]
    public List<ObstacleData> obstacleTypes = new List<ObstacleData>();
    public float obstacleDensity = 0.15f; // Percentage of room to fill with obstacles
    public int minObstacles = 2;
    public int maxObstacles = 8;
    public float minDistanceFromDoors = 2.0f;
    public float minDistanceFromSpawn = 3.0f;
    
    [Header("Pattern Generation")]
    public bool usePatterns = true;
    public bool generateClusters = true;
    public bool generatePaths = true;
    
    private List<GameObject> generatedObstacles = new List<GameObject>();
    
    public void GenerateObstacles(RectInt roomBounds, List<Vector2> doorPositions, Vector2 playerSpawnPoint)
    {
        ClearExistingObstacles();
        
        if (obstacleTypes.Count == 0) return;
        
        int obstacleCount = CalculateObstacleCount(roomBounds);
        List<Vector2Int> validPositions = FindValidObstaclePositions(roomBounds, doorPositions, playerSpawnPoint);
        
        if (usePatterns)
        {
            GeneratePatternedObstacles(validPositions, obstacleCount);
        }
        else
        {
            GenerateRandomObstacles(validPositions, obstacleCount);
        }
    }
    
    private int CalculateObstacleCount(RectInt roomBounds)
    {
        int roomArea = roomBounds.width * roomBounds.height;
        int targetCount = Mathf.RoundToInt(roomArea * obstacleDensity);
        return Mathf.Clamp(targetCount, minObstacles, maxObstacles);
    }
    
    private List<Vector2Int> FindValidObstaclePositions(RectInt roomBounds, List<Vector2> doorPositions, Vector2 playerSpawnPoint)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();
        
        for (int x = roomBounds.x + 1; x < roomBounds.x + roomBounds.width - 1; x++)
        {
            for (int y = roomBounds.y + 1; y < roomBounds.y + roomBounds.height - 1; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                
                if (IsValidObstaclePosition(position, doorPositions, playerSpawnPoint))
                {
                    validPositions.Add(position);
                }
            }
        }
        
        return validPositions;
    }
    
    private bool IsValidObstaclePosition(Vector2Int position, List<Vector2> doorPositions, Vector2 playerSpawnPoint)
    {
        // Check distance from doors
        foreach (Vector2 doorPos in doorPositions)
        {
            if (Vector2.Distance(position, doorPos) < minDistanceFromDoors)
            {
                return false;
            }
        }
        
        // Check distance from player spawn
        if (Vector2.Distance(position, playerSpawnPoint) < minDistanceFromSpawn)
        {
            return false;
        }
        
        // Check if position is already occupied
        Collider2D existing = Physics2D.OverlapPoint(position, LayerMask.GetMask("Obstacles", "Walls"));
        return existing == null;
    }
    
    private void GeneratePatternedObstacles(List<Vector2Int> validPositions, int targetCount)
    {
        int placedCount = 0;
        
        if (generateClusters)
        {
            placedCount += GenerateClusters(validPositions, targetCount / 2);
        }
        
        if (generatePaths)
        {
            placedCount += GeneratePathObstacles(validPositions, (targetCount - placedCount) / 2);
        }
        
        // Fill remaining with random placement
        int remaining = targetCount - placedCount;
        GenerateRandomObstacles(validPositions, remaining);
    }
    
    private int GenerateClusters(List<Vector2Int> validPositions, int maxClusters)
    {
        int clustersGenerated = 0;
        int obstaclesPlaced = 0;
        
        while (clustersGenerated < maxClusters && validPositions.Count > 0)
        {
            Vector2Int clusterCenter = validPositions[Random.Range(0, validPositions.Count)];
            int clusterSize = Random.Range(2, 5);
            
            List<Vector2Int> clusterPositions = GetClusterPositions(clusterCenter, clusterSize, validPositions);
            
            foreach (Vector2Int pos in clusterPositions)
            {
                if (PlaceObstacle(pos))
                {
                    obstaclesPlaced++;
                    validPositions.Remove(pos);
                }
            }
            
            clustersGenerated++;
        }
        
        return obstaclesPlaced;
    }
    
    private List<Vector2Int> GetClusterPositions(Vector2Int center, int size, List<Vector2Int> validPositions)
    {
        List<Vector2Int> clusterPositions = new List<Vector2Int>();
        clusterPositions.Add(center);
        
        for (int i = 1; i < size; i++)
        {
            Vector2Int newPos = FindNearestValidPosition(clusterPositions, validPositions, 1.5f);
            if (newPos != Vector2Int.zero)
            {
                clusterPositions.Add(newPos);
            }
        }
        
        return clusterPositions;
    }
    
    private int GeneratePathObstacles(List<Vector2Int> validPositions, int maxPaths)
    {
        int pathsGenerated = 0;
        int obstaclesPlaced = 0;
        
        while (pathsGenerated < maxPaths && validPositions.Count > 4)
        {
            Vector2Int start = validPositions[Random.Range(0, validPositions.Count)];
            Vector2Int end = validPositions[Random.Range(0, validPositions.Count)];
            
            if (Vector2Int.Distance(start, end) > 3)
            {
                List<Vector2Int> pathPositions = GeneratePathBetween(start, end, validPositions);
                
                foreach (Vector2Int pos in pathPositions)
                {
                    if (PlaceObstacle(pos))
                    {
                        obstaclesPlaced++;
                        validPositions.Remove(pos);
                    }
                }
                
                pathsGenerated++;
            }
        }
        
        return obstaclesPlaced;
    }
    
    private List<Vector2Int> GeneratePathBetween(Vector2Int start, Vector2Int end, List<Vector2Int> validPositions)
    {
        List<Vector2Int> pathPositions = new List<Vector2Int>();
        
        Vector2Int current = start;
        Vector2Int direction = new Vector2Int(
            Mathf.RoundToInt(Mathf.Sign(end.x - start.x)),
            Mathf.RoundToInt(Mathf.Sign(end.y - start.y))
        );
        
        while (current != end && pathPositions.Count < 6)
        {
            if (validPositions.Contains(current))
            {
                pathPositions.Add(current);
            }
            
            if (Random.Range(0f, 1f) > 0.5f)
            {
                current.x += direction.x;
            }
            else
            {
                current.y += direction.y;
            }
            
            if (current.x == end.x) current.y += direction.y;
            if (current.y == end.y) current.x += direction.x;
        }
        
        return pathPositions;
    }
    
    private void GenerateRandomObstacles(List<Vector2Int> validPositions, int count)
    {
        for (int i = 0; i < count && validPositions.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            Vector2Int position = validPositions[randomIndex];
            
            if (PlaceObstacle(position))
            {
                validPositions.RemoveAt(randomIndex);
            }
        }
    }
    
    private bool PlaceObstacle(Vector2Int position)
    {
        ObstacleData selectedObstacle = SelectObstacleType();
        if (selectedObstacle?.obstaclePrefab != null)
        {
            GameObject obstacle = Instantiate(selectedObstacle.obstaclePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            obstacle.transform.SetParent(transform);
            
            ConfigureObstacle(obstacle, selectedObstacle);
            generatedObstacles.Add(obstacle);
            
            return true;
        }
        
        return false;
    }
    
    private ObstacleData SelectObstacleType()
    {
        if (obstacleTypes.Count == 0) return null;
        
        float totalWeight = 0f;
        foreach (ObstacleData obstacle in obstacleTypes)
        {
            totalWeight += obstacle.spawnWeight;
        }
        
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (ObstacleData obstacle in obstacleTypes)
        {
            currentWeight += obstacle.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return obstacle;
            }
        }
        
        return obstacleTypes[obstacleTypes.Count - 1];
    }
    
    private void ConfigureObstacle(GameObject obstacle, ObstacleData data)
    {
        // Add appropriate components based on obstacle data
        if (data.blocksMovement || data.blocksProjectiles)
        {
            Collider2D collider = obstacle.GetComponent<Collider2D>();
            if (collider == null)
            {
                collider = obstacle.AddComponent<BoxCollider2D>();
            }
            
            if (!data.blocksMovement)
            {
                collider.isTrigger = true;
            }
        }
        
        if (data.isDestructible)
        {
            DestructibleObstacle destructible = obstacle.AddComponent<DestructibleObstacle>();
            destructible.health = Random.Range(1, 4);
        }
        
        // Set layer
        obstacle.layer = LayerMask.NameToLayer("Obstacles");
    }
    
    private Vector2Int FindNearestValidPosition(List<Vector2Int> existingPositions, List<Vector2Int> validPositions, float maxDistance)
    {
        foreach (Vector2Int existing in existingPositions)
        {
            foreach (Vector2Int valid in validPositions)
            {
                if (Vector2Int.Distance(existing, valid) <= maxDistance && !existingPositions.Contains(valid))
                {
                    return valid;
                }
            }
        }
        
        return Vector2Int.zero;
    }
    
    public void ClearExistingObstacles()
    {
        foreach (GameObject obstacle in generatedObstacles)
        {
            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
        generatedObstacles.Clear();
    }
    
    public int GetObstacleCount()
    {
        generatedObstacles.RemoveAll(obstacle => obstacle == null);
        return generatedObstacles.Count;
    }
}