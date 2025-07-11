using UnityEngine;

public class PlayerStateManager : EntityStateManager
{
    [Header("Stats")]
    public PlayerStats templateStats;
    public PlayerStats playerStats;

    [Header("Weapon")]
    public Weapon defaultWeapon;
    public Weapon EquippedWeapon;

    private PlayerIdleState _idleState;
    private PlayerWalkState _walkState;
    private PlayerShiftState _shiftState;
    private PlayerSwingSwordState _swingSwordState;

    public PlayerIdleState IdleState { get => _idleState; }
    public PlayerWalkState WalkState { get => _walkState; }
    public PlayerShiftState ShiftState { get => _shiftState; }
    public PlayerSwingSwordState SwingSwordState { get => _swingSwordState; }

    private void Awake()
    {
        playerStats = Instantiate(templateStats);
        playerStats.hideFlags = HideFlags.HideAndDontSave; // Prevents it from being saved in the scene

        EquippedWeapon = Instantiate(defaultWeapon);

        EquippedWeapon.hideFlags = HideFlags.HideAndDontSave; // Prevents it from being saved in the scene


        _idleState = new PlayerIdleState(this);
        _walkState = new PlayerWalkState(this);
        _shiftState = new PlayerShiftState(this);
        _swingSwordState = new PlayerSwingSwordState(this);
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
        Debug.Log("player hit by NPC");
        
        
    }
}
