using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPointDisplay : MonoBehaviour
{
    private Image SPImage;
    private TMP_Text text;
    private Image StaminaImage;
    public Color HugoSPColor;
    public Color TenetSPColor;
    private void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();
        if(images.Length == 1)
            SPImage = images[0];
        else
        {
            StaminaImage = images[0];
            SPImage = images[1];
        }
        text = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        UpdateSPInfo();
        if(PlayerController_Combat.Instance.currentCharacter != null)
            PlayerController_Combat.Instance.currentCharacter.OnSPChanged += UpdateSPInfo;
        CombatUI.Instance.OnCombatInfoUpdated.AddListener(UpdateSPInfo);
    }

    private void OnDisable()
    {
        PlayerController_Combat.Instance.currentCharacter.OnSPChanged -= UpdateSPInfo;
        CombatUI.Instance.OnCombatInfoUpdated.RemoveListener(UpdateSPInfo);
    }

    public void UpdateSPInfo()
    {
        if (PlayerController_Combat.Instance.currentCharacterName == Characters.HUGO)
        {
            SPImage.color = HugoSPColor;
            if (StaminaImage != null)
            {
                StaminaImage.gameObject.SetActive(false);
            }
        }
        else
        {
            SPImage.color = TenetSPColor;
            if (StaminaImage != null)
            {
                StaminaImage.gameObject.SetActive(true);
                StaminaImage.fillAmount = PlayerController_Combat.Instance.currentCharacter.movesAvailable;
            }
        }
        text.SetText(PlayerController_Combat.Instance.currentCharacter?.CurrentSkillPoint.ToString());
    }
}
