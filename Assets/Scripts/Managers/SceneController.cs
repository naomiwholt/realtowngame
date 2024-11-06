using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

// SceneController.cs
public class SceneController : MonoBehaviour
{


    public bool SceneLoadComplete;


    private void OnEnable()
    {
        GameManager.OnGameStateChanged += UpdateForGameState;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= UpdateForGameState;
    }
    private void UpdateForGameState(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.MainMenu:
         
                break;

            case GameManager.GameState.Loading:
        
                break;

            case GameManager.GameState.Playing:
         
                break;

            case GameManager.GameState.Paused:
                // Handle paused state, e.g., freezing player movement
                break;

            case GameManager.GameState.GameOver:
                // Handle game over state if needed
                break;
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneLoadComplete = true;
        Debug.Log(sceneName + " is loaded.");
        EssentialsManager._instance.gameManager.StartGame();
    }

    public Scene GetCurrentGameplayScene()
    {
        // Iterate through loaded scenes, skipping the "Don't Destroy On Load" scene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            // Skip "Don't Destroy On Load" scene, which has an empty name
            if (!string.IsNullOrEmpty(scene.name) && scene.isLoaded)
            {
                return scene;
            }
        }

        Debug.LogWarning("No active gameplay scene found besides the persistent scene.");
        return default;
    }
}




