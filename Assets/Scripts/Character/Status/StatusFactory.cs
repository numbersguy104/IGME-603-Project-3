using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class StatusFactory
{
    private static Dictionary<string, ConstructorInfo> statusDict = new Dictionary<string, ConstructorInfo>();

    [RuntimeInitializeOnLoadMethod]
    static void RegisterAllStatus()
    {
        StatusData[] allStatuses = Resources.LoadAll<StatusData>("Combat/Status");
        foreach (StatusData status in allStatuses)
        {
            Type type = Type.GetType($"Status_{status.name}");
            if (type == null)
            {
                Debug.LogError($"Implementation of Status_{status.name} not found");
                return;
            }
            ConstructorInfo constructor = type?.GetConstructor(new Type[] { typeof(StatusData), typeof(Character_Combat), typeof(int) });
            statusDict[status.name] = constructor;
        }
    }
    
    /// <summary>
    /// Get a Status Instance on a character from StatusData.
    /// </summary>
    /// <param name="data">StatusData of the status to be instantiated</param>
    /// <param name="owner">The character this status is going to be applied on</param>
    /// <param name="turns">The turns the status will last for</param>
    /// <returns></returns>
    public static Status GetStatus(StatusData data, Character_Combat owner, int turns)
    {
        if (statusDict.TryGetValue(data.name, out var constructor))
            return constructor.Invoke(new object[] {data, owner, turns}) as Status;

        Debug.LogError($"Unknown Status: {data.name}");
        return null;
    }
}