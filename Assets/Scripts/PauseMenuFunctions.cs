using NUnit.Framework.Interfaces;
using UnityEngine;

public class PauseMenuFunctions : MonoBehaviour
{
    bool isPaused = false;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject skillsMenu;

    [SerializeField]
    GameObject itemsMenu;

    [SerializeField]
    GameObject configMenu;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                skillsMenu.SetActive(false);
                itemsMenu.SetActive(false);
                configMenu.SetActive(false);
                pauseMenu.SetActive(true);
                isPaused = true;
            }

            else
            {
                Time.timeScale = 1f;
                skillsMenu.SetActive(false);
                itemsMenu.SetActive(false);
                configMenu.SetActive(false);
                pauseMenu.SetActive(false);
                isPaused = false;
            }
        }
    }
}
