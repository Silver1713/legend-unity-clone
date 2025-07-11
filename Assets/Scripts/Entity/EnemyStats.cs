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
    
}
