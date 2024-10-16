using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState currentState;

    // Event to notify listeners of game state changes
    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState)
    {
        currentState = newState;

        // Notify all listeners of the state change
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.MainMenu:
                UIManager.Instance.UpdateUI(GameState.MainMenu);
                break;
            case GameState.Playing:
                UIManager.Instance.UpdateUI(GameState.Playing);
                SceneController.Instance.LoadScene("ApartmentDemo");
                break;
            case GameState.Paused:
                UIManager.Instance.UpdateUI(GameState.Paused);
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                UIManager.Instance.UpdateUI(GameState.GameOver);
                break;
        }
    }

    public void StartGame()
    {
        UpdateGameState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            UpdateGameState(GameState.Paused);
        }
        else if (currentState == GameState.Paused)
        {
            UpdateGameState(GameState.Playing);
            Time.timeScale = 1f;
        }
    }

    

    public void GameOver()
    {
        UpdateGameState(GameState.GameOver);
    }
}


