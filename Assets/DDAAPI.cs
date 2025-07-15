
using System;
using System.IO;
using System.Runtime.InteropServices;

using UnityEngine;
using SimpleJSON;

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

    #region DLL Imports

    [DllImport("DDAEngine")]
    private static extern int DDA_Load();

    [DllImport("DDAEngine")]
    private static extern int DDA_LoadConfig(string json);
    [DllImport("DDAEngine")]
    private static extern int DDA_INIT();
    [DllImport("DDAEngine")]
    private static extern void DDA_Initialize();

    [DllImport("DDAEngine")]
    private static extern void DDA_Shutdown();

    [DllImport("DDAEngine")]
    private static extern void DDA_CollectMetric(string metricName, float value);

    [DllImport("DDAEngine")]
    private static extern void DDA_CollectMetricInt(string metricName, int value);

    [DllImport("DDAEngine")]
    private static extern void DDA_SubmitLevelMetrics(string jsonMetricMatrix);

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
    private static extern void DDA_ImportParametersJson(string json);

    [DllImport("DDAEngine")]
    private static extern float DDA_GetPlayerSkillLevel();

    [DllImport("DDAEngine")]
    private static extern void DDA_SetIdealMetrics(float completionTime, float deathRate, float accuracy);

    [DllImport("DDAEngine")]
    private static extern void DDA_Reset();

    [DllImport("DDAEngine")]
    private static extern void DDA_FreeString(IntPtr str);

    #endregion


    private void Start()
    {
        //Read from Assets/Configuration/...
        string path = "F:\\Unity Projects\\legend-unity\\Assets\\Configuration\\shooter_game_config.json";
        string jsonData =
            File.ReadAllText(path);
        Debug.Log(jsonData);
        int initResult = DDA_LoadConfig(jsonData);
        if (initResult != 0)
        {
            Debug.LogError("DDAEngine initialization failed with error code: " + initResult);
            return;
        }


    }
}