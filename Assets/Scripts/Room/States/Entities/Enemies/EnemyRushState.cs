using UnityEngine;

public class EnemyRushState : EnemyBaseState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerStateManager Player { get; set; }
    public EnemyRushState(EnemyStateManager entity) : base(entity)
    {

    }


    public override void EnterState()
    {
        Debug.Log("Entering Rush State");
       // enemy.TransitionToState(enemy.IdleState);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, enemy.Player.transform.position, enemy.Stats.speed * Time.deltaTime);
        Debug.Log(enemy.name + " Running..");
        
    }
}
