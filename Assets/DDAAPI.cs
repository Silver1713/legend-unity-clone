using System;
using System.Runtime.InteropServices;
using UnityEngine;
using SimpleJSON;

public class DDAAPI : MonoBehaviour
{
    #region DLL Imports
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

    //[DllImport("DDAEngine")]
    //private static extern void DDA_GetAIParameters(out AIParameters outParams);

    //[DllImport("DDAEngine")]
    //private static extern void DDA_GetPCGParameters(out PCGParameters outParams);

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
}