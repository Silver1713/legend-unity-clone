using UnityEngine;




public enum ATTACK_BEHAVIOUR
{
    ATTACK_SWARM,
    ATTACK_RANGE,
    ATTACK_CONTACT
}
[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/Entity/Enemy")]
public class EnemyStats : EntityData
{
    [Header("Attack Behaviours")]
    [SerializeField]
    public ATTACK_BEHAVIOUR attackBehaviour;

    [Header("Attack Stats")]
    public float baseAttack = 10.0f;
    public float attackSpeed = 10.0f;
    public float attackRange = 3.0f;
    public float attackCooldown = .50f;


    [Header("Vision Stats")]
    public float visionRange = 5.0f;
    public float visionAngle = 45.0f; // In degrees, 45 means 90 degrees of vision (45 left and 45 right)



}
