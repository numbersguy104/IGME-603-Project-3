using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillSlotButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rectTransform;
    Canvas canvas;
    public SkillSettingUI skillPanel;
    public SkillData skillData;

    public void Awake()
    {
        skillPanel = GetComponentInParent<SkillSettingUI>();
        rectTransform = GetComponent<RectTransform>();
        canvas =  GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

}
