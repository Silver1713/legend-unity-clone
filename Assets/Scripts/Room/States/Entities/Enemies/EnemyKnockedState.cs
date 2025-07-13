using System.Collections;
using UnityEngine;

public class EnemyKnockedState : EnemyBaseState
{

    public bool isDamage = false;
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

       

        enemy.StartCoroutine(GoBack());

        //spawn damage indicator
      
    }

    private IEnumerator GoBack()
    {
        yield return new WaitForSeconds(.3f);
        enemy.Rigidbody2d.linearVelocity = Vector2.zero;
        enemy.Rigidbody2d.bodyType = RigidbodyType2D.Kinematic;
        if (isDamage)
        {
            enemy.TransitionToState((enemy.Stats.attackBehaviour == ATTACK_BEHAVIOUR.ATTACK_SWARM) ? enemy.SwarmState : enemy.WalkState);
        }
        else
        {
            enemy.TransitionToPreviousState();
        }
            isDamage = false; // Reset the damage flag
        IndicatorSpawn = false; // Reset the indicator spawn flag

    }

    
}
