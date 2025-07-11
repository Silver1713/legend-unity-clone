using System.Collections;
using UnityEngine;

public class EnemyKnockedState : EnemyBaseState
{

    public float raw_damage;
    private UIManager uiManager = UIManager.Instance;


    private bool IndicatorSpawn = false;

    public EnemyKnockedState(EnemyStateManager entity) : base(entity) { }


    public override void EnterState()
    {
        enemy.Animator.SetBool("IsWalking", false);

        // Face the player
        enemy.Animator.SetFloat("MoveX", enemy.HitDirection.x * -1);
        enemy.Animator.SetFloat("MoveY", enemy.HitDirection.y * -1);

        enemy.Rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
        enemy.Rigidbody2d.AddForce(enemy.HitDirection * enemy.ThrustForce, ForceMode2D.Impulse);

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

        enemy.StartCoroutine(GoWalking());

        //spawn damage indicator
      
    }

    private IEnumerator GoWalking()
    {
        yield return new WaitForSeconds(.3f);
        enemy.Rigidbody2d.linearVelocity = Vector2.zero;
        enemy.Rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
        enemy.TransitionToState(enemy.WalkState);
        IndicatorSpawn = false; // Reset the indicator spawn flag

    }

    
}
