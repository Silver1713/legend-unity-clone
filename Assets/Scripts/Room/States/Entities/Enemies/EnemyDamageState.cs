using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyDamageState : EnemyBaseState
{
    private bool IndicatorSpawn = false;
    private UIManager uiManager = UIManager.Instance;
    public float raw_damage;
    public EnemyDamageState(EnemyStateManager enemy) : base(enemy)
    {

    }
    
    public override void EnterState()
    {
        raw_damage = enemy.Stats.baseAttack;
        if (!IndicatorSpawn)
        {
            IndicatorSpawn = true;

            enemy.Stats.health -= 10;
            if (enemy.Stats.health <= 0.0f)
            {
                uiManager.CreateDamageIndicator(enemy.transform, $"{raw_damage}", Color.red);
                enemy.TransitionToState(enemy.DieState);

            }
            else
            {
                // Spawn damage indicator
                uiManager.CreateDamageIndicator(enemy.transform, $"{raw_damage}", Color.red);



            }

        }

        GoBack();



    }

    private void GoBack()
    {
        enemy.Rigidbody2d.linearVelocity = Vector2.zero;
        enemy.Rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
        enemy.KnockState.isDamage = true; // Set the damage flag to true
        enemy.TransitionToState(enemy.KnockState);

        IndicatorSpawn = false; // Reset the indicator spawn flag
    }

    public override void Update()
    {
        
    }
    
    
   
}