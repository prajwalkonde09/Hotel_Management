using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = 9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small offset to keep grounded
        }

        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        if (move.magnitude >= 0.1f) // If there's movement input
        {
            // Rotate smoothly towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move the player
            controller.Move(move * moveSpeed * Time.deltaTime);

            animator.ResetTrigger("Stop");
            animator.SetTrigger("Move");
        }
        else
        {
            // Trigger "Stop" animation
            animator.ResetTrigger("Move");
            animator.SetTrigger("Stop");
        }

        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
