using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private GridManager gridManager;

    public float moveSpeed = 5f;
    private Vector2 moveInput; // Movement input from the new Input System
    private Vector2Int playerGridPosition;

    private Vector3 targetPosition;  // Target position for mouse click movement
    private bool isMovingToClick;    // Flag to determine if we are moving to a click

    private Animator animator;  // Reference to the player's animator
    private Vector3 lastMoveDirection;  // Track last move direction for idle animation

    // SpriteRenderer for flipping the sprite
    private SpriteRenderer spriteRenderer;

    private PlayerInputActions playerInputActions;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        // Enable input actions
        playerInputActions.Player.Move.Enable();
        playerInputActions.Player.Click.Enable();   // Enable mouse click
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Click.performed += OnClick; // Mouse click movement
    }

    void OnDisable()
    {
        playerInputActions.Player.Move.Disable();
        playerInputActions.Player.Click.Disable();
    }

    void Start()
    {
        // Assign components in the scene

        gridManager = FindObjectOfType<GridManager>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();  // Get the SpriteRenderer to flip the sprite

        // Check if the gridRenderer or gridManager is null

        
        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
        }
    }

    void Update()
    {
        // Handle WASD movement input
        HandleMovement();

        // Handle mouse-click movement
        HandleMouseClickMovement();

        // Update the player's grid position continuously
        UpdatePlayerGridPosition(transform.position);
    }

    private void HandleMovement()
    {
        // Apply movement to the player using moveInput
        Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Update animation and sprite flip logic based on movement direction
        if (moveDirection.magnitude > 0)
        {
            lastMoveDirection = moveDirection;
            UpdateWalkDirection(moveDirection);
            FlipSprite(moveDirection);
        }
        else
        {
            UpdateIdleDirection(lastMoveDirection);  // Set idle when not moving
        }

        // Lock Z-axis to ensure player stays on the XY plane
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void HandleMouseClickMovement()
    {
        if (isMovingToClick)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Update animation and sprite flip logic based on movement direction
            if (moveDirection.magnitude > 0)
            {
                lastMoveDirection = moveDirection;
                UpdateWalkDirection(moveDirection);
                FlipSprite(moveDirection);
            }

            // If the player is close to the target position, stop moving
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMovingToClick = false;
                UpdateIdleDirection(lastMoveDirection);  // Set idle when reaching the target
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        UpdateIdleDirection(lastMoveDirection);  // Stop animation and switch to idle
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        // Get the world position from the mouse click
        Vector3 mouseWorldPosition = GetWorldPositionFromMouse();

        // Set the target position and start moving toward it
        targetPosition = mouseWorldPosition;
        isMovingToClick = true;

       // Debug.Log($"Moving to Target Position: {targetPosition}");
    }

    private void UpdatePlayerGridPosition(Vector3 playerWorldPosition)
    {
        Vector2Int newGridPosition = gridManager.ConvertWorldToGrid(playerWorldPosition);

        // If the player's grid position has changed, update it
        if (newGridPosition != playerGridPosition)
        {
            playerGridPosition = newGridPosition;
          //  Debug.Log($"Player moved to grid position: {playerGridPosition}");

            // Optionally, highlight the player's grid cell

        }
    }

    private Vector3 GetWorldPositionFromMouse()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
        Plane groundPlane = new Plane(Vector3.back, Vector3.zero);  // Assuming grid is on the XY plane (Z = 0)
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return Vector3.zero;  // Return a default position if no intersection
    }

    // Flip the sprite based on movement direction
    private void FlipSprite(Vector3 moveDirection)
    {
        // Flip the sprite horizontally if moving left (x < 0)
        if (moveDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    // Update the animation booleans based on movement direction (for walking)
    private void UpdateWalkDirection(Vector3 moveDirection)
    {
        ResetWalkAndIdleDirections();  // Reset all directions first

        // Determine which direction the player is moving and set the appropriate boolean
        if (moveDirection.x > 0 && moveDirection.y > 0) animator.SetBool("NE_Walk", true);  // North-East
        else if (moveDirection.x > 0 && moveDirection.y < 0) animator.SetBool("SE_Walk", true);  // South-East
        else if (moveDirection.x > 0) animator.SetBool("E_Walk", true);  // East
        else if (moveDirection.x < 0 && moveDirection.y > 0) animator.SetBool("NW_Walk", true);  // North-West
        else if (moveDirection.x < 0 && moveDirection.y < 0) animator.SetBool("SW_Walk", true);  // South-West
        else if (moveDirection.x < 0) animator.SetBool("W_Walk", true);  // West
        else if (moveDirection.y > 0) animator.SetBool("N_Walk", true);  // North
        else if (moveDirection.y < 0) animator.SetBool("S_Walk", true);  // South
    }

    // Update the idle direction based on the player's last movement direction (for idling)
    private void UpdateIdleDirection(Vector3 lastMoveDirection)
    {
        ResetWalkAndIdleDirections();  // Reset all directions first

        // Determine which direction the player was facing when they stopped and set the appropriate idle boolean
        if (lastMoveDirection.x > 0 && lastMoveDirection.y > 0) animator.SetBool("NE_Idle", true);  // North-East idle
        else if (lastMoveDirection.x > 0 && lastMoveDirection.y < 0) animator.SetBool("SE_Idle", true);  // South-East idle
        else if (lastMoveDirection.x > 0) animator.SetBool("E_Idle", true);  // East idle
        else if (lastMoveDirection.x < 0 && lastMoveDirection.y > 0) animator.SetBool("NW_Idle", true);  // North-West idle
        else if (lastMoveDirection.x < 0 && lastMoveDirection.y < 0) animator.SetBool("SW_Idle", true);  // South-West idle
        else if (lastMoveDirection.x < 0) animator.SetBool("W_Idle", true);  // West idle
        else if (lastMoveDirection.y > 0) animator.SetBool("N_Idle", true);  // North idle
        else if (lastMoveDirection.y < 0) animator.SetBool("S_Idle", true);  // South idle
    }

    // Reset all walk and idle animation booleans
    private void ResetWalkAndIdleDirections()
    {
        // Reset walking booleans
        animator.SetBool("N_Walk", false);
        animator.SetBool("NE_Walk", false);
        animator.SetBool("E_Walk", false);
        animator.SetBool("SE_Walk", false);
        animator.SetBool("S_Walk", false);
        animator.SetBool("SW_Walk", false);
        animator.SetBool("W_Walk", false);
        animator.SetBool("NW_Walk", false);

        // Reset idle booleans
        animator.SetBool("N_Idle", false);
        animator.SetBool("NE_Idle", false);
        animator.SetBool("E_Idle", false);
        animator.SetBool("SE_Idle", false);
        animator.SetBool("S_Idle", false);
        animator.SetBool("SW_Idle", false);
        animator.SetBool("W_Idle", false);
        animator.SetBool("NW_Idle", false);
    }
}














