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
    

}
