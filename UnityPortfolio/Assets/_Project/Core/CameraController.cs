using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 20f;         // Speed of zoom transition
    public float minZoom = 3f;
    public float maxZoom = 25f;

    private Camera cam;
    private float targetZoom;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        // Handle input
        if (Input.GetKey(KeyCode.Equals)) // Zoom in
        {
            targetZoom -= zoomSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Minus)) // Zoom out
        {
            targetZoom += zoomSpeed * Time.deltaTime;
        }

        // Clamp target zoom
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

        // Smoothly move camera zoom toward target
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}