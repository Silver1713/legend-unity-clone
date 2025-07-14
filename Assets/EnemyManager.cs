using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum EnemyType
    {
        Contact,
        Ranged,
        Swarm
    }
    public GameObject rushType;
    public GameObject rangedType;
    public GameObject swarmType;
    public Dictionary<EnemyType, GameObject> enemyTypes;
    public SpawnParameters spawnParameters;



    public static EnemyManager Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if  (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        enemyTypes = new Dictionary<EnemyType, GameObject>
        {
            { EnemyType.Contact, rushType },
            { EnemyType.Ranged, rangedType },
            { EnemyType.Swarm, swarmType }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject GetEnemyPrefab()
    {
        if (enemyTypes.TryGetValue(SelectEnemyType(), out GameObject prefab))
        {
            return prefab;
        }
        return null; // or handle the case where the type is not found
    }


    public EnemyType SelectEnemyType()
    {
        float totalWeight = 0f;
        foreach (var pair in spawnParameters.weights)
        {
            totalWeight += pair.Value;
        }
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        foreach (var pair in spawnParameters.weights)
        {
            cumulativeWeight += pair.Value;
            if (randomValue <= cumulativeWeight)
            {
                return pair.Key; // Return the selected enemy type
            }
        }
        return EnemyType.Contact; // Default fallback
    }


    public int GetNumberOfEnemies()
    {
        return spawnParameters.GetNumberEnemy(spawnParameters.testWeight);
    }
}
