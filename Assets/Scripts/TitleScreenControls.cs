using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenControls : MonoBehaviour
{
    public void OnStartClick() 
    {
        SceneManager.LoadScene("SceneSettingMenuTest");
    }

    public void OnQuitClick() 
    {
        Application.Quit();
    }
}
