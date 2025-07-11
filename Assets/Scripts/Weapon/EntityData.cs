using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/Entity/Stats")]
public class EntityData : ScriptableObject
{
    [Header("Entity Stats")]
    public int strength;
    public int defense;
    public int speed;
    public int health;
    public int maxHealth;
    
}
