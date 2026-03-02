using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class StatusFactory
{
    private static Dictionary<string, ConstructorInfo> statusDict = new Dictionary<string, ConstructorInfo>();

    public static void RegisterStatus(string name)
    {
        Type type = Type.GetType($"Status_{name}");
        if (type == null)
        {
            Debug.LogError($"Implementation of Status_{name} not found");
            return;
        }
        ConstructorInfo constructor = type?.GetConstructor(new Type[] { typeof(Character_Combat), typeof(int) });
        statusDict[name] = constructor;
    }
    public static Status GetStatus(string name, Character_Combat owner, int turns)
    {
        if (statusDict.TryGetValue(name, out var constructor))
            return constructor.Invoke(new object[] {owner, turns}) as Status;

        Debug.LogError($"Unknown Status: {name}");
        return null;
    }
}