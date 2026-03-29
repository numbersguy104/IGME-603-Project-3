using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSettingUI : MonoBehaviour
{
    private List<SkillData> skillsNotEquipped;
    private List<SkillData> skillsEquipped;

    [SerializeField] private GameObject skillSlotsEquipped;
    [SerializeField] private GameObject skillSlotsNotEquipped;
    
    private void Awake()
    {
    }

    public void GetSkills(bool isHugo)
    {
        skillsNotEquipped = new  List<SkillData>();
        List<SkillData> skillLearned = (isHugo ? CharacterStatsManager.Instance.hugo : CharacterStatsManager.Instance.tenet).skillLearned;
        skillsEquipped = (isHugo ? CharacterStatsManager.Instance.hugo : CharacterStatsManager.Instance.tenet).skillLearned;
        foreach (var skill in skillLearned)
        {
            if(skillsEquipped.Contains(skill))
                continue;
            skillsNotEquipped.Add(skill);
        }
    }
    public void UpdateSkill(bool isHugo)
    {
        GetSkills(isHugo);
        int skillSlots = (isHugo ? CharacterStatsManager.Instance.hugo : CharacterStatsManager.Instance.tenet)
            .maxSkillSlots;
        for (int i = 0 ; i < skillSlots; i++)
            skillSlotsEquipped.transform.GetChild(i).GetComponent<Image>().enabled = true;
        for (int i = skillSlots; i < 4; i++)
            skillSlotsEquipped.transform.GetChild(i).GetComponent<Image>().enabled = false;

        SkillSlotButton[] buttons = skillSlotsEquipped.GetComponentsInChildren<SkillSlotButton>();
        for (int i = 0; i < skillsEquipped.Count; i++)
        {
            buttons[i].skillData = skillsEquipped[i];
            buttons[i].gameObject.SetActive(true);
            // buttons[i].
        }
        
        for (int i = skillsEquipped.Count; i < 4; i++)
        {
            buttons[i].skillData = null;
            buttons[i].gameObject.SetActive(false);
        }
    }

    public void SwitchSkill(SkillData skillToSwitch)
    {
        
    }
}
