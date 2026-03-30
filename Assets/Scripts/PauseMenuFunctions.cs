using System;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    GameObject skillsHugo;

    [SerializeField]
    GameObject skillsTenet;

    public bool isHugoSkill => skillsHugo.activeSelf;

    private void Start()
    {
        skillsMenu.GetComponent<SkillSettingUI>().UpdateSkill(skillsHugo.activeSelf);
    }

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

    public void Resume()
    {
        Time.timeScale = 1f;
        skillsMenu.SetActive(false);
        itemsMenu.SetActive(false);
        configMenu.SetActive(false);
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    public void OnSkillsMenuClick() 
    {
        pauseMenu.SetActive(false);
        skillsMenu.SetActive(true);
    }

    public void OnItemsMenuClick() 
    {
        pauseMenu.SetActive(false);
        itemsMenu.SetActive(true);
    }

    public void OnConfigMenuClick() 
    {
        pauseMenu.SetActive(false);
        configMenu.SetActive(true);
    }

    public void OnSwapClick() 
    {
        skillsHugo.SetActive(!skillsHugo.activeSelf);
        skillsTenet.SetActive(!skillsTenet.activeSelf);
        skillsMenu.GetComponent<SkillSettingUI>().UpdateSkill(skillsHugo.activeSelf);
    }

    public void OnBackClick()
    {
        skillsMenu.SetActive(false);
        itemsMenu.SetActive(false);
        configMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void OnReturnToTitleClick()
    {
        SceneManager.LoadScene("Scenes/TitleScreen");
    }
}
