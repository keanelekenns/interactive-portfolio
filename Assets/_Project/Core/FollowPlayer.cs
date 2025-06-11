using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;        // Reference to the player's transform
    public Vector3 offset;          // Offset from the player (so the camera isn't exactly at the player's position)
    public float smoothTime = 0.3f;   // Time it takes to smooth the camera
    private Vector3 velocity = Vector3.zero; // To store the current velocity for SmoothDamp
    private void LateUpdate()
    {
        // The desired position of the camera is the player's position + offset
        Vector3 desiredPosition = player.position + offset;

        // Smoothly move the camera towards the desired position
        // Vector3 smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothSpeed);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Set the camera position to the smoothed position
        transform.position = smoothedPosition;
    }
}
