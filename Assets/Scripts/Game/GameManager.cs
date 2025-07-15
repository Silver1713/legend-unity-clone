using System;
using Unity.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public DDAEngineWrapper ddaEngineWrapper;

    public float totalDamage;
    public float totalHealing;

    public float rangedAttackRatio;
    public float meleeAttackRatio;

    public float meleeAttacks;
    public float rangedAttacks;

    [Header("Room Statistics")] 
    public int roomEnemies;

    public int roomEnemyKilled;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static GameManager Instance
    {
        get => _instance;
    }

    public GameObject player;
    
    public AnyList anyList;
    void Awake()
    {
        // Check if an instance already exists
        if (_instance == null)
        {
            _instance = this;

        }
        else
        {
            Destroy(gameObject);
        }


        

    }
    void Start()
    {
        Debug.Log("Game Manager Started");

        anyList.Add("player.TotalDamage", AnyList.TYPE.FLOAT, totalDamage);
        anyList.Add("player.TotalHealing", AnyList.TYPE.FLOAT, totalHealing);
        anyList.Add("player.RangedAttackRatio", AnyList.TYPE.FLOAT, rangedAttackRatio);
        anyList.Add("player.MeleeAttackRatio", AnyList.TYPE.FLOAT, meleeAttackRatio);
        anyList.Add("player.MeleeAttacks", AnyList.TYPE.FLOAT, meleeAttacks);
        anyList.Add("player.RangedAttacks", AnyList.TYPE.FLOAT, rangedAttacks);
        anyList.Add("player.GameObject", AnyList.TYPE.GAMEOBJECT, player);


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetPlayer(GameObject obj)
    {
        player = obj;
    }

    public GameObject GetPlayer()
    {
        return player;
    }


    public void EnemyDied()
    {
        roomEnemyKilled++;
    }

    public void ResetEnemy()
    {
                roomEnemies = 0;
        roomEnemyKilled = 0;
    }

    public void Melee(float dmg)
    {
        totalDamage += dmg;
        meleeAttacks += dmg;
        meleeAttackRatio = meleeAttacks / (totalDamage - totalHealing);
        rangedAttackRatio = rangedAttacks / (totalDamage - totalHealing);
    }

    public void Ranged(float dmg)
    {
        totalDamage += dmg;
        rangedAttacks += dmg;
        rangedAttackRatio = rangedAttacks / (totalDamage - totalHealing);
        meleeAttackRatio = meleeAttacks / (totalDamage - totalHealing);
    }

    public void PrintStats()
    {
                Debug.Log($"Total Damage: {totalDamage}");
        Debug.Log($"Total Healing: {totalHealing}");
        Debug.Log($"Ranged Attack Ratio: {rangedAttackRatio}");
        Debug.Log($"Melee Attack Ratio: {meleeAttackRatio}");

        ddaEngineWrapper.RecordDamage(totalDamage);
        Debug.Log("Evolved DDA: " + ddaEngineWrapper.GetPlayerSkillLevel());
    }


    public void SaveStats()
    {
                anyList.Add("player.TotalDamage", AnyList.TYPE.FLOAT, totalDamage);
        anyList.Add("player.TotalHealing", AnyList.TYPE.FLOAT, totalHealing);
        anyList.Add("player.RangedAttackRatio", AnyList.TYPE.FLOAT, rangedAttackRatio);
        anyList.Add("player.MeleeAttackRatio", AnyList.TYPE.FLOAT, meleeAttackRatio);
        anyList.Add("player.MeleeAttacks", AnyList.TYPE.FLOAT, meleeAttacks);
        anyList.Add("player.RangedAttacks", AnyList.TYPE.FLOAT, rangedAttacks);
        anyList.Add("player.GameObject", AnyList.TYPE.GAMEOBJECT, player);
    }
}
