using System;
using UnityEngine;

public class SkillSlotButton : MonoBehaviour
{
    public SkillSettingUI skillPanel;
    public SkillData skillData;

    public void Awake()
    {
        skillPanel = GetComponentInParent<SkillSettingUI>();
    }

    public void Switch()
    {
        skillPanel.SwitchSkill(skillData);
    }
}
