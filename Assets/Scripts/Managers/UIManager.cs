using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject mainMenuUI;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject hudUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicates
        }
    }

    private void OnEnable()
    {
        // Subscribe to the GameManager's state change event
        GameManager.OnGameStateChanged += UpdateUI;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        GameManager.OnGameStateChanged -= UpdateUI;
    }

    // Update the UI based on the current game state
    public void UpdateUI(GameManager.GameState gameState)
    {
        mainMenuUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        hudUI.SetActive(false);

        switch (gameState)
        {
            case GameManager.GameState.MainMenu:
                mainMenuUI.SetActive(true);
                break;
            case GameManager.GameState.Playing:
                hudUI.SetActive(true);
                break;
            case GameManager.GameState.Paused:
                pauseMenuUI.SetActive(true);
                break;
            case GameManager.GameState.GameOver:
                gameOverUI.SetActive(true);
                break;
        }
    }

    // Resume the game from pause
    public void OnResumeButton()
    {
        GameManager.Instance.TogglePause();
    }

    // Return to Main Menu
    public void OnMainMenuButton()
    {
        SceneController.Instance.LoadMainMenu();  // Use SceneController to load the Main Menu
    }

    // Start the game
    public void OnStartGameButton()
    {
        GameManager.Instance.UpdateGameState(GameManager.GameState.Playing);  // Switch to the Playing state
    }

    // Quit the game
    public void OnQuitButton()
    {
        Application.Quit();
    }
}


