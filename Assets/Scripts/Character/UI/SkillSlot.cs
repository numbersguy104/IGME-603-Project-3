using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour, IDropHandler
{
    private SkillSettingUI skillSettingUI;
    public SkillSlotButton button;
    public void Awake()
    {
        skillSettingUI = GetComponentInParent<SkillSettingUI>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (button == null)
        {
            skillSettingUI.EquipSkill(this, eventData.pointerDrag.GetComponent<SkillSlotButton>());
        }
        else
        {
            GameObject GO = eventData.pointerDrag;
            if (GO.TryGetComponent(out SkillSlotButton otherButton))
            {
                skillSettingUI.Switch(button, otherButton);
            }
        }
    }

    public void RemoveSkill()
    {
        button = null;  
    }
}
