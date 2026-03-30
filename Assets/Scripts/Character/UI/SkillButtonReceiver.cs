using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButtonReceiver : MonoBehaviour, IDropHandler
{
    private SkillSettingUI skillSettingUI;
    
    
    private void Awake()
    {
        skillSettingUI = GetComponentInParent<SkillSettingUI>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        SkillSlotButton button = eventData.pointerDrag.GetComponent<SkillSlotButton>();
        skillSettingUI.UnequipSkill(button);
    }
}
