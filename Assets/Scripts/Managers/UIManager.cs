using UnityEngine;

public class UIManager : MonoBehaviour
{
   

    public GameObject mainMenuUI;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject hudUI;
    public GameObject loadingScreenUI;


    private void Update()
    {
        // Update the UI based on the current game state in every frame
        UpdateUI(EssentialsManager._instance.gameManager.currentState);
    }


    public void LoadingSceneButtonPressed(string scenename)
    {
        // Button calls this to start the game
        EssentialsManager._instance.gameManager.LoadGame(scenename);
    }

    // The switch statement to handle UI updates
    public void UpdateUI(GameManager.GameState gameState)
    {
        // Deactivate all panels first
        mainMenuUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        hudUI.SetActive(false);
        loadingScreenUI.SetActive(false);

        // Switch to activate the appropriate UI based on the current state
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
            case GameManager.GameState.Loading:
                loadingScreenUI.SetActive(true);
                break;
        }
    }

    // Optionally, you can still use these methods for manual control
    public void ShowLoadingScreen()
    {
        loadingScreenUI.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        loadingScreenUI.SetActive(false);
    }
}

