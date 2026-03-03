using System;
using UnityEngine;
using UnityEngine.UI;

// Temporary script for skill button. For Week1 Test Only
public class TempSkillButton : MonoBehaviour
{
    public Skill skill;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(UseSkill);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(UseSkill);
    }

    public void UseSkill()
    {
        PlayerController_Combat.Instance.UseSkill(skill);
    }
}
