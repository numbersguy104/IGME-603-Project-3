using System.Collections.Generic;
using UnityEngine;

public class DataTracker : MonoBehaviour
{
    public static DataTracker Instance;

    private Dictionary<string, int> skillUses = new Dictionary<string, int>();

    public float hugoTime = 0;
    public float tenetTime = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddSkillUse(string skillName)
    {
        if (!skillUses.ContainsKey(skillName))
        {
            skillUses[skillName] = 0;
        }
        skillUses[skillName] += 1;
    }

    public void AddSkillUse(Skill skill)
    {
        AddSkillUse(skill.skillData.skillName);
    }

    public Dictionary<string, int> GetSkillUses()
    {
        return skillUses;
    }
}
