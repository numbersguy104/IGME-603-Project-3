using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelUI : MonoBehaviour
{
    public List<SkillButton> buttonList;
    private void Awake()
    {
        Transform skillButtons = transform.Find("Skills");
        for (int i = 0; i < 3; i++)
        {
            SkillButton button = skillButtons.GetChild(i).GetComponent<SkillButton>();
            buttonList.Add(button);
        }
    }

    private void OnEnable()
    {
        List<Skill> skillList = PlayerController_Combat.Instance.currentCharacter.skills;
        Transform skillButtons = transform.Find("Skills");
        for (int i = 0; i < skillList.Count; i++)
        {
            SkillButton button = skillButtons.GetChild(i).GetComponent<SkillButton>();
            button.skill = skillList[i];
            button.gameObject.SetActive(true);
            button.UpdateSkillInfo();
        }
        for (int i = skillList.Count; i < 3; i++)
        {
            buttonList[i].gameObject.SetActive(false);
        }

        UpdateSKillButtons();
        
        CombatUI.Instance.OnCombatInfoUpdated.AddListener(UpdateSKillButtons);
    }

    private void OnDisable()
    {
        CombatUI.Instance.OnCombatInfoUpdated.RemoveListener(UpdateSKillButtons);
    }

    public void UpdateSKillButtons()
    {
        if (!gameObject.activeSelf)
            return;
        foreach (var skillButton in buttonList)
        {
            if(skillButton.gameObject.activeSelf == false)
                continue;
            Skill skill = skillButton.skill;
            skillButton.interactable = skill.isReady && PlayerController_Combat.Instance.currentCharacter.CurrentSkillPoint >= skill.skillData.cost;
            switch (skill)
            {
                case AttackSkill:
                    skillButton.interactable &= PlayerController_Combat.Instance.currentCharacter.attacksAvailable > 0;
                    break;
            }
        }
    }

    // public void CreateButton(Skill skill)
    // {
    //     // TODO
    // }
    
}
