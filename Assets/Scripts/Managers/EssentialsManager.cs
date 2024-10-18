using UnityEngine;

public class EssentialsManager : MonoBehaviour
{
    private static EssentialsManager _instance;

    public GameObject gridManager;  // Reference to the GridManager
    public GameObject sortingManager;  // Reference to the SortingManager
    public GameObject Player;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);  // Persist the Essentials across scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
    }

    private void OnEnable()
    {
        // Subscribe to the GameManager's state change event
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }

    // Handle GameState changes
    private void HandleGameStateChange(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.MainMenu:
                gridManager.SetActive(false);
                sortingManager.SetActive(false);
                Player.SetActive(false);
                break;
            case GameManager.GameState.Playing:
                Player.SetActive(true);
                sortingManager.SetActive(true);
                gridManager.SetActive(true);
               
                
                break;
            case GameManager.GameState.Paused:
                // Optionally handle pause logic
                break;
            case GameManager.GameState.GameOver:
                // Optionally handle game over logic
                break;
        }
    }
}

