using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private IsometricDepthSorting depthSortingManager;  // Reference to the sorting manager

    public float moveSpeed = 5f;
    public Vector2 moveInput;
    public bool isMovingToClick = false;
    public float ovalWidth = 1.5f;  // Width of the oval (adjustable in the Inspector)
    public float ovalHeight = 0.75f;  // Height of the oval (adjustable in the Inspector)
    public float ovalYOffset = 0f;    // Adjustable Y-offset for the oval in the Inspector
    public bool showDebug = true;      // Toggle for visualizing CircleCast in the Inspector

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
        depthSortingManager = FindObjectOfType<IsometricDepthSorting>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (depthSortingManager == null)
        {
            Debug.LogError("IsometricDepthSorting manager not found in the scene!");
        }
    }

    void Update()
    {
        HandleMovement();
        HandleMouseClickMovement();
    }
    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0).normalized;
        float moveStep = moveSpeed * Time.deltaTime;

        // Adjust the center position based on Y offset
        Vector3 ovalCenter = new Vector3(transform.position.x, transform.position.y + ovalYOffset, 0);

        // Debug to visualize the oval bounds if debugging is enabled
        if (showDebug)
        {
            Debug.DrawRay(ovalCenter, moveDirection * moveStep, Color.red);  // Show movement direction
            DrawOval(ovalCenter, ovalWidth, ovalHeight, Color.green);        // Draw the collision detection oval
        }

        // Perform an approximation of an OvalCast by scaling the Y-dimension
        RaycastHit2D hit = Physics2D.CircleCast(ovalCenter, ovalWidth / 2, moveDirection, moveStep, LayerMask.GetMask("InteractableObject"));

        if (hit.collider == null)
        {
            // No collision detected, move the player
            transform.localPosition += moveDirection * moveStep;
            lastMoveDirection = moveDirection;

            // Handle animations
            UpdateWalkDirection(moveDirection);
            FlipSprite(moveDirection);
        }
        else if (hit.collider.CompareTag("Obstacle"))
        {
            // Collision detected, stop movement
            Debug.Log("Collision detected with: " + hit.collider.gameObject.name);

            // Optional: Draw the hit point for debugging
            if (showDebug)
            {
                Debug.DrawLine(ovalCenter, hit.point, Color.blue);
            }
        }
    }

    // Function to draw a circle for the CircleCast visualization
    private void DrawOval(Vector3 center, float width, float height, Color color)
    {
        int segments = 20;  // Adjust the number of segments for smoothness
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0) * width / 2, Mathf.Sin(0) * height / 2, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * width / 2, Mathf.Sin(angle) * height / 2, 0);
            Debug.DrawLine(prevPoint, nextPoint, color);
            prevPoint = nextPoint;
        }
    }







    private void HandleMouseClickMovement()
{
    if (isMovingToClick)
    {
        // Calculate the direction towards the target position
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Calculate the movement step based on speed and time
        float moveStep = moveSpeed * Time.deltaTime;

        // Log to check if mouse movement is being processed
       // Debug.Log("Moving towards: " + targetPosition + ", Move Step: " + moveStep);

        // Cast a ray to detect potential obstacles (if needed), otherwise move the player
        // Assuming no raycast obstacle for simplicity in this example
        transform.localPosition += moveDirection * moveStep;

        // Log the new local position after movement
      //  Debug.Log("New Position after mouse click (local): " + transform.localPosition);

        // Update animations and sorting if the player is moving
        if (moveDirection.magnitude > 0.1f)
        {
            lastMoveDirection = moveDirection;
            UpdateWalkDirection(moveDirection);
            FlipSprite(moveDirection);
            depthSortingManager.SortSpritesByDepthAndY();  // Update sorting order for the player
        }

        // Stop moving if we are close to the target position
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
        //Debug.Log("Move Input: " + moveInput);  // Add this to check if input is received correctly
        if (moveInput.magnitude > 0)
        {
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
        depthSortingManager.SortSpritesByDepthAndY();  // Update sorting order for the player
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
        Debug.Log("trigger box triggered");
        // Get the parent object that has the SpriteRenderer component
        SpriteRenderer staticObject = other.GetComponentInParent<SpriteRenderer>();
        if (staticObject != null && depthSortingManager != null)
        {
            depthSortingManager.AddToSortingList(staticObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Get the parent object that has the SpriteRenderer component
        SpriteRenderer staticObject = other.GetComponentInParent<SpriteRenderer>();
        if (staticObject != null && depthSortingManager != null)
        {
            depthSortingManager.RemoveFromSortingList(staticObject);
        }
    }

}














