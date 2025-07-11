using System;
using Unity.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
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
}
