using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class EnemyStateManager : EntityStateManager
{
   // public List<EnemyBaseState> states;
    private PlayerStateManager _player;
    private EnemyIdleState _idleState;
    private EnemyWalkState _walkState;
    private EnemyKnockedState _knockedState;
    private EnemyDieState _dieState;
    private EnemyAttackState _attackState;
    private EnemyRushState _rushState;

    private Vector2 _hitDir;



    public EnemyWalkState WalkState { get => _walkState; }
    public EnemyIdleState IdleState { get => _idleState; }
    public EnemyKnockedState KnockState { get => _knockedState; }
    public EnemyDieState DieState { get => _dieState; }
    public EnemyAttackState AttackState { get => _attackState; }

    public EnemyRushState RushState { get => _rushState; }

    [Header("Movement")]

    public Vector2 HitDirection { get => _hitDir; }
    public float ThrustForce = 13.0f;

    [Header("Stats")] 
    public EnemyStats templateStats;

    public EnemyStats Stats;

    [Header("Settings")] public float COOLDOWN;

    public PlayerStateManager Player
    {
        get => _player;
        set => _player = value;
    }


    private void Awake()
    {
        // states = new List<EnemyBaseState>();

        Stats = Instantiate(templateStats);
        Stats.hideFlags = HideFlags.HideAndDontSave; // Prevents it from being saved in the scene
        _idleState = new EnemyIdleState(this);
        _walkState = new EnemyWalkState(this);
        _knockedState = new EnemyKnockedState(this);
        _dieState = new EnemyDieState(this);
        _attackState = new EnemyAttackState(this);
        _rushState = new EnemyRushState(this);
        _direction = Vector2.down;

        COOLDOWN = Stats.attackCooldown;
    }
    protected override void Start()
    {
        base.Start();
        TransitionToState(_walkState);
    }

    // Called from Player when hit an NPC with the sword
    // Stores the player direction to apply thrust force (inversed) to NPC
    public void Hit(Vector2 direction, float damage)
    {
        _hitDir = direction;
        KnockState.raw_damage = damage;
        TransitionToState(KnockState);
    }

    protected override void Update()
    {
        base.Update();
        if (COOLDOWN <= 0)
        {
            COOLDOWN = Stats.attackCooldown;
        }
        COOLDOWN -= Time.deltaTime;

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.gameObject.CompareTag("Player"))
        {
            _player = collision.gameObject.GetComponent<PlayerStateManager>();
            if (_player != null && _currentState != KnockState && _currentState != DieState && COOLDOWN <= 0)
            {
                
                TransitionToState(AttackState);
                //_player.TakeDamage(this); // Notify player to take damage
            }
        }
    }

}
