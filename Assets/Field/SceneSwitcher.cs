using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// SceneSwitcher allows switching between scenes in a specified order.
/// It persists across scene loads and can be triggered by pressing the Tab key.
/// </summary>
public class SceneSwitcher : MonoBehaviour
{
    [Header("Temp Switch")]
    [SerializeField] private string[] sceneOrder; // Example: ["FieldTest", "AnotherScene"]
    [SerializeField] private int startIndex = 0;

    private int _index;
    private bool _isSwitching;

    /// <summary>
    /// Makes the root GameObject persistent across scene loads and initializes the starting scene index.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject.transform.root.gameObject);
        // Make the entire GameSystems persistent: you can also attach this script to a child of GameSystems.
        // Using root here ensures the whole GameSystems object is not destroyed.

        _index = startIndex;
    }

    /// <summary>
    /// Checks for Tab key input to trigger scene switching if not already switching.
    /// </summary>
    private void Update()
    {
        if (_isSwitching) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchToNextScene();
        }
    }

    /// <summary>
    /// Initiates switching to the next scene in the order.
    /// </summary>
    public void SwitchToNextScene()
    {
        if (sceneOrder == null || sceneOrder.Length == 0) return;
        StartCoroutine(SwitchRoutine());
    }

    /// <summary>
    /// Coroutine that handles the scene switching process, including optional state saving and restoration.
    /// </summary>
    private IEnumerator SwitchRoutine()
    {
        _isSwitching = true;

        // 1) (Optional) You can do: save state / stop input / close UI / fade out here
        // Example: Time.timeScale = 1f;

        _index = (_index + 1) % sceneOrder.Length;
        string next = sceneOrder[_index];

        // 2) Switch scene
        SceneManager.LoadScene(next);

        // 3) Wait one frame for the new scene to initialize (Awake/OnEnable)
        yield return null;

        // 4) (Optional) Do things here: rebind camera target / respawn character / restore state
        // Example: FindFirstObjectByType<CharacterTeamController>()?.RebindAfterSceneLoad();

        _isSwitching = false;
    }
}
