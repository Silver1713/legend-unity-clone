
using System;
using System.IO;
using System.Runtime.InteropServices;
using Defective.JSON;
using UnityEngine;
using Random = UnityEngine.Random;

// Unity C# wrapper for DDAEngine


#region Structs

namespace LOCAL {
[StructLayout(LayoutKind.Sequential)]
public struct AIParameters
{
    public float Aggression;
    public float ReactionTime;
    public float Accuracy;
    public float MovementSpeed;
    public float DetectionRadius;
    public float AttackFrequency;
}

[StructLayout(LayoutKind.Sequential)]
public struct PCGParameters
{
    public float EnemyDensity;
    public float LootFrequency;
    public float ObstacleComplexity;
    public float PathBranchingIntensity;
    public float hazardIntensity;
    public float minEnemyRoom;
    public float maxEnemyRoom;
}
}


#endregion


public class DDAAPI : MonoBehaviour
{
    public bool LevelEnding = false;

    #region DLL Imports

    [DllImport("DDAEngine")]
    private static extern int DDA_Load();

    [DllImport("DDAEngine")]
    private static extern int DDA_LoadConfig([MarshalAs(UnmanagedType.LPStr)] string json);
    [DllImport("DDAEngine")]
    private static extern void DDA_SetAnyIdealMetric([MarshalAs(UnmanagedType.LPStr)] string metric, float value);
    [DllImport("DDAEngine")]
    private static extern int DDA_INIT();
    [DllImport("DDAEngine")]
    private static extern void DDA_Initialize();

    [DllImport("DDAEngine")]
    private static extern void DDA_Shutdown();

    [DllImport("DDAEngine")]
    private static extern void DDA_CollectMetric([MarshalAs(UnmanagedType.LPStr)] string metricName, float value);

    [DllImport("DDAEngine")]
    private static extern void DDA_CollectMetricInt([MarshalAs(UnmanagedType.LPStr)] string metricName, int value);

    [DllImport("DDAEngine")]
    private static extern void DDA_SubmitLevelMetrics([MarshalAs(UnmanagedType.LPStr)] string jsonMetricMatrix);

    [DllImport("DDAEngine")]
    private static extern void DDA_EvolveParameters();

    [DllImport("DDAEngine")]
    private static extern void DDA_GetAIParameters(out LOCAL.AIParameters outParams);

    [DllImport("DDAEngine")]
    private static extern void DDA_GetPCGParameters(out LOCAL.PCGParameters outParams);

    [DllImport("DDAEngine")]
    private static extern float DDA_GetDifficultyMultiplier();

    [DllImport("DDAEngine")]
    private static extern void DDA_SetMode(int mode);

    [DllImport("DDAEngine")]
    private static extern int DDA_GetMode();

    [DllImport("DDAEngine")]
    private static extern void DDA_SetEvolutionEnabled(int enabled);

    [DllImport("DDAEngine")]
    private static extern int DDA_IsEvolutionEnabled();

    [DllImport("DDAEngine")]
    private static extern void DDA_UpdateAdaptive(float deltaTime);

    [DllImport("DDAEngine")]
    private static extern IntPtr DDA_GetLevelGenerationHints();

    [DllImport("DDAEngine")]
    private static extern IntPtr DDA_ExportParametersJson();

    [DllImport("DDAEngine")]
    private static extern void DDA_ImportParametersJson([MarshalAs(UnmanagedType.LPStr)]string json);

    [DllImport("DDAEngine")]
    private static extern float DDA_GetPlayerSkillLevel();

    [DllImport("DDAEngine")]
    private static extern void DDA_SetIdealMetrics(float completionTime, float deathRate, float accuracy);

    [DllImport("DDAEngine")]
    private static extern void DDA_Reset();

    [DllImport("DDAEngine")]
    private static extern void DDA_FreeString(IntPtr str);

    [DllImport("DDAEngine")]
    private static extern void DDA_AdvanceEngine();

    #endregion

    public static DDAAPI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        //Read from Assets/Configuration/...
        //OnDDAStart();
        



    }

    public void LoadConfig(DDAConfigBuilder config)
    {
        string data = config.ToJSON();
        int result = DDA_LoadConfig(data);
        if (result != 0)
        {
            Debug.LogError("Failed to load DDA configuration with error code: " + result);
        }
        else
        {
            Debug.Log("DDA configuration loaded successfully.");
        }
    }

    public void InitalizeNative()
    {
        DDA_Initialize();
        DDA_SetMode(2);

    }

    public void CollectMetric(string metricName, float value)
    {
        if (!LevelEnding)
        DDA_CollectMetric(metricName, value);
    }
    public void CollectMetricInt(string metricName, int value)
    {
        DDA_CollectMetricInt(metricName, value);
    }

    public void Advance()
    {
        DDA_AdvanceEngine();
        
    }

   

    public void OnDDAStart()
    {
        string path = "";
        string jsonData =
            DDAConfigBuilder.instance.ToJSON();
        Debug.Log(jsonData);
        int initResult = DDA_LoadConfig(jsonData);
        if (initResult != 0)
        {
            Debug.LogError("DDAEngine initialization failed with error code: " + initResult);
            return;
        }

        DDA_Initialize();

        for (int i =0; i < 3; i++)
        {
           OnDDALevelStart();

           for (int x=0; x < 100; x++)
           {
               float r = Random.Range(0f, 2f);
               DDA_CollectMetric("TestMetric", r);
           }
           DDA_AdvanceEngine();
           GetJSON();
        }


    }

    public void OnDDALevelStart()
    {
        DDA_CollectMetric("TestMetric", Random.Range(0f, 1f));
    }

    public void OnDDAUpdate(float deltaTime)
    {
        DDA_UpdateAdaptive(deltaTime);
    }

    public void OnDDALevelEnd()
    {
        DDA_EvolveParameters();

    }



    public void GetJSON()
    {
       string state = Marshal.PtrToStringAnsi(DDA_ExportParametersJson());
        Debug.Log("Exported JSON: " + state);
        DDA_FreeString(Marshal.StringToHGlobalAnsi(state));
        DDAConfigBuilder.instance.FromJSON(state);
       
    }

}