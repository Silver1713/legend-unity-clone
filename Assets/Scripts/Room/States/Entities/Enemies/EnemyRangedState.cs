using UnityEngine;

public class EnemyRangedState : EnemyBaseState
{
    private float attackRange = 5.0f;
    private float minRange = 2.0f;
    private float projectileSpeed = 8.0f;
    private float lastShotTime;
    private float shotCooldown = 1.5f;
    private bool isAiming;
    
    public EnemyRangedState(EnemyStateManager enemy) : base(enemy)
    {
    }
    
    public override void EnterState()
    {
        enemy.Animator.SetBool("IsWalking", false);
        isAiming = false;
        lastShotTime = 0f;
    }
    
    public override void Update()
    {
        if (enemy.Player == null) return;
        
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.Player.transform.position);
        
        if (distanceToPlayer > attackRange)
        {
            enemy.TransitionToState(enemy.WalkState);
            return;
        }
        
        if (distanceToPlayer < minRange)
        {
            MoveAwayFromPlayer();
        }
        else
        {
            AimAtPlayer();
            
            if (Time.time - lastShotTime > shotCooldown && HasLineOfSight())
            {
                ShootAtPlayer();
                lastShotTime = Time.time;
            }
        }
    }
    
    private void MoveAwayFromPlayer()
    {
        Vector2 directionAwayFromPlayer = (enemy.transform.position - enemy.Player.transform.position).normalized;
        Vector2 velocity = directionAwayFromPlayer * enemy.WalkSpeed * 0.7f;
        
        enemy.Rigidbody2d.linearVelocity = velocity;
        enemy.Direction = directionAwayFromPlayer;
        
        enemy.Animator.SetFloat("MoveX", enemy.Direction.x);
        enemy.Animator.SetFloat("MoveY", enemy.Direction.y);
        enemy.Animator.SetBool("IsWalking", true);
    }
    
    private void AimAtPlayer()
    {
        Vector2 directionToPlayer = (enemy.Player.transform.position - enemy.transform.position).normalized;
        enemy.Direction = directionToPlayer;
        
        enemy.Animator.SetFloat("MoveX", enemy.Direction.x);
        enemy.Animator.SetFloat("MoveY", enemy.Direction.y);
        enemy.Animator.SetBool("IsWalking", false);
        
        enemy.Rigidbody2d.linearVelocity = Vector2.zero;
        isAiming = true;
    }
    
    private bool HasLineOfSight()
    {
        Vector2 rayDirection = (enemy.Player.transform.position - enemy.transform.position).normalized;
        float distance = Vector2.Distance(enemy.transform.position, enemy.Player.transform.position);
        
        RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, rayDirection, distance, LayerMask.GetMask("Walls"));
        
        return hit.collider == null;
    }
    
    private void ShootAtPlayer()
    {
        GameObject projectile = CreateProjectile();
        if (projectile != null)
        {
            Vector2 direction = (enemy.Player.transform.position - enemy.transform.position).normalized;
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = direction * projectileSpeed;
            }
            
            EnemyProjectile projScript = projectile.GetComponent<EnemyProjectile>();
            if (projScript != null)
            {
                projScript.SetDamage(enemy.Stats.baseAttack);
                projScript.SetLifetime(3.0f);
            }
        }
    }
    
    private GameObject CreateProjectile()
    {
        GameObject projectilePrefab = Resources.Load<GameObject>("Prefabs/EnemyProjectile");
        if (projectilePrefab != null)
        {
            return Object.Instantiate(projectilePrefab, enemy.transform.position, Quaternion.identity);
        }
        
        GameObject simpleProjectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        simpleProjectile.transform.localScale = Vector3.one * 0.3f;
        simpleProjectile.transform.position = enemy.transform.position;
        
        Rigidbody2D rb = simpleProjectile.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        
        EnemyProjectile projScript = simpleProjectile.AddComponent<EnemyProjectile>();
        
        Collider2D collider = simpleProjectile.GetComponent<Collider2D>();
        collider.isTrigger = true;
        
        return simpleProjectile;
    }
}