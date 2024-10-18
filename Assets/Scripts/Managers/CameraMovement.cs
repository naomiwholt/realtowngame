using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public Transform target;  // The target to follow (usually the player)
    public float smoothSpeed = 0.125f;  // How smooth the camera movement should be
    public float zoomSpeed = 1f;  // Speed of zooming in/out
    public float minZoom = 5f;  // Minimum zoom level
    public float maxZoom = 15f;  // Maximum zoom level
    public float initialZoom = 10f;  // Initial zoom level

    private Camera mainCamera;
    private CameraInputActions cameraInputActions;
    private float zoomInput;

    void Awake()
    {
        mainCamera = Camera.main;
        cameraInputActions = new CameraInputActions();

        // Set the initial orthographic size
        mainCamera.orthographicSize = initialZoom;
    }

    void OnEnable()
    {
        cameraInputActions.Camera.Enable();
        cameraInputActions.Camera.Zoom.performed += OnZoom;
        cameraInputActions.Camera.Zoom.canceled += OnZoom;
    }

    void OnDisable()
    {
        cameraInputActions.Camera.Zoom.performed -= OnZoom;
        cameraInputActions.Camera.Zoom.canceled -= OnZoom;
        cameraInputActions.Camera.Disable();
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Fix the Z-coordinate to -10f to ensure correct depth
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10f);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        HandleZoom();
    }

    private void HandleZoom()
    {
        if (zoomInput != 0f)
        {
            float newSize = mainCamera.orthographicSize - zoomInput * zoomSpeed * Time.deltaTime;
            mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }
}



