using System.Collections;
using UnityEngine;

public class EnemyKnockedState : EnemyBaseState
{
    UIManager uiManager = UIManager.Instance;
    public EnemyKnockedState(EnemyStateManager entity) : base(entity) { }

    private bool IndicatorSpawn = false;



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
            uiManager.CreateDamageIndicator(enemy.transform, "Knocked", Color.red);
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
