using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Vector2 moveInput;
    public bool isMovingToClick = false;
    public bool showDebug = true;

    private Vector3 targetPosition;
    private Vector3 lastMoveDirection;
    private PlayerInputActions playerInputActions;

    private PlayerAnimationController animationController;
    private DepthSortingManager depthSortingManager;

    private bool isPointerOverUI = false;
    public void Initialise()
    {
        playerInputActions = new PlayerInputActions();
        animationController = GetComponent<PlayerAnimationController>();
        playerInputActions.Player.Move.Enable();
        playerInputActions.Player.Click.Enable();
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Click.performed += OnClick;
    }

    private void Start()
    {
      //  gameObject.SetActive(false); // Ensure the player is initially disabled
        depthSortingManager = FindObjectOfType<DepthSortingManager>();
        if (depthSortingManager == null)
        {
            Debug.LogError("IsometricDepthSorting manager not found in the scene!");
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

   // private void OnTriggerExit2D(Collider2D other)
   // {
   //     if (depthSortingManager != null)
   //     {
   //         // Remove player from the sorting list on exiting the trigger
   //         depthSortingManager.RemoveFromSortingList(GetComponent<SpriteRenderer>());
   //
   //         // Remove other objects from the sorting list
   //         Transform parentObject = other.transform.parent;
   //         if (parentObject != null)
   //         {
   //             SpriteRenderer sprite = parentObject.GetComponent<SpriteRenderer>();
   //             if (sprite != null)
   //             {
   //                 depthSortingManager.RemoveFromSortingList(sprite);
   //             }
   //         }
   //     }
   // }

   

    void OnDisable()
    {
        playerInputActions.Player.Move.Disable();
        playerInputActions.Player.Click.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleMouseClickMovement();
    }

    private void HandleMovement()
    {
        Vector2 moveDirection = new Vector2(moveInput.x, moveInput.y).normalized;
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
            // If clicking on UI, don't move the player
            return;
        }

        Vector3 mouseWorldPosition = GetWorldPositionFromMouse();
        targetPosition = mouseWorldPosition;
        isMovingToClick = true;
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
}
















