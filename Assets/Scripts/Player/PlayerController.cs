using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // Configurable parameters
    public float moveSpeed = 5f;
    public bool showDebug = true;

    // Movement variables
    private Vector2 moveInput;
    private Vector3 targetPosition;
    private Vector3 lastMoveDirection;
    private bool isMovingToClick = false;

    // Component and manager references
    private PlayerInputActions playerInputActions;
    private PlayerAnimationController animationController;
    private DepthSortingManager depthSortingManager;
    private GridManager gridManager;

    // Vacuum mode
    private bool isVacuumModeOn = false;

    /// <summary>
    /// Initialize player input actions and references to other managers.
    /// </summary>
    public void Initialise()
    {
        playerInputActions = new PlayerInputActions();
        animationController = GetComponent<PlayerAnimationController>();

        // Enable input actions
        playerInputActions.Player.Move.Enable();
        playerInputActions.Player.Click.Enable();
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Click.performed += OnClick;

        // Retrieve manager references
        gridManager = EssentialsManager._instance.gridManager;
        depthSortingManager = EssentialsManager._instance.sortingManager;
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseClickMovement();

        if (isVacuumModeOn)
        {
            VacuumTiles();
        }
    }

    private void OnDisable()
    {
        playerInputActions.Player.Move.Disable();
        playerInputActions.Player.Click.Disable();
    }

    /// <summary>
    /// Handles player movement based on input.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 moveDirection = moveInput.normalized;
        float moveStep = moveSpeed * Time.deltaTime;

        Vector2 newPosition = (Vector2)transform.position + moveDirection * moveStep;
        Rigidbody2D rb2D = GetComponent<Rigidbody2D>();

        if (moveInput.magnitude > 0)
        {
            rb2D.MovePosition(newPosition);
            lastMoveDirection = moveDirection;
            animationController.PlayWalkAnimation(moveDirection);
        }
        else
        {
            animationController.PlayIdleAnimation(lastMoveDirection);
        }
    }

    /// <summary>
    /// Handles mouse click movement towards a target position.
    /// </summary>
    private void HandleMouseClickMovement()
    {
        if (isMovingToClick)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            float moveStep = moveSpeed * Time.deltaTime;

            transform.localPosition += moveDirection * moveStep;

            if (moveDirection.magnitude > 0.1f)
            {
                lastMoveDirection = moveDirection;
                animationController.PlayWalkAnimation(moveDirection);
            }

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMovingToClick = false;
                animationController.PlayIdleAnimation(lastMoveDirection);
            }
        }
    }

    /// <summary>
    /// Toggles vacuum mode on or off.
    /// </summary>
    public void ToggleVacuumMode()
    {
        isVacuumModeOn = !isVacuumModeOn;
        Debug.Log($"Vacuum mode {(isVacuumModeOn ? "enabled" : "disabled")}");
    }

    /// <summary>
    /// Cleans tiles intersecting with the player's position or nearby colliders.
    /// </summary>
    private void VacuumTiles()
    {
        if (gridManager == null) return;

        Vector3Int playerGridPosition = gridManager.tilemap.WorldToCell(transform.position);
        gridManager.ClearTile(playerGridPosition);

        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collider in overlappingColliders)
        {
            Vector3 colliderPosition = collider.transform.position;
            Vector3Int colliderGridPosition = gridManager.tilemap.WorldToCell(colliderPosition);
            gridManager.ClearTile(colliderGridPosition);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (depthSortingManager != null)
        {
            depthSortingManager.AddToDynamicSorting(GetComponent<SpriteRenderer>());

            Transform parentObject = other.transform.parent;
            if (parentObject != null)
            {
                SpriteRenderer sprite = parentObject.GetComponent<SpriteRenderer>();
                if (sprite != null)
                {
                    depthSortingManager.AddToDynamicSorting(sprite);
                }
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
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Ignore UI clicks
        }

        targetPosition = GetWorldPositionFromMouse();
        isMovingToClick = true;
    }

    private Vector3 GetWorldPositionFromMouse()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
        Plane groundPlane = new Plane(Vector3.back, Vector3.zero);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return Vector3.zero;
    }
}

















