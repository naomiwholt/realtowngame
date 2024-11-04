using System;
using UnityEngine;
using System.Collections;

// GameManager.cs
public class GameManager : MonoBehaviour
{
  

    public enum GameState { MainMenu, Loading, Playing, Paused, GameOver }
    public GameState currentState;

    public static event Action<GameState> OnGameStateChanged;



    private void Start()
    {
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);  // Notify all listeners of the state change

        switch(newState)
        {
            case GameState.MainMenu:              
            break;

            case GameState.Loading:
         
    
                break;

            case GameState.Playing:                      
            break;

            case GameState.Paused:           
            Time.timeScale = 0f;
            break;

            case GameState.GameOver:    
            break;
        }
    }

    public void LoadGame(string scenename)
    {
        UpdateGameState(GameState.Loading);
        EssentialsManager._instance.sceneController.LoadScene(scenename);
    }

   

    public void StartGame() // should maybe put a scene ready bool or something here later? 
    {
        UpdateGameState(GameState.Playing);    
    }
  
}

