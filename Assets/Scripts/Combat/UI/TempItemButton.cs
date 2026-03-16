using System;
using UnityEngine;
using UnityEngine.UI;

// Temporary script for item button. For Week1 Test Only
public class TempItemButton : MonoBehaviour
{
    public Item item;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button)
        {
            button.onClick.AddListener(UseItem);
        }
    }

    private void OnDisable()
    {
        if (button)
        {
            button.onClick.RemoveListener(UseItem);
        }
    }

    public void UseItem()
    {
        PlayerController_Combat.Instance.UseItem(item);
    }
}
