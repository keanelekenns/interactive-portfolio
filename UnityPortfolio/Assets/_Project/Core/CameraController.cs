using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 3f;  // Speed of zoom transition
    public float minZoom = 3f;
    public float maxZoom = 24f;

    private Camera cam;
    private float targetZoom;
    private PlayerInputActions inputActions;

    void Awake()
    {
        cam = GetComponent<Camera>();

        inputActions = new PlayerInputActions();
        targetZoom = cam.orthographicSize;
    }
    private void OnEnable()
    {
        inputActions.Camera.Enable();
        inputActions.Camera.ScrollZoom.performed += UpdateScrollZoom;
    }

    private void OnDisable()
    {
        inputActions.Camera.Disable();
    }

    private void UpdateScrollZoom(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        float direction = Mathf.Sign(scrollValue);

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            cam.orthographicSize -= direction * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }


}