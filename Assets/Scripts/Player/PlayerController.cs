using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

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
    private Rigidbody2D rb2D;
    private PlayerInputActions playerInputActions;
    private PlayerAnimationController animationController;

    // Coroutines
    private Coroutine mouseMoveCoroutine;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void Initialise()
    {
        playerInputActions = new PlayerInputActions();
        animationController = GetComponent<PlayerAnimationController>();

        // Enable input actions
        playerInputActions.Player.Move.Enable();
        playerInputActions.Player.Click.Enable();

        // Input callbacks
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Click.performed += OnClick;
    }

    private void OnDisable()
    {
        // Disable input actions
        playerInputActions.Player.Move.Disable();
        playerInputActions.Player.Click.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.magnitude > 0)
        {
            // Start continuous movement
            StartKeyboardMovement();
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;

        // Stop movement
        StopKeyboardMovement();
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // Ignore UI clicks

        targetPosition = GetWorldPositionFromMouse();

        if (mouseMoveCoroutine != null)
        {
            StopCoroutine(mouseMoveCoroutine);
        }

        mouseMoveCoroutine = StartCoroutine(MoveToClickPosition());
    }

    private IEnumerator MoveToClickPosition()
    {
        isMovingToClick = true;

        while (isMovingToClick)
        {
            Vector2 moveDirection = ((Vector2)targetPosition - rb2D.position).normalized;
            float moveStep = moveSpeed * Time.fixedDeltaTime;

            // Incrementally move towards the target
            Vector2 newPosition = rb2D.position + moveDirection * moveStep;
            rb2D.MovePosition(newPosition);

            if (Vector2.Distance(rb2D.position, targetPosition) < 0.1f)
            {
                isMovingToClick = false;
                animationController.PlayIdleAnimation(lastMoveDirection);
            }
            else
            {
                lastMoveDirection = moveDirection;
                animationController.PlayWalkAnimation(moveDirection);
            }

            yield return new WaitForFixedUpdate(); // Wait for the next physics step
        }
    }

    private void StartKeyboardMovement()
    {
        if (mouseMoveCoroutine != null)
        {
            StopCoroutine(mouseMoveCoroutine);
            isMovingToClick = false;
        }

        // Continuous movement coroutine
        StartCoroutine(KeyboardMovement());
    }

    private IEnumerator KeyboardMovement()
    {
        while (moveInput.magnitude > 0)
        {
            Vector2 moveDirection = moveInput.normalized;
            float moveStep = moveSpeed * Time.fixedDeltaTime;

            Vector2 newPosition = rb2D.position + moveDirection * moveStep;
            rb2D.MovePosition(newPosition);

            lastMoveDirection = moveDirection;
            animationController.PlayWalkAnimation(moveDirection);

            yield return new WaitForFixedUpdate(); // Wait for the next physics step
        }

        animationController.PlayIdleAnimation(lastMoveDirection);
    }

    private void StopKeyboardMovement()
    {
        StopAllCoroutines();
        animationController.PlayIdleAnimation(lastMoveDirection);
    }

    private Vector3 GetWorldPositionFromMouse()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        Plane groundPlane = new Plane(Vector3.back, Vector3.zero);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return Vector3.zero;
    }

 
}



















