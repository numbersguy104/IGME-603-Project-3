using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rectTransform;
    Canvas canvas;
    public SkillSettingUI skillSettingUI;
    public Button button;
    public TMP_Text text;
    public CanvasGroup buttonCanvasGroup;
    public SkillSlot slot;
    public SkillData skillData;
    public UnityAction ButtonSelected => () => skillSettingUI.UpdateDescription(skillData);

    public bool isEquipped;

    public void Awake()
    {
        skillSettingUI = GetComponentInParent<SkillSettingUI>();
        rectTransform = GetComponent<RectTransform>();
        canvas =  GetComponentInParent<Canvas>();
        button = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();
        buttonCanvasGroup =  GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(ButtonSelected);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(ButtonSelected);
    }

    public Transform oldParent;
    public void OnBeginDrag(PointerEventData eventData)
    {
        oldParent = transform.parent;
        rectTransform.SetParent(canvas.transform);
        buttonCanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            null,
            out localPos
        );
        rectTransform.anchoredPosition = localPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StartCoroutine(ResetAfterOneFrame());

        IEnumerator ResetAfterOneFrame()
        {
            yield return null;
            buttonCanvasGroup.blocksRaycasts = true;
            if (transform.parent == canvas.transform)
            {
                transform.SetParent(oldParent);
            }
            transform.localPosition =  Vector3.zero;
        }
    }

    // public void OnDrop(PointerEventData eventData)
    // {
    //     GameObject GO = eventData.pointerDrag;
    //     if (isEquipped && GO.TryGetComponent(out SkillSlotButton button))
    //     {
    //         skillSettingUI.Switch(this, button);
    //     }
    // }

    public void SetSlot(SkillSlot slot)
    {
        this.slot = slot;
        transform.SetParent(slot.transform);
        slot.button = this;
        transform.localPosition = Vector3.zero;
    }

    public void SetSkill(SkillData skillData)
    {
        this.skillData = skillData;
        text.SetText(skillData.skillName);
    }

}
