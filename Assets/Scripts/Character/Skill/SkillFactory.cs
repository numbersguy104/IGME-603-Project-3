using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class SkillFactory
{
    private static Dictionary<string, ConstructorInfo> skillDict = new Dictionary<string, ConstructorInfo>();

    [RuntimeInitializeOnLoadMethod]
    static void RegisterAllSkills()
    {
        SkillData[] allSkills = Resources.LoadAll<SkillData>("Combat/Skills");
        foreach (SkillData skill in allSkills)
        {
            Type type = Type.GetType($"Skill_{skill.name}");
            ConstructorInfo constructor = null;
            
            if (type == null)
            {
                switch (skill.skillType)
                {
                    case SkillType.Attack:
                        constructor = typeof(AttackSkill).GetConstructor(new Type[] { typeof(SkillData)});
                        break;
                    case SkillType.Buff:
                        constructor = typeof(BuffSkill).GetConstructor(new Type[] { typeof(SkillData)});
                        break;
                }
            }
            else
            {
                Debug.Log($"Register: {skill.name}");
                constructor = type?.GetConstructor(new Type[] { typeof(SkillData)});
            }
            skillDict[skill.name] = constructor;
        }
    }
    
    /// <summary>
    /// Get a Skill Instance
    /// </summary>
    /// <param name="data">Data of the skill to be instantiated</param>
    public static Skill GetSkill(SkillData data)
    {
        Debug.Log("GetSkill: " + data.skillName);
        if (skillDict.TryGetValue(data.name, out var constructor))
            return constructor.Invoke(new object[] {data}) as Skill;

        Debug.LogError($"Unknown Skill: {data.name}");
        return null;
    }
}