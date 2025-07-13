using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(EntityStateManager entity) : base(entity){}

    public override void EnterState()
    {
        // Transition Animator to Idle (See Animator states graph)
        player.Animator.SetBool("IsWalking", false);
    }

    public override void Update()
    {
        // If any Input direction, transition to walking state
        if (Input.GetAxisRaw("Horizontal") != 0.0f || 
            Input.GetAxisRaw("Vertical") != 0.0f)
        {
            // Set current face
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                player.facing = PlayerStateManager.PLAYER_FACE.FACE_LEFT;

            } else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                player.facing = PlayerStateManager.PLAYER_FACE.FACE_RIGHT;

            } else if (Input.GetAxisRaw("Vertical") > 0)
            {
                player.facing = PlayerStateManager.PLAYER_FACE.FACE_UP;

            } else if (Input.GetAxisRaw("Vertical") < 0)
            {
                player.facing = PlayerStateManager.PLAYER_FACE.FACE_DOWN;
            }
                player.TransitionToState(player.WalkState);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {

            
            
            player.TransitionToState(player.SwingSwordState);

        } else if (Input.GetKeyDown(player.ProjectileInput) && player.ProjectileCooldown <= 0.0f)
        {
            GameObject projectile = GameObject.Instantiate(player.equippedRangedWeapon.projectilePrefab, player.projectilesCtn.transform);
            projectile.transform.position = player.transform.position;
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            projectileComponent.speed = player.equippedRangedWeapon.baseAttackSpeed;
            projectileComponent.owner = player.gameObject;
            player.ProjectileCooldown = player.playerStats.Projectile_Cooldown;
            switch (player.facing)
            {
                case PlayerStateManager.PLAYER_FACE.FACE_DOWN:
                    projectileComponent.direction = Projectile.DIRECTION.DOWN;
                    break;
                case PlayerStateManager.PLAYER_FACE.FACE_UP:
                    projectileComponent.direction = Projectile.DIRECTION.UP;
                    break;
                case PlayerStateManager.PLAYER_FACE.FACE_LEFT:
                    projectileComponent.direction = Projectile.DIRECTION.LEFT;
                    break;
                case PlayerStateManager.PLAYER_FACE.FACE_RIGHT:
                    projectileComponent.direction = Projectile.DIRECTION.RIGHT;
                    break;
            }
        }
    }
}
