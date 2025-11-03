using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    private Vector2 moveInput; // Stores the movement input
    private Vector3? destination = null; // Where the player wants to be

    private Rigidbody2D rb; // Rigidbody2D component for physics-based movement
    private Animator animator;

    private void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        InputManager.InputActions.Player.Enable();
        InputManager.InputActions.Player.PointMove.performed += UpdatePlayerDestination;
    }

    private void OnDisable()
    {
        InputManager.InputActions.Player.Disable();
    }


    private void FixedUpdate()
    {
        MoveCharacter();
        UpdateAnimator();
    }

    private void MoveCharacter()
    {
        // Read any manual movement inputs
        moveInput = InputManager.InputActions.Player.Move.ReadValue<Vector2>();
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

        Vector3 movement;
        if (destination is Vector3 dest)
        {
            movement = dest - transform.position;
        }
        else
        {
            movement = rb.velocity;
        }

        float absVertical = Math.Abs(movement.y);
        float absHorizontal = Math.Abs(movement.x);

        // Determine the dominating axis if both have magnitude
        bool walkingVertical = false;
        bool walkingHorizontal = false;
        if (absHorizontal > 0 && absVertical > 0)
        {
            if (absHorizontal > absVertical)
            {
                walkingHorizontal = true;
            }
            else
            {
                walkingVertical = true;
            }
        }

        // Determine the correct direction if there is one
        bool isWalkingNorth = false;
        bool isWalkingSouth = false;
        bool isWalkingWest = false;
        bool isWalkingEast = false;
        bool isIdle = false;
        if (absVertical > 0 && !walkingHorizontal)
        {
            if (movement.y > 0)
            {
                isWalkingNorth = true;
            }
            else
            {
                isWalkingSouth = true;
            }
        }
        else if (absHorizontal > 0 && !walkingVertical)
        {
            if (movement.x > 0)
            {
                isWalkingEast = true;
            }
            else
            {
                isWalkingWest = true;
            }
        }
        else
        {
            isIdle = true;
        }

        animator.SetBool("isWalkingNorth", isWalkingNorth);
        animator.SetBool("isWalkingSouth", isWalkingSouth);
        animator.SetBool("isWalkingWest", isWalkingWest);
        animator.SetBool("isWalkingEast", isWalkingEast);
        animator.SetBool("isIdle", isIdle);
    }
}
