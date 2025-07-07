  using System;
  using System.Runtime.InteropServices;
  using UnityEngine;

  public static class DDAEngineImporter
  {
      // =============================================================================
      // DLL IMPORTS - Core Functions
      // =============================================================================

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_Initialize();

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_Shutdown();

      // =============================================================================
      // DLL IMPORTS - Metric Collection
      // =============================================================================

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_CollectMetric([MarshalAs(UnmanagedType.LPStr)] string metricName, float value);

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_CollectMetricInt([MarshalAs(UnmanagedType.LPStr)] string metricName, int value);

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_SubmitLevelMetrics([MarshalAs(UnmanagedType.LPStr)] string jsonMetricMatrix);

      // =============================================================================
      // DLL IMPORTS - Parameter Management
      // =============================================================================

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_EvolveParameters();

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_GetAIParameters(out AIParametersC outParams);

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_GetPCGParameters(out PCGParametersC outParams);

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern float DDA_GetDifficultyMultiplier();

      // =============================================================================
      // DLL IMPORTS - Mode and Settings
      // =============================================================================

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_SetMode(DDAMode mode);

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern DDAMode DDA_GetMode();

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_SetEvolutionEnabled(int enabled);

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern int DDA_IsEvolutionEnabled();

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_UpdateAdaptive(float deltaTime);

      // =============================================================================
      // DLL IMPORTS - Level Generation and Skill
      // =============================================================================

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr DDA_GetLevelGenerationHints();

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr DDA_ExportParametersJson();

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_ImportParametersJson([MarshalAs(UnmanagedType.LPStr)] string json);

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern float DDA_GetPlayerSkillLevel();

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_SetIdealMetrics(float completionTime, float deathRate, float accuracy);

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_Reset();

      [DllImport("DDAEngine", CallingConvention = CallingConvention.Cdecl)]
      public static extern void DDA_FreeString(IntPtr str);

      // =============================================================================
      // STRUCTS - Must match C++ structs exactly
      // =============================================================================

      [StructLayout(LayoutKind.Sequential)]
      public struct AIParametersC
      {
          public float aggressiveness;
          public float reactionTime;
          public float accuracy;
          public float movementSpeed;
          public float detectionRange;
          public float attackFrequency;

          public override string ToString()
          {
              return $"AI[Aggr:{aggressiveness:F2}, React:{reactionTime:F2}, Acc:{accuracy:F2}, " +
                     $"Speed:{movementSpeed:F2}, Range:{detectionRange:F1}, Freq:{attackFrequency:F2}]";
          }
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct PCGParametersC
      {
          public float enemyDensity;
          public float powerUpFrequency;
          public float obstacleComplexity;
          public float pathBranching;
          public float hazardIntensity;
          public int minEnemiesPerRoom;
          public int maxEnemiesPerRoom;

          public override string ToString()
          {
              return $"PCG[Density:{enemyDensity:F2}, PowerUps:{powerUpFrequency:F2}, " +
                     $"Obstacles:{obstacleComplexity:F2}, Enemies:{minEnemiesPerRoom}-{maxEnemiesPerRoom}]";
          }
      }

      // =============================================================================
      // ENUMS - Must match C++ enums exactly
      // =============================================================================

      public enum DDAMode
      {
          DDA_MODE_ADAPTIVE = 0,
          DDA_MODE_FIXED = 1,
          DDA_MODE_LEARNING = 2
      }

      // =============================================================================
      // HELPER METHODS - For easier string handling
      // =============================================================================

      public static string GetLevelGenerationHintsString()
      {
          IntPtr ptr = DDA_GetLevelGenerationHints();
          if (ptr == IntPtr.Zero) return "";

          string result = Marshal.PtrToStringAnsi(ptr);
          DDA_FreeString(ptr);
          return result ?? "";
      }

      public static string ExportParametersJsonString()
      {
          IntPtr ptr = DDA_ExportParametersJson();
          if (ptr == IntPtr.Zero) return "";

          string result = Marshal.PtrToStringAnsi(ptr);
          DDA_FreeString(ptr);
          return result ?? "";
      }

      // =============================================================================
      // CONVENIENCE METHODS - Simplified API
      // =============================================================================

      public static void SubmitPlayerMetrics(int deaths, float completionTime, float accuracy,
                                           int enemiesKilled, float damageTaken, int powerupsCollected)
      {
          string json = $@"{{
              ""player_deaths"": {deaths},
              ""completion_time"": {completionTime},
              ""accuracy"": {accuracy:F3},
              ""enemies_killed"": {enemiesKilled},
              ""damage_taken"": {damageTaken:F1},
              ""powerups_collected"": {powerupsCollected},
              ""distance_traveled"": 1000.0
          }}";

          DDA_SubmitLevelMetrics(json);
      }

      public static bool IsEvolutionEnabled()
      {
          return DDA_IsEvolutionEnabled() != 0;
      }

      public static void SetEvolutionEnabled(bool enabled)
      {
          DDA_SetEvolutionEnabled(enabled ? 1 : 0);
      }
  }

  Usage Example:

  using UnityEngine;

  public class DDAController : MonoBehaviour
  {
      void Start()
      {
          // Initialize the DDA system
          DDAEngineImporter.DDA_Initialize();

          // Set ideal metrics (5 min completion, 20% death rate, 70% accuracy)
          DDAEngineImporter.DDA_SetIdealMetrics(300f, 0.2f, 0.7f);

          // Set to adaptive mode
          DDAEngineImporter.DDA_SetMode(DDAEngineImporter.DDAMode.DDA_MODE_ADAPTIVE);

          Debug.Log("DDA Engine initialized!");
      }

      void Update()
      {
          // Update adaptive parameters each frame
          DDAEngineImporter.DDA_UpdateAdaptive(Time.deltaTime);
      }

      public void OnLevelComplete(int deaths, float time, float accuracy, int enemiesKilled)
      {
          // Submit metrics using convenience method
          DDAEngineImporter.SubmitPlayerMetrics(deaths, time, accuracy, enemiesKilled, 50f, 3);

          // Get updated parameters
          DDAEngineImporter.DDA_GetAIParameters(out var aiParams);
          DDAEngineImporter.DDA_GetPCGParameters(out var pcgParams);

          Debug.Log($"New AI params: {aiParams}");
          Debug.Log($"New PCG params: {pcgParams}");
          Debug.Log($"Player skill: {DDAEngineImporter.DDA_GetPlayerSkillLevel():F2}");

          // Trigger evolution every few levels
          DDAEngineImporter.DDA_EvolveParameters();
      }

      void OnApplicationQuit()
      {
          DDAEngineImporter.DDA_Shutdown();
      }
  }