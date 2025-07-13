using UnityEngine;

public class PlayerDiedState : PlayerBaseState
{
    public PlayerDiedState(EntityStateManager entity) : base(entity) { }

    public override void EnterState()
    {
        // Transition Animator to Idle (See Animator states graph)
        player.Animator.SetBool("IsWalking", false);
    }

    public override void Update()
    {
        
    }
}