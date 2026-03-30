using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSettingUI : MonoBehaviour
{
    private List<SkillData> skillNotEquipped;
    private List<SkillData> skillEquipped;
    private PlayerCharacter currentCharacter;

    private GameObject skillSlotsEquipped;
    private GameObject skillSlotsNotEquipped;
    [SerializeField] private TMP_Text description;
    
    private void Awake()
    {
    }

    public void GetSkills()
    {
        skillNotEquipped = new List<SkillData>();
        skillEquipped = new List<SkillData>();
        List<SkillData> skillLearned = currentCharacter.skillLearned;
        foreach (var skill in skillLearned)
        {
            if(currentCharacter.skillEquipped.Contains(skill))
                skillEquipped.Add(skill);
            else
                skillNotEquipped.Add(skill);
        }
    }
    public void UpdateSkill(bool isHugo)
    {
        currentCharacter = isHugo ? CharacterStatsManager.Instance.hugo : CharacterStatsManager.Instance.tenet;
        GetSkills();
        UpdatePanel();
    }

    public void UpdatePanel()
    {
        int skillSlots = currentCharacter.maxSkillSlots;
        for (int i = 0 ; i < skillSlots; i++)
            skillSlotsEquipped.transform.GetChild(i).GetComponent<Image>().enabled = true;
        for (int i = skillSlots; i < 4; i++)
            skillSlotsEquipped.transform.GetChild(i).GetComponent<Image>().enabled = false;

        SkillSlotButton[] buttons = skillSlotsEquipped.GetComponentsInChildren<SkillSlotButton>();
        for (int i = 0; i < skillEquipped.Count; i++)
        {
            buttons[i].skillData = skillEquipped[i];
            buttons[i].gameObject.SetActive(true);
            // buttons[i].
        }
        
        for (int i = skillEquipped.Count; i < 4; i++)
        {
            buttons[i].skillData = null;
            buttons[i].gameObject.SetActive(false);
        }
    }

    public void RemoveSkillFrom(SkillData skill, bool isEquippedBefore)
    {
        if (isEquippedBefore)
        {
            skillEquipped.Remove(skill);
        }
        else
        {
            skillNotEquipped.Remove(skill);
        }
        UpdatePanel();
    }

    public void AddSkill(SkillData skill, bool isEquipped)
    {
        if (isEquipped)
        {
            skillEquipped.Add(skill);
        }
        else
        {
            skillNotEquipped.Add(skill);
        }
        UpdatePanel();
    }

    public void UpdateCharacterSkillSet()
    {
        currentCharacter.skillEquipped = this.skillEquipped;
    }

    public void UpdateDescription(SkillData skill)
    {
        description.SetText(skill.skillDescription);
    }
}
