using UnityEditor;
using UnityEngine;

public class EnemyDieState : EnemyBaseState
{
    public EnemyDieState(EnemyStateManager entity) : base(entity) { }
    GameManager gameManager;

    public override void EnterState()
    {
        if (gameManager == null) gameManager = GameManager.Instance;

        gameManager.roomEnemyKilled++;
        Debug.Log("Enemy has died.");
        GameObject.Destroy(enemy.gameObject);
      
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
