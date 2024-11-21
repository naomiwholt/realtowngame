using UnityEngine;

public class EssentialsManager : MonoBehaviour

{
    /*the essentials manager is what makes all the other managers persistent across scenes. if we  need to reference another Manager  in a manager script 
     will go through essentials manager to get refeferences for other managers*/

    public static EssentialsManager _instance { get; private set; }

    public GameManager gameManager;
    public UIManager uiManager;
    public SceneController sceneController;
    public CameraMovement cameraManager;
    public GridManager gridManager;
    public DepthSortingManager sortingManager;
    public PlayerController player;
    public ObjectPlacementManager objectPlacementManager;
    public InventoryManager inventoryManager;



    //Game Objects

    
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

        print("Essentials Manager Awake");
        GetManagersReferences();


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
                InitializeManagersForMainMenu();
                break;

            case GameManager.GameState.Playing:
                InitializeManagersForPlay();
                break;

            case GameManager.GameState.Paused:
                InitializeManagersForPause();
                break;

            case GameManager.GameState.GameOver:
                InitializeManagersForGameOver();
                break;
        }
    }

    private void GetManagersReferences()
    {
         
        gameManager = GetComponentInChildren<GameManager>();
        uiManager = GetComponentInChildren<UIManager>();
        sceneController = GetComponentInChildren<SceneController>();
        cameraManager = GetComponentInChildren<CameraMovement>();
        gridManager = GetComponentInChildren<GridManager>();
        sortingManager = GetComponentInChildren<DepthSortingManager>();
        player = GetComponentInChildren<PlayerController>();
        objectPlacementManager = GetComponentInChildren<ObjectPlacementManager>();
        inventoryManager = GetComponentInChildren<InventoryManager>();
    }

    private void InitializeManagersForMainMenu()
    {
       gridManager.gameObject.SetActive(false);
    }

    // Remove gridManager activation from InitializeManagersForPlay
    private void InitializeManagersForPlay()
    {
        objectPlacementManager.Initialise();
        sortingManager.InitialiseSorting();
        inventoryManager.Initialise();
        player.Initialise();
    }

    private void InitializeManagersForPause()
    {

    }

    private void InitializeManagersForGameOver()
    {
      
    }




}



