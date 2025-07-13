using UnityEngine;

public class PlayerStateManager : EntityStateManager
{
    [Header("Stats")]
    public PlayerStats templateStats;
    public PlayerStats playerStats;

    [Header("Weapon")]
    public MeleeWeapon defaultWeapon;
    public MeleeWeapon EquippedWeapon;

    public RangedWeapon equippedRangedWeapon;
    public RangedWeapon defaultRangedWeapon;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    private PlayerIdleState _idleState;
    private PlayerWalkState _walkState;
    private PlayerShiftState _shiftState;
    private PlayerSwingSwordState _swingSwordState;
    private PlayerDiedState _diedState;

    [Header("Controls")] 
    public KeyCode ProjectileInput = KeyCode.F;

    [Header("Containers")] public GameObject projectilesCtn;


    [Header("Timers")]
    public float ProjectileCooldown = 0.5f; // Cooldown for ranged attacks

    public enum PLAYER_FACE
    {
        FACE_LEFT,
        FACE_RIGHT,
        FACE_UP,
        FACE_DOWN,
    }

    public PLAYER_FACE facing;
    public PlayerIdleState IdleState { get => _idleState; }
    public PlayerWalkState WalkState { get => _walkState; }
    public PlayerShiftState ShiftState { get => _shiftState; }
    public PlayerSwingSwordState SwingSwordState { get => _swingSwordState; }
    public PlayerDiedState DiedState { get => _diedState; }



    private void Awake()
    {
        projectilesCtn = new GameObject("Projectiles");
        playerStats = Instantiate(templateStats);
        playerStats.hideFlags = HideFlags.HideAndDontSave; // Prevents it from being saved in the scene

        EquippedWeapon = Instantiate(defaultWeapon);

        EquippedWeapon.hideFlags = HideFlags.HideAndDontSave; // Prevents it from being saved in the scene


        equippedRangedWeapon = Instantiate(defaultRangedWeapon);
        equippedRangedWeapon.hideFlags = HideFlags.HideAndDontSave; // Prevents it from being saved in the scene


        _idleState = new PlayerIdleState(this);
        _walkState = new PlayerWalkState(this);
        _shiftState = new PlayerShiftState(this);
        _swingSwordState = new PlayerSwingSwordState(this);
        _diedState = new PlayerDiedState(this);

        ProjectileCooldown = playerStats.Projectile_Cooldown;
    }

    protected override void Start()
    {
        base.Start();
        Doorway.ShiftRoomEvent += OnShiftRoom;
        TransitionToState(IdleState);
    }

    private void OnShiftRoom(Doorway door)
    {
        TransitionToState(ShiftState);
    }

    public void TakeDamage(EnemyStateManager enemy)
    {
        playerStats.health -= enemy.Stats.baseAttack;
        if (playerStats.health <= 0.0f)
        {
            // Handle player death
            Debug.Log("Player has died.");
            TransitionToState(DiedState);
        }
        else
        {
            // Handle player taking damage
            Debug.Log($"Player took damage: {enemy.Stats.baseAttack}. Remaining health: {playerStats.health}");

            UIManager.Instance.CreateDamageIndicator(transform, $"{enemy.Stats.baseAttack}", Color.white);
        }
            UIManager.Instance.UpdateHealthDisplay(playerStats.health, playerStats.maxHealth);


    }

    protected override void Update()
    {
        base.Update();

        // Update the projectile cooldown timer
        if (ProjectileCooldown > 0)
        {
            ProjectileCooldown -= Time.deltaTime;
        }
        // Ensure the cooldown does not go below zero
        
    }
}
