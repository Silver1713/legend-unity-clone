using UnityEditor;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(EnemyStateManager entity) : base(entity) { }


    public override void EnterState()
    {
        Debug.Log("Enemy is attacking.");
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