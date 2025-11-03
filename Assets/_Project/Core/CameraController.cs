using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 3f;  // Speed of zoom transition
    public float minZoom = 3f;
    public float maxZoom = 24f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void OnEnable()
    {
        InputManager.InputActions.Camera.Enable();
        InputManager.InputActions.Camera.ScrollZoom.performed += UpdateScrollZoom;
    }

    private void OnDisable()
    {
        InputManager.InputActions.Camera.Disable();
    }

    void Start()
    {
        if (DeviceUtils.IsMobile())
        {
            cam.orthographicSize = 24f; // zoomed out for smaller screens
        }
        else
        {
            cam.orthographicSize = 20f;
        }
    }

    private void UpdateScrollZoom(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        float direction = Mathf.Sign(scrollValue);

        if (Mathf.Abs(scrollValue) > 0.01f)
        {
            Zoom(direction);
        }
    }

    // TODO: Nice to have pinch zoom, but right now I can't seem to get it
    // to detect it nicely and it also triggers the PointMove action. So I'm
    // leaving it for now.
    // Code is taken from https://www.arcaneshift.com/blog/2023/06/19/pinch-and-scroll-with-unitys-new-input-system/
    // public void UpdatePinchZoom(InputAction.CallbackContext context)
    // {

    //     // if there are not two active touches, return
    //     if (Touch.activeTouches.Count < 2)
    //         return;

    //     // get the finger inputs
    //     Touch primary = Touch.activeTouches[0];
    //     Touch secondary = Touch.activeTouches[1];

    //     // check if none of the fingers moved, return
    //     if (primary.phase == TouchPhase.Moved || secondary.phase == TouchPhase.Moved)
    //     {
    //         // if fingers have no history, then return
    //         if (primary.history.Count < 1 || secondary.history.Count < 1)
    //             return;

    //         // calculate distance before and after touch movement
    //         float currentDistance = Vector2.Distance(primary.screenPosition, secondary.screenPosition);
    //         float previousDistance = Vector2.Distance(primary.history[0].screenPosition, secondary.history[0].screenPosition);

    //         // the zoom distance is the difference between the previous distance and the current distance
    //         float pinchDistance = currentDistance - previousDistance;
    //         Zoom(pinchDistance * 0.1f);
    //     }
    // }

    public void Zoom(float zoomDelta)
    {
        // Debug.Log("Zoom: " + zoomDelta);
        cam.orthographicSize -= zoomDelta * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }


}