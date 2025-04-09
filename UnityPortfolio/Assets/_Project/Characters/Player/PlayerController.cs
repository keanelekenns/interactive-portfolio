using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    private Vector2 moveInput; // Stores the movement input

    private Rigidbody2D rb; // Rigidbody2D component for physics-based movement
    private PlayerInputActions playerActions;

    private void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        playerActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerActions.Enable();
        playerActions.Player.PointMove.performed += UpdatePlayerDestination;
    }

    private void OnDisable()
    {
        playerActions.Disable();
    }


    private void Update()
    {
        // Use the new input system to get movement input
        moveInput = playerActions.Player.Move.ReadValue<Vector2>();

        // Apply the movement based on input
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        // Move the character using the input and the rigidbody
        rb.velocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);
    }

    private void UpdatePlayerDestination(InputAction.CallbackContext context)
    {
        Vector2 destination = context.ReadValue<Vector2>();
        destination = Camera.main.ScreenToWorldPoint(destination);
        Debug.Log("destination: " + destination.x + " " + destination.y);
        transform.position = new Vector3(destination.x, destination.y, transform.position.z);
    }
}
