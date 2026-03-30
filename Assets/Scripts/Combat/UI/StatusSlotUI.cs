using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusSlotUI : MonoBehaviour
{
    public Status status;
    public int turnRemained;
    [SerializeField] private Image statusIcon;
    [SerializeField] private TMP_Text turnRemainedText;

    public void UpdateStatus(Status status)
    {
        this.status = status;
        statusIcon.sprite = status.statusData.statusIcon;
        if (status.TurnsRemained > 0)
            turnRemainedText.SetText(status.TurnsRemained.ToString());
        else
            turnRemainedText.enabled = false;
    }

    public void SetVisible(bool visible)
    {
        statusIcon.enabled = visible;
        turnRemainedText.enabled = visible;
    }
}
