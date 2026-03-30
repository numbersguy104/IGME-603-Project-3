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
    private PauseMenuFunctions pauseMenu;

    [SerializeField] private GameObject skillSlotsEquipped;
    [SerializeField] private GameObject skillSlotsNotEquipped;
    
    [SerializeField] private GameObject skillButtonPrefab;
    [SerializeField] private TMP_Text description;
    
    private void Awake()
    {
        pauseMenu = GetComponentInParent<PauseMenuFunctions>();
    }

    private void OnEnable()
    {
        UpdateSkill(pauseMenu.isHugoSkill);
    }

    private void OnDisable()
    {
        UpdateCharacterSkillSet();
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
        InitPanel();
    }

    public void InitPanel()
    {
        int skillSlots = currentCharacter.maxSkillSlots;
        for (int i = 0; i < skillSlots; i++)
        {
            skillSlotsEquipped.transform.GetChild(i).GetComponent<Image>().enabled = true;
            skillSlotsEquipped.transform.GetChild(i).GetComponent<SkillSlot>().button = null;
            RemoveAllChildren(skillSlotsEquipped.transform.GetChild(i));
        }
        for (int i = skillSlots; i <= 2; i++)
            skillSlotsEquipped.transform.GetChild(i).GetComponent<Image>().enabled = false;
        for (int i = 0; i < skillSlotsNotEquipped.transform.childCount; i++)
            RemoveAllChildren(skillSlotsNotEquipped.transform.GetChild(i));
        
        for (int i = 0; i < skillEquipped.Count; i++)
        {
            SkillSlot slot = skillSlotsEquipped.transform.GetChild(i).GetComponent<SkillSlot>();
            RemoveAllChildren(skillSlotsEquipped.transform.GetChild(i));
            GameObject buttonGO = Instantiate(skillButtonPrefab, slot.transform);
            SkillSlotButton slotButton = buttonGO.GetComponent<SkillSlotButton>();
            slotButton.SetSkill(skillEquipped[i]);
            slotButton.SetSlot(slot);
            slotButton.isEquipped = true;
        }
        
        for (int i = 0; i < skillNotEquipped.Count; i++)
        {
            Transform slot = skillSlotsNotEquipped.transform.GetChild(i);
            RemoveAllChildren(slot);
            GameObject buttonGO = Instantiate(skillButtonPrefab, slot.transform);
            SkillSlotButton slotButton = buttonGO.GetComponent<SkillSlotButton>();
            slotButton.SetSkill(skillNotEquipped[i]);
            slotButton.isEquipped = false;
        }
    }

    public void EquipSkill(SkillSlot slot, SkillSlotButton skillSlotButton)
    {
        if (skillEquipped.Contains(skillSlotButton.skillData))
            return;
        skillEquipped.Add(skillSlotButton.skillData);
        skillNotEquipped.Remove(skillSlotButton.skillData);
        skillSlotButton.SetSlot(slot);
        skillSlotButton.isEquipped = true;
        List<GameObject> buttons = new List<GameObject>();
        for (int i = 0; i < skillSlotsNotEquipped.transform.childCount; i++)
        {
            if(skillSlotsNotEquipped.transform.GetChild(i).childCount > 0)
                buttons.Add(skillSlotsNotEquipped.transform.GetChild(i).GetChild(0).gameObject);
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i];
            button.transform.SetParent(skillSlotsNotEquipped.transform.GetChild(i));
            button.transform.localPosition = Vector3.zero;
        }
        UpdateCharacterSkillSet();
    }
    
    public void UnequipSkill(SkillSlotButton skillSlotButton)
    {
        if (skillNotEquipped.Contains(skillSlotButton.skillData))
            return;
        skillNotEquipped.Add(skillSlotButton.skillData);
        skillEquipped.Remove(skillSlotButton.skillData);
        skillSlotButton.slot.RemoveSkill();
        int i = skillNotEquipped.Count - 1;
        skillSlotButton.transform.SetParent(skillSlotsNotEquipped.transform.GetChild(i));
        skillSlotButton.transform.localPosition = Vector3.zero;
        skillSlotButton.slot = null;
        skillSlotButton.isEquipped = false;
        UpdateCharacterSkillSet();
    }


    public void Switch(SkillSlotButton skill_A, SkillSlotButton skill_B)
    {
        if (!skill_B.isEquipped)
        {
            skillEquipped.Add(skill_B.skillData);
            skillEquipped.Remove(skill_A.skillData);
            skillNotEquipped.Add(skill_A.skillData);
            skillNotEquipped.Remove(skill_B.skillData);
            SkillSlot slot_equipped = skill_A.slot;
            Transform slot_notEquipped = skill_B.oldParent;
            skill_A.transform.SetParent(slot_notEquipped);
            skill_A.transform.localPosition = Vector3.zero;
            skill_A.slot = null;
            skill_B.SetSlot(slot_equipped);
            skill_A.isEquipped = false;
            skill_B.isEquipped = true;
        }
        else
        {
            SkillSlot slot_A = skill_A.slot;
            SkillSlot slot_B = skill_B.slot;
            skill_A.SetSlot(slot_B);
            skill_B.SetSlot(slot_A);
        }
        UpdateCharacterSkillSet();
    }

    public void UpdateCharacterSkillSet()
    {
        currentCharacter.skillEquipped = this.skillEquipped;
    }

    public void UpdateDescription(SkillData skill)
    {
        description.SetText(skill.skillDescription);
    }

    public void RemoveAllChildren(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
            Destroy(root.GetChild(i).gameObject);
    }
}
