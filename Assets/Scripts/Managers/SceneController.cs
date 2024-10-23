using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

// SceneController.cs
public class SceneController : MonoBehaviour
{
    public static SceneController _instance { get; private set; }

    public bool SceneLoadComplete;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        GameManager._instance.StartGame();
    }
}




