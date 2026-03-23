using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Skill skill;
    private Button button;
    private TMP_Text text;

    public bool interactable
    {
        get { return button.interactable; }
        set { button.interactable = value; }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();
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

    public void UpdateSkillInfo()
    {
        text.SetText(skill.skillData.skillName);
    }
}
