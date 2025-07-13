using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(EntityStateManager entity) : base(entity) { }

    public override void EnterState()
    {
        player.Animator.SetBool("IsWalking", true);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            player.TransitionToState(player.SwingSwordState);
        }
      

        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        // Allow the character to move one direction at time!
        Vector2 movement = Vector2.zero;
        if (horizontalMovement != 0.0f)
        {
            movement.x = horizontalMovement;

            if (horizontalMovement < 0)
            {
                player.facing = PlayerStateManager.PLAYER_FACE.FACE_LEFT;
            }
            else if (horizontalMovement > 0)
            {
                player.facing = PlayerStateManager.PLAYER_FACE.FACE_RIGHT;
            }
        }
        else if (verticalMovement != 0.0f)
        {
            movement.y = verticalMovement;

            if (verticalMovement < 0)
            {
                player.facing = PlayerStateManager.PLAYER_FACE.FACE_DOWN;
            }
            else if (verticalMovement > 0)
            {
                player.facing = PlayerStateManager.PLAYER_FACE.FACE_UP;
            }
        }

        if (Input.GetKeyDown(player.ProjectileInput) && player.ProjectileCooldown <= 0.0f)
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
                case PlayerStateManager.PLAYER_FACE.FACE_LEFT:
                    projectileComponent.direction = Projectile.DIRECTION.LEFT;
                    break;
                case PlayerStateManager.PLAYER_FACE.FACE_RIGHT:
                    projectileComponent.direction = Projectile.DIRECTION.RIGHT;
                    break;
                case PlayerStateManager.PLAYER_FACE.FACE_UP:
                    projectileComponent.direction = Projectile.DIRECTION.UP;
                    break;

            }

        }

        // Update the Animator only when motion
        if (movement != Vector2.zero)
        {
            player.Direction = movement;
            player.Animator.SetFloat("MoveX", horizontalMovement);
            player.Animator.SetFloat("MoveY", verticalMovement);
        }
        else
        {
            // No motion, transition to Idle keeping the previous animation
            player.TransitionToState(player.IdleState);
        }
    }

    public override void FixedUpdate()
    {
        player.KinematicController.MovePosition(player.Direction, player.WalkSpeed);
    }
}
