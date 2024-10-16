using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public float zoomSpeed = 1f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    private Camera mainCamera;
    private CameraInputActions cameraInputActions;
    private float zoomInput;

    void Awake()
    {
        mainCamera = Camera.main;
        cameraInputActions = new CameraInputActions();
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

        // Unsubscribe from events

      
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
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



