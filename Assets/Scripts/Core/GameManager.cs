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
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.MainMenu:
                EssentialsManager._instance.player.gameObject.SetActive(false);
                break;
            case GameState.Loading:
                EssentialsManager._instance.player.gameObject.SetActive(false);
                
                break;
            case GameState.Playing:
                EssentialsManager._instance.player.gameObject.SetActive(true);
                break;
            case GameState.Paused:
                
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                EssentialsManager._instance.player.gameObject.SetActive(false);
                break;
        }
    }

    public void LoadGame(string scenename)
    {
        UpdateGameState(GameState.Loading);
        EssentialsManager._instance.sceneController.LoadScene(scenename);
    }



    public void StartGame()
    {
        UpdateGameState(GameState.Playing);
        Vector3 spawnPosition = GameObject.Find("PlayerSpawnPoint").transform.position;
        GameObject player = EssentialsManager._instance.player.gameObject;
        player.transform.position = spawnPosition;
    }

}

