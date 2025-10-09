using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    private Vector2 moveInput; // Stores the movement input
    private Vector3? destination = null; // Where the player wants to be

    private Rigidbody2D rb; // Rigidbody2D component for physics-based movement
    private PlayerInputActions playerActions;
    private Animator animator;

    private void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        playerActions = new PlayerInputActions();
        animator = GetComponentInChildren<Animator>();
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


    private void FixedUpdate()
    {
        MoveCharacter();
        UpdateAnimator();
    }

    private void MoveCharacter()
    {
        // Read any manual movement inputs
        moveInput = playerActions.Player.Move.ReadValue<Vector2>();
        if (moveInput != Vector2.zero) // Manual movement takes precedence
        {
            destination = null;
            // Update the player's velocity with the manual controls
            rb.velocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);
        }
        else if (destination is Vector3 dest) // Move towards the destination
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, dest, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
            if (Vector3.Distance(transform.position, dest) < 0.05f)
            {
                // Once we arrive at our destination, we no longer have a destination
                destination = null;
            }
        }
        else // If no manual movements and no destination, stop moving
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void UpdatePlayerDestination(InputAction.CallbackContext context)
    {
        Vector2 dest = context.ReadValue<Vector2>();
        Vector3 dest3d = Camera.main.ScreenToWorldPoint(dest);
        // Debug.Log("destination: " + dest3d.x + " " + dest3d.y + " " + dest3d.z);
        destination = new Vector3(dest3d.x, dest3d.y, 0);
    }

    private void UpdateAnimator()
    {
        bool isWalkingNorth = false;
        bool isWalkingSouth = false;
        bool isIdle = false;

        if (rb.velocity.y > 0)
        {
            isWalkingNorth = true;
        }
        else if (rb.velocity.y < 0)
        {
            isWalkingSouth = true;
        }
        else
        {
            isIdle = true;
        }

        animator.SetBool("isWalkingNorth", isWalkingNorth);
        animator.SetBool("isWalkingSouth", isWalkingSouth);
        animator.SetBool("isIdle", isIdle);
    }
}
