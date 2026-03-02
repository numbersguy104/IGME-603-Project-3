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
            Type type = Type.GetType($"Status_{status.statusName}");
            if (type == null)
            {
                Debug.LogError($"Implementation of Status_{status.statusName} not found");
                return;
            }
            ConstructorInfo constructor = type?.GetConstructor(new Type[] { typeof(StatusData), typeof(Character_Combat), typeof(int) });
            statusDict[status.statusName] = constructor;
        }
    }
    public static Status GetStatus(StatusData data, Character_Combat owner, int turns)
    {
        if (statusDict.TryGetValue(data.statusName, out var constructor))
            return constructor.Invoke(new object[] {data, owner, turns}) as Status;

        Debug.LogError($"Unknown Status: {data.statusName}");
        return null;
    }
}