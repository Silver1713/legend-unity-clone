using System.Runtime.InteropServices;
using UnityEngine;

public class ExampleObject : MonoBehaviour
{
    public DDAEngineWrapper engine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        engine.StartLevel("1");

        engine.RecordDamage(500.0f);
        
    }

    // Update is called once per frame
    void Update()
    { engine.RecordDamage(5000.0f);
        Debug.Log(engine.GetAIParameters().aggressiveness);
    }

}
