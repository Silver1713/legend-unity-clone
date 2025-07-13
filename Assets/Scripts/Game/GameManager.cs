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
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static GameManager Instance
    {
        get => _instance;
    }

    public GameObject player;
    
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


    public void Melee(float dmg)
    {
        totalDamage += dmg;
        meleeAttacks += dmg;
        meleeAttackRatio = meleeAttacks / (totalDamage - totalHealing);
    }

    public void Ranged(float dmg)
    {
        totalDamage += dmg;
        rangedAttacks += dmg;
        rangedAttackRatio = rangedAttacks / (totalDamage - totalHealing);
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
}
