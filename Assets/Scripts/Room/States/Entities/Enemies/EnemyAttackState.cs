using UnityEditor;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(EnemyStateManager entity) : base(entity) { }


    public override void EnterState()
    {
        Debug.Log("Enemy is attacking.");
        enemy.Player.playerStats.health -= enemy.Stats.baseAttack;
        UIManager.Instance.CreateDamageIndicator(enemy.Player.transform, $"{enemy.Stats.baseAttack}", Color.white);
        enemy.TransitionToState(enemy.WalkState);
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}