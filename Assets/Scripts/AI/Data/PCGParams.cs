using UnityEngine;

[CreateAssetMenu(fileName = "PCGParams", menuName = "Scriptable Objects/AI/DDA/PCG")]
public class PCGParams : DDAParameters
{
    public int roomSize = 12;
    public string shapePattern = "Rect";
    public int trapCount = 3;
    public float enemyDensity = 0.25f;
    public float treasureChance = 0.2f;
    public float obstacleComplexity = 0.1f;

    [Header("Random Walk Parameters")]
    public int minSize;
    public int maxSize;
    public float cornerIntensity;
}
