using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Character_Combat character = null;

    [SerializeField] private LayoutElement healthFull;
    [SerializeField] private LayoutElement healthMissing;

    //for the character icon below the health bar - currently unused
    [SerializeField] private Image icon;

    //Updates this health bar graphic to a fraction of remaining HP.
    //The input will be clamped to between 0 and 1 (inclusive).
    //Only affects the UI.
    public void UpdateHealthFraction(float fraction) {
        fraction = Mathf.Clamp(fraction, 0.0f, 1.0f);
        healthFull.flexibleHeight = fraction;
        healthMissing.flexibleHeight = 1 - fraction;
    }

    //Updates this health bar graphic with current and max HP values by calling UpdateHealthFraction.
    //Only affects the UI.
    public void UpdateHealthValue(float current, float max)
    {
        UpdateHealthFraction(current/max);
    }

    public void UpdateIcon(Sprite newIcon)
    {
        icon.sprite = newIcon;
    }
}
