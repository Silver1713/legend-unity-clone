using UnityEngine;
using System.Collections.Generic;

public class EnemySwarmState : EnemyBaseState
{
    private float swarmRadius = 3.0f;
    private float separationForce = 2.0f;
    private float cohesionForce = 1.0f;
    private float alignmentForce = 1.5f;
    private float playerSeekForce = 2.5f;
    private List<EnemyStateManager> nearbyEnemies;
    
    public EnemySwarmState(EnemyStateManager enemy) : base(enemy)
    {
        nearbyEnemies = new List<EnemyStateManager>();
    }
    
    public override void EnterState()
    {
        enemy.Animator.SetBool("IsWalking", true);
    }
    
    public override void Update()
    {
        if (enemy.Player == null) return;
        
        FindNearbyEnemies();
        Vector2 swarmForce = CalculateSwarmForce();
        Vector2 playerForce = SeekPlayer();
        
        Vector2 totalForce = swarmForce + playerForce;
        totalForce = Vector2.ClampMagnitude(totalForce, enemy.WalkSpeed);
        
        enemy.Rigidbody2d.linearVelocity = totalForce;
        
        if (totalForce.magnitude > 0.1f)
        {
            enemy.Direction = totalForce.normalized;
            enemy.Animator.SetFloat("MoveX", enemy.Direction.x);
            enemy.Animator.SetFloat("MoveY", enemy.Direction.y);
        }
        
        float distanceToPlayer = Vector2.Distance(enemy.transform.position, enemy.Player.transform.position);
        if (distanceToPlayer < 1.5f && enemy.COOLDOWN <= 0)
        {
            enemy.TransitionToState(enemy.AttackState);
        }
    }
    
    private void FindNearbyEnemies()
    {
        nearbyEnemies.Clear();
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.transform.position, swarmRadius);
        
        foreach (Collider2D collider in colliders)
        {
            EnemyStateManager otherEnemy = collider.GetComponent<EnemyStateManager>();
            if (otherEnemy != null && otherEnemy != enemy)
            {
                nearbyEnemies.Add(otherEnemy);
            }
        }
    }
    
    private Vector2 CalculateSwarmForce()
    {
        if (nearbyEnemies.Count == 0) return Vector2.zero;
        
        Vector2 separation = CalculateSeparation();
        Vector2 cohesion = CalculateCohesion();
        Vector2 alignment = CalculateAlignment();
        
        return separation * separationForce + cohesion * cohesionForce + alignment * alignmentForce;
    }
    
    private Vector2 CalculateSeparation()
    {
        Vector2 separationForce = Vector2.zero;
        
        foreach (EnemyStateManager otherEnemy in nearbyEnemies)
        {
            Vector2 direction = enemy.transform.position - otherEnemy.transform.position;
            float distance = direction.magnitude;
            
            if (distance > 0 && distance < 1.5f)
            {
                direction.Normalize();
                direction /= distance;
                separationForce += direction;
            }
        }
        
        return separationForce;
    }
    
    private Vector2 CalculateCohesion()
    {
        if (nearbyEnemies.Count == 0) return Vector2.zero;
        
        Vector2 centerOfMass = Vector2.zero;
        
        foreach (EnemyStateManager otherEnemy in nearbyEnemies)
        {
            centerOfMass += (Vector2)otherEnemy.transform.position;
        }
        
        centerOfMass /= nearbyEnemies.Count;
        return (centerOfMass - (Vector2)enemy.transform.position).normalized;
    }
    
    private Vector2 CalculateAlignment()
    {
        if (nearbyEnemies.Count == 0) return Vector2.zero;
        
        Vector2 averageDirection = Vector2.zero;
        
        foreach (EnemyStateManager otherEnemy in nearbyEnemies)
        {
            averageDirection += otherEnemy.Direction;
        }
        
        averageDirection /= nearbyEnemies.Count;
        return averageDirection.normalized;
    }
    
    private Vector2 SeekPlayer()
    {
        Vector2 directionToPlayer = (enemy.Player.transform.position - enemy.transform.position).normalized;
        return directionToPlayer * playerSeekForce;
    }
}