using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PCGParams", menuName = "Scriptable Objects/AI/DDA/Enemy")]
public class SpawnParameters : DDAParameters
{
    public int minEnemies;
    public int maxEnemies;

    public float minHealth = 30.0f;
    public float maxHealth = 100.0f;



    public float testWeight = 0.443f;
    [SerializeField]public List<SKeyValuePair<EnemyManager.EnemyType, float>> weights;




    public float DetermineHelth(float weight)
    {
        // Use the weight to determine the health of the enemy
        float health = Mathf.Lerp(minHealth, maxHealth, weight);
        return Mathf.Clamp(health, minHealth, maxHealth);
    }

    public int GetNumberEnemy(float weight)
    {
        // Use the weight to determine the number of enemies to spawn
        int numberOfEnemies = Mathf.RoundToInt(Mathf.Lerp(minEnemies, maxEnemies, weight));
        return Mathf.Clamp(numberOfEnemies, minEnemies, maxEnemies);

    }


}
