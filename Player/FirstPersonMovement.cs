using UnityEngine;

/// --------------------------------------------------------
/// FirstPersonMovement
///
/// Basic first-person controller for walking and looking:
/// - Uses CharacterController for movement
/// - Applies mouse input to look around
/// - Gravity and movement speed handled manually
/// - Pulls speed dynamically from PlayerStats
///
/// Requires: 
/// - CharacterController on the player GameObject
/// - MainCamera as the player's view
/// --------------------------------------------------------
public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5f;                  // Default movement speed (used if PlayerStats not assigned)
    public float mouseSensitivity = 2f;       // Mouse look sensitivity

    private CharacterController characterController;
    private Vector3 velocity;                // Tracks vertical velocity (for gravity)
    private Camera playerCamera;
    private float rotationX = 0f;            // Tracks vertical camera rotation

    public PlayerStats playerStats;          // External stats (movement speed, etc.)

    private void Start()
    {
        // --- Initialize components ---
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        // --- Lock the cursor to the center of the screen ---
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // --- HANDLE MOVEMENT INPUT ---

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Direction based on player's orientation
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Prevent diagonal movement from being faster
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        // Use PlayerStats if available, fallback to default speed
        float currentSpeed = playerStats != null ? playerStats.GetMovementSpeed() : speed;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // --- HANDLE GRAVITY ---

        if (characterController.isGrounded)
        {
            velocity.y = -2f; // Keeps player grounded
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // --- HANDLE MOUSE LOOK ---

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Vertical rotation (camera only)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Horizontal rotation (body rotates)
        transform.Rotate(Vector3.up * mouseX);
    }
}
