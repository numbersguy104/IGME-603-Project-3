using TMPro;
using UnityEngine;

public class StatsTextUpdater : MonoBehaviour
{
    private enum Characters
    {
        HUGO,
        TENET
    }

    [SerializeField] private Characters character;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private TextMeshProUGUI levelText;

    void Start()
    {
        int charIndex = 0;
        if (character == Characters.TENET)
        {
            charIndex = 1;
        }
        Character characterStats = CharacterStatsManager.Instance.characters[charIndex];

        if (healthText != null)
        {
            healthText.text = characterStats.CurrentHealth + "/" + characterStats.MaxHealth;
        }

        if (expText != null)
        {
            expText.text = "EXP: " + characterStats.exp + "/" + characterStats.expToNextLevel;
        }

        if (levelText != null)
        {
            levelText.text = "Level " + characterStats.level;
        }
    }
}
