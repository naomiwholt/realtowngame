using UnityEngine;

public class EssentialsManager : MonoBehaviour
{
    private static EssentialsManager _instance;

    public GameObject gridManager;
    public GameObject sortingManager;
    public GameObject Player;

    

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
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }

    private void HandleGameStateChange(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.MainMenu:
                gridManager.SetActive(false);
                sortingManager.SetActive(false);
                Player.SetActive(false);
                break;

            case GameManager.GameState.Loading:
                
                break;

            case GameManager.GameState.Playing:
                // Turn on essential managers
                gridManager.SetActive(true);
                sortingManager.SetActive(true);
                Player.SetActive(true);
                break;

            case GameManager.GameState.Paused:
                // Handle paused state, e.g., freezing player movement
                break;

            case GameManager.GameState.GameOver:
                // Handle game over state if needed
                break;
        }
    }

  

}


