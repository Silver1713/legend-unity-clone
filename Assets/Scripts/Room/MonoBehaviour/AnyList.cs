using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnyList
{
    public enum TYPE
    {
        STRING,
        INT,
        FLOAT,
        BOOL,
        VECTOR2,
        VECTOR3,
        VECTOR4,
        COLOR,
        QUATERNION,
        GAMEOBJECT,
        // Add more types as needed
    }
    [SerializeField]
    public List<SKeyValuePair<string, SKeyValuePair<TYPE, object>>> list =
        new List<SKeyValuePair<string, SKeyValuePair<TYPE, object>>>();

    public void Add<T>(string name, TYPE t, T value)
    {
        list.Add(new SKeyValuePair<string, SKeyValuePair<TYPE, object>>(
            name,
            new SKeyValuePair<TYPE, object>(t, value)
        ));
    }

    public void Get<T>(string name, out T value)
    {
        value = default;
        foreach (var item in list)
        {
            if (item.Key == name && item.Value.Value.GetType() == typeof(T))
            {
                value = (T)item.Value.Value;
                return;
            }
        }
    }
    public void RemoveAt(int index)
    {
        if (index >= 0 && index < list.Count)
        {
            list.RemoveAt(index);
        }
    }
}
