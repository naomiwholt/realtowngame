using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GridManager gridManager;

    private IsometricDepthSorting depthSortingManager;        // Reference to the sorting manager

    public float moveSpeed = 5f;
    public Vector2 moveInput;
    public bool isMovingToClick = false;

    private Vector2Int playerGridPosition;
    private Vector3 targetPosition;

    private Animator animator;
    private Vector3 lastMoveDirection;
    private SpriteRenderer spriteRenderer;

    private PlayerInputActions playerInputActions;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        playerInputActions.Player.Move.Enable();
        playerInputActions.Player.Click.Enable();
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Click.performed += OnClick;
    }

    void OnDisable()
    {
        playerInputActions.Player.Move.Disable();
        playerInputActions.Player.Click.Disable();
    }

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
       // dynamicSortingComponent = GetComponent<DynamicSortingComponent>();
        depthSortingManager = FindObjectOfType<IsometricDepthSorting>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
        }

    

        if (depthSortingManager == null)
        {
            Debug.LogError("IsometricDepthSorting manager not found in the scene!");
        }
    }

    void Update()
    {
        HandleMovement();
        HandleMouseClickMovement();
        UpdatePlayerGridPosition(transform.position);

        // Sorting updates now controlled through movement state, no need for constant checks here
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (moveDirection.magnitude > 0)
        {
            lastMoveDirection = moveDirection;
            UpdateWalkDirection(moveDirection);
            FlipSprite(moveDirection);
          //  dynamicSortingComponent.UpdateSortingOrder();  // Sorting only updated while moving
            depthSortingManager.SortSpritesByDepthAndY();  // Update sorting order for the player
        }
        else
        {
            UpdateIdleDirection(lastMoveDirection);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void HandleMouseClickMovement()
    {
        if (isMovingToClick)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            if (moveDirection.magnitude > 0.1)
            {
                lastMoveDirection = moveDirection;
                UpdateWalkDirection(moveDirection);
                FlipSprite(moveDirection);
             //   dynamicSortingComponent.UpdateSortingOrder();  // Sorting updated when moving to click
                depthSortingManager.SortSpritesByDepthAndY();  // Update sorting order for the player
            }

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMovingToClick = false;
                UpdateIdleDirection(lastMoveDirection);
            }
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.magnitude > 0)
        {
           // dynamicSortingComponent.UpdateSortingOrder();  // Trigger sorting when movement starts
            depthSortingManager.SortSpritesByDepthAndY();  // Update sorting order for the player
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        UpdateIdleDirection(lastMoveDirection);
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector3 mouseWorldPosition = GetWorldPositionFromMouse();
        targetPosition = mouseWorldPosition;
        isMovingToClick = true;
       // dynamicSortingComponent.UpdateSortingOrder();  // Trigger sorting when clicking to move
        depthSortingManager.SortSpritesByDepthAndY();  // Update sorting order for the player
    }

    private void UpdatePlayerGridPosition(Vector3 playerWorldPosition)
    {
        Vector2Int newGridPosition = gridManager.ConvertWorldToGrid(playerWorldPosition);

        if (newGridPosition != playerGridPosition)
        {
            playerGridPosition = newGridPosition;
        }
    }

    private Vector3 GetWorldPositionFromMouse()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
        Plane groundPlane = new Plane(Vector3.back, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return Vector3.zero;
    }

    private void FlipSprite(Vector3 moveDirection)
    {
        if (moveDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void UpdateWalkDirection(Vector3 moveDirection)
    {
        ResetWalkAndIdleDirections();

        if (moveDirection.x > 0 && moveDirection.y > 0) animator.SetBool("NE_Walk", true);
        else if (moveDirection.x > 0 && moveDirection.y < 0) animator.SetBool("SE_Walk", true);
        else if (moveDirection.x > 0) animator.SetBool("E_Walk", true);
        else if (moveDirection.x < 0 && moveDirection.y > 0) animator.SetBool("NW_Walk", true);
        else if (moveDirection.x < 0 && moveDirection.y < 0) animator.SetBool("SW_Walk", true);
        else if (moveDirection.x < 0) animator.SetBool("W_Walk", true);
        else if (moveDirection.y > 0) animator.SetBool("N_Walk", true);
        else if (moveDirection.y < 0) animator.SetBool("S_Walk", true);
    }

    private void UpdateIdleDirection(Vector3 lastMoveDirection)
    {
        ResetWalkAndIdleDirections();

        if (lastMoveDirection.x > 0 && lastMoveDirection.y > 0) animator.SetBool("NE_Idle", true);
        else if (lastMoveDirection.x > 0 && lastMoveDirection.y < 0) animator.SetBool("SE_Idle", true);
        else if (lastMoveDirection.x > 0) animator.SetBool("E_Idle", true);
        else if (lastMoveDirection.x < 0 && lastMoveDirection.y > 0) animator.SetBool("NW_Idle", true);
        else if (lastMoveDirection.x < 0 && lastMoveDirection.y < 0) animator.SetBool("SW_Idle", true);
        else if (lastMoveDirection.x < 0) animator.SetBool("W_Idle", true);
        else if (lastMoveDirection.y > 0) animator.SetBool("N_Idle", true);
        else if (lastMoveDirection.y < 0) animator.SetBool("S_Idle", true);
    }

    private void ResetWalkAndIdleDirections()
    {
        animator.SetBool("N_Walk", false);
        animator.SetBool("NE_Walk", false);
        animator.SetBool("E_Walk", false);
        animator.SetBool("SE_Walk", false);
        animator.SetBool("S_Walk", false);
        animator.SetBool("SW_Walk", false);
        animator.SetBool("W_Walk", false);
        animator.SetBool("NW_Walk", false);

        animator.SetBool("N_Idle", false);
        animator.SetBool("NE_Idle", false);
        animator.SetBool("E_Idle", false);
        animator.SetBool("SE_Idle", false);
        animator.SetBool("S_Idle", false);
        animator.SetBool("SW_Idle", false);
        animator.SetBool("W_Idle", false);
        animator.SetBool("NW_Idle", false);
    }

    // Trigger logic to detect nearby static objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Get the parent object that has the SpriteRenderer component
        SpriteRenderer staticObject = other.GetComponentInParent<SpriteRenderer>();
      //  Debug.Log("entering " + other.gameObject.name);
        if (staticObject != null && depthSortingManager != null)
        {
            depthSortingManager.AddToSortingList(staticObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Get the parent object that has the SpriteRenderer component
        SpriteRenderer staticObject = other.GetComponentInParent<SpriteRenderer>();

      //.  Debug.Log("exiting " + other.gameObject.name);
        if (staticObject != null && depthSortingManager != null)
        {
            depthSortingManager.RemoveFromSortingList(staticObject);
        }
    }
}














