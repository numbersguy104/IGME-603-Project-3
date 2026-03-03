using UnityEngine;
using UnityEngine.SceneManagement;

public class TabSceneToggle : MonoBehaviour
{
    [Header("Two scene names in Build Settings")]
    [SerializeField] private string sceneA = "SceneSettingNew";
    [SerializeField] private string sceneB = "CombatTest";

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab)) return;

        string current = SceneManager.GetActiveScene().name;
        string next = (current == sceneA) ? sceneB : sceneA;

        SceneManager.LoadScene(next);
    }
}