using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanelUI : MonoBehaviour
{
    public List<Skill> skillList;
    private void OnEnable()
    {
        // TODO: Temp Test Only
        Transform skillButtons = transform.Find("Skills");
        for (int i = 0; i < skillButtons.childCount; i++)
            skillButtons.GetChild(i).GetComponent<TempSkillButton>().skill = skillList[0];
    }

    public void UpdateSkillList(List<Skill> skillList)
    {
        this.skillList = skillList;
        // TODO:
        // Transform skillButtons = transform.Find("Skills");
        // for (int i = 0; i < skillButtons.childCount; i++)
        //     Destroy(skillButtons.GetChild(i).gameObject);
        // foreach (Skill skill in skillList)
        //     CreateButton(skill);
    }

    public void CreateButton(Skill skill)
    {
        // TODO
    }
    
}
