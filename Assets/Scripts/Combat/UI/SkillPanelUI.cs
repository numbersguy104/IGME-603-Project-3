using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelUI : MonoBehaviour
{
    public List<Skill> skillList;
    public List<TempSkillButton> buttonList;
    private void OnEnable()
    {
        // TODO: Temp Test Only
        Transform skillButtons = transform.Find("Skills");
        for (int i = 0; i < skillList.Count; i++)
        {
            TempSkillButton button = skillButtons.GetChild(i).GetComponent<TempSkillButton>();
            button.skill = skillList[i];
            buttonList.Add(button);
        }
    }

    public void UpdateSkillList(List<Skill> skillList)
    {
        this.skillList = skillList;
        // TODO

        foreach (var skillButton in buttonList)
        {
            Skill skill = skillButton.skill;
            switch (skill)
            {
                case AttackSkill attack:
                    skillButton.GetComponent<Button>().interactable =
                        PlayerController_Combat.Instance.currentCharacter.attacksAvailable > 0;
                    break;
            }
        }
    }

    public void CreateButton(Skill skill)
    {
        // TODO
    }
    
}
