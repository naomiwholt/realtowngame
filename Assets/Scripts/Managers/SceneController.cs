using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;  // Needed for SceneAsset

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Scene References (Drag & Drop)")]
    public List<SceneAsset> sceneAssets;  // Drag & drop scenes from assets folder here

    private Dictionary<string, string> sceneNameMap = new Dictionary<string, string>();  // Stores scene names for reference

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSceneMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSceneMap()
    {
        // Populate the sceneNameMap with scene names from the list of SceneAssets
        sceneNameMap.Clear();

        foreach (SceneAsset sceneAsset in sceneAssets)
        {
            if (sceneAsset != null)
            {
                string sceneName = sceneAsset.name;  // Get the scene name from SceneAsset
                sceneNameMap.Add(sceneName, sceneName);  // Add to map for future reference
            }
        }
    }

    // Function to load a scene by name
    public void LoadScene(string sceneName)
    {
        if (sceneNameMap.ContainsKey(sceneName))
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in the list of scene references.");
        }
    }

    // Function to load a scene asynchronously, useful for large scenes
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // Optionally display a loading progress bar
            yield return null;
        }
    }

    // Function to reload the current scene
    public void ReloadScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene);
    }

    // Function to load the next scene (for linear level progression)
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            LoadScene(SceneManager.GetSceneByBuildIndex(nextSceneIndex).name);
        }
        else
        {
            Debug.LogWarning("No more scenes to load!");
        }
    }

    // Function to return to the main menu
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");  // Adjust the main menu scene name if necessary
    }
}


