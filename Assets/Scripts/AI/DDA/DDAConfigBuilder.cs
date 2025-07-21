using System;
using System.Collections.Generic;
using Defective.JSON;
using NUnit.Framework;
using UnityEngine;




public class JSONConfigObject
{

}

[System.Serializable]
public class GroupParameters
{
    public string name;
    public string description;
    public List<Parameter> parameters;

    public GroupParameters()
    {
        parameters = new List<Parameter>();
    }
    public GroupParameters(string name, string description, List<Parameter> parameters = null)
    {
        this.name = name;
        this.description = description;
        this.parameters = parameters ?? new List<Parameter>();
    }

    public void AddParameter(Parameter parameter)
    {
        parameters.Add(parameter);
    }

    public void RemoveParameter(string parameterName)
    {
        parameters.RemoveAll(p => p.name == parameterName);
    }
}

[System.Serializable]
public class Parameter
{
    public enum Type
    {
        Float,
        Double,
        Int,
        String,
        Bool,
    }
    public string name;
    public Type type;
    public string defaultValue;
    public string minValue;
    public string maxValue;
    public string description;

    public Parameter()
    {
        name = string.Empty;
        type = Type.Float;
        defaultValue = "0";
        minValue = null;
        maxValue = null;
        description = string.Empty;
    }
    public Parameter(string name, Type type, string defaultValue, string minValue = null, string maxValue = null, string description = "")
    {
        this.name = name;
        this.type = type;
        this.defaultValue = defaultValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.description = description;
    }

}

[System.Serializable]
public class FitnessMetric
{
    public enum EvaluationType
    {
        Minimize,
        Maximize,
        Distance,
    }
    [SerializeField]
    public string metricName;
    [SerializeField]
    public float weight;
    [SerializeField]
    public float idealValue;
    [SerializeField]
    public float minValue;
    [SerializeField]
    public float maxValue;
    [SerializeField]
    public EvaluationType evaluationType;

}

[System.Serializable]
public class FitnessConfig
{
    public enum FitnessType
    {
        Weighted_Sum,
        Weighted_Product,
        Uniform,
    }
    [SerializeField]
    public float baseFitness;
    [SerializeField]
    public FitnessType aggregationMethod;
    [SerializeField]
    public List<FitnessMetric> metrics;

    public FitnessConfig() 

    {
        baseFitness = 1.0f;
        aggregationMethod = FitnessType.Uniform;
        metrics = new List<FitnessMetric>();
    }

    public void AddMetric(FitnessMetric metric)
    {
        metrics.Add(metric);
    }
    public void RemoveMetric(string metricName)
    {
        metrics.RemoveAll(m => m.metricName == metricName);
    }


}




[System.Serializable]
public class DDAConfigBuilder : MonoBehaviour
{
    [SerializeField]
    public List<GroupParameters> parameterGroups;
    [SerializeField]
    public FitnessConfig fitnessConfig;

    [SerializeField] public JSONObject exportObject;

    public static DDAConfigBuilder instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);

        }
        instance = this;
    }

    public DDAConfigBuilder()
    {
        parameterGroups = new List<GroupParameters>();
        fitnessConfig = new FitnessConfig();
    }

    public T getParam<T>(string name)
    {
        JSONObject val = exportObject["current_parameters"]
            ["parameters"][name];
        string n = exportObject.ToString();
        if (val == null)
        {
            throw new System.Exception($"Parameter '{name}' not found.");
        }
        float a = 0;
        if (typeof(T) == typeof(float))
        {
            a = val.floatValue;
            return (T)(object)a;
        }
        else if (typeof(T) == typeof(int))
        {
            a = val.intValue;
            return (T)(object)a;
        }
        else if (typeof(T) == typeof(string))       
        {
            return (T)(object)val.stringValue;
        }
        else if (typeof(T) == typeof(bool))
        {
            return (T)(object)val.boolValue;
        }
        else if (typeof(T) == typeof(double))
        {
            return (T)(object)val.doubleValue;
        }
        else
        {
            throw new System.Exception($"Unsupported type: {typeof(T)}");
        }
    }

    public T getParam<T>(string group, string param)
    {
        string name = $"{group}.{param}";
        JSONObject val = exportObject["current_parameters"]
            ["parameters"][name];
        string n = exportObject.ToString();
        if (val == null)
        {
            throw new System.Exception($"Parameter '{name}' not found.");
        }
        float a = 0;
        if (typeof(T) == typeof(float))
        {
            a = val.floatValue;
            return (T)(object)a;
        }
        else if (typeof(T) == typeof(int))
        {
            a = val.intValue;
            return (T)(object)a;
        }
        else if (typeof(T) == typeof(string))
        {
            return (T)(object)val.stringValue;
        }
        else if (typeof(T) == typeof(bool))
        {
            return (T)(object)val.boolValue;
        }
        else if (typeof(T) == typeof(double))
        {
            return (T)(object)val.doubleValue;
        }
        else
        {
            throw new System.Exception($"Unsupported type: {typeof(T)}");
        }
    }

    public void AddParameterGroup(string name, string description, List<Parameter> parameters = null)
    {
        GroupParameters group = new GroupParameters(name, description, parameters);
        parameterGroups.Add(group);
    }


    public void AddParameterToGroup(string groupName, Parameter parameter)
    {
        GroupParameters group = parameterGroups.Find(g => g.name == groupName);
        if (group != null)
        {
            group.AddParameter(parameter);
        }
        else
        {
            Debug.LogWarning($"Group '{groupName}' not found.");
        }
    }

    public void RemoveParameterFromGroup(string groupName, string parameterName)
    {
        GroupParameters group = parameterGroups.Find(g => g.name == groupName);
        if (group != null)
        {
            group.RemoveParameter(parameterName);
        }
        else
        {
            Debug.LogWarning($"Group '{groupName}' not found.");
        }
    }

    public void SetFitnessConfig(FitnessConfig config)
    {
        fitnessConfig = config;
    }


    public void SetDefaultConfig()
    {
        // Set default parameter groups
        AddParameterGroup("Default Group", "This is the default group.");

        // Set default fitness config
        SetFitnessConfig(new FitnessConfig
        {
            baseFitness = 1.0f,
            aggregationMethod = FitnessConfig.FitnessType.Weighted_Sum,
            metrics = new List<FitnessMetric>()
        });

        // Add default parameters to the default group
        AddParameterToGroup("Default Group", new Parameter("Default Parameter", Parameter.Type.Float, "2.3444", "0.0", "1.0", "This is a default parameter."));
        AddParameterToGroup("Default Group", new Parameter("Another Parameter", Parameter.Type.Int, "10", "0", "100", "This is another default parameter."));
        // Add a fitness metric to the default fitness config
        fitnessConfig.AddMetric(new FitnessMetric
        {
            metricName = "Default Metric",
            weight = 1.0f,
            idealValue = 0.5f,
            minValue = 0.0f,
            maxValue = 1.0f,
            evaluationType = FitnessMetric.EvaluationType.Maximize
        });
    }


    public void AddFitnessConfig(FitnessMetric metric)
    {
        if (fitnessConfig == null)
        {
            throw new System.Exception("FitnessConfig is not initialized. Please set it before adding metrics.");
        }
        fitnessConfig.AddMetric(metric);
    }

    public void RemoveFitnessMetric(string metricName)
    {
        if (fitnessConfig == null)
        {
            throw new System.Exception("FitnessConfig is not initialized. Please set it before removing metrics.");
        }
        fitnessConfig.RemoveMetric(metricName);
    }


    public void FromJSON(string json)
    {
        exportObject = new JSONObject(json);
    }

    public JSONObject GetJSON()
    {
        return exportObject;
    }
    public string ToJSON()
    {
        string json = "{\n";


        // Serialize parameter groups
        json += "\"parameterGroups\": [\n";
        for (int i = 0; i < parameterGroups.Count; i++)
        {
            string paramJSON = string.Empty;
            GroupParameters group = parameterGroups[i];
            paramJSON += $"{{\n" +
                    $"\"name\": \"{group.name}\",\n" +
                    $"\"description\": \"{group.description}\",\n" +
                    $"\"parameters\": [\n";


            int count = 0;
            foreach (Parameter param in group.parameters)
            {
                string paramStringGrp = "{\n";

                paramStringGrp += $"\"name\": \"{param.name}\",\n" +
                                  $"\"type\": \"{param.type.ToString().ToLower()}\",\n";
                switch (param.type)
                {
                    case Parameter.Type.Int:
                        int defaultCtnInt = int.Parse(param.defaultValue);
                        int minCtnInt = param.minValue != null ? int.Parse(param.minValue) : int.MinValue;
                        int maxCtnInt = param.maxValue != null ? int.Parse(param.maxValue) : int.MaxValue;
                        paramStringGrp += $"\"defaultValue\": {param.defaultValue},\n" +
                                          $"\"minValue\": {minCtnInt},\n" +
                                          $"\"maxValue\": {maxCtnInt},\n";
                        break;
                    case Parameter.Type.Float:
                        float defaultCtnFloat = float.Parse(param.defaultValue);
                        float minCtnFloat = param.minValue != null ? float.Parse(param.minValue) : float.MinValue;
                        float maxCtnFloat = param.maxValue != null ? float.Parse(param.maxValue) : float.MaxValue;
                        paramStringGrp += $"\"defaultValue\": {defaultCtnFloat},\n" +
                                          $"\"minValue\": {minCtnFloat},\n" +
                                          $"\"maxValue\": {maxCtnFloat},\n";
                        break;
                    case Parameter.Type.String:
                        paramStringGrp += $"\"defaultValue\": \"{param.defaultValue}\",\n";
                        if (param.minValue != null)
                            paramStringGrp += $"\"minValue\": \"{param.minValue}\",\n";
                        if (param.maxValue != null)
                            paramStringGrp += $"\"maxValue\": \"{param.maxValue}\",\n";
                        break;
                    case Parameter.Type.Bool:
                        bool defaultCtnBool = bool.Parse(param.defaultValue);
                        paramStringGrp += $"\"defaultValue\": \"{defaultCtnBool}\",\n";
                        if (param.minValue != null)
                            paramStringGrp += $"\"minValue\": \"{param.minValue}\",\n";
                        if (param.maxValue != null)
                            paramStringGrp += $"\"maxValue\": \"{param.maxValue}\",\n";
                        break;

                    case Parameter.Type.Double:
                        double defaultCtnDouble = double.Parse(param.defaultValue);
                        double minCtnDouble = param.minValue != null ? double.Parse(param.minValue) : double.MinValue;
                        double maxCtnDouble = param.maxValue != null ? double.Parse(param.maxValue) : double.MaxValue;
                        paramStringGrp += $"\"defaultValue\": {defaultCtnDouble},\n" +
                                          $"\"minValue\": {minCtnDouble},\n" +
                                          $"\"maxValue\": {maxCtnDouble},\n";
                        break;
                    default:
                        throw new System.Exception($"Unsupported parameter type: {param.type}");

                }

                paramStringGrp += $"\"description\": \"{param.description}\"\n";
                paramStringGrp += "}";

                paramJSON += paramStringGrp;
                if (count < group.parameters.Count - 1)
                {
                    paramJSON += ",\n";
                }
                else
                {
                    paramJSON += "\n";
                }
                count++;
            }

            paramJSON += "]\n" +
                    "}";
            if (i < parameterGroups.Count - 1)
            {
                paramJSON += ",\n";
            }
            else
            {
                paramJSON += "\n";
            }
            json += paramJSON;
        }

        json += "],\n";

        string FitnessConfigJSON = string.Empty;
        // Serialize fitness config

        FitnessConfigJSON += "\"fitnessConfig\": {\n" +
            $"\"baseFitness\": {fitnessConfig.baseFitness},\n" +
            $"\"aggregationMethod\": \"{fitnessConfig.aggregationMethod.ToString().ToLower()}\",\n" +
            "\"metrics\": [\n";
        for (int i = 0; i < fitnessConfig.metrics.Count; i++)
        {
            string metricJSON = string.Empty;
            FitnessMetric metric = fitnessConfig.metrics[i];
            metricJSON += "{\n" +
                          $"\"metricName\": \"{metric.metricName}\",\n" +
                          $"\"weight\": {metric.weight},\n" +
                          $"\"idealValue\": {metric.idealValue},\n" +
                          $"\"minValue\": {metric.minValue},\n" +
                          $"\"maxValue\": {metric.maxValue},\n" +
                          $"\"evaluationType\": \"{metric.evaluationType.ToString().ToLower()}\"\n" +
                          "}";
            if (i < fitnessConfig.metrics.Count - 1)
            {
                metricJSON += ",\n";
            }
            else
            {
                metricJSON += "\n";
            }
            FitnessConfigJSON += metricJSON;
        }

        FitnessConfigJSON += "]\n" +
            "}";

        json += FitnessConfigJSON;

        json += "\n}";
        return json;
    }

}
