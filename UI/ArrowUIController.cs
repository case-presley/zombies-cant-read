using UnityEngine;

/// --------------------------------------------------------
/// ArrowUIController
///
/// Points a UI arrow toward a target object in the world,
/// relative to the player's current position. Commonly used
/// for guiding players toward objectives like drop-off boxes
/// or shelves. Ignores vertical height (Y-axis).
///
/// Attach to a UI canvas element containing the arrow image.
/// --------------------------------------------------------
public class ArrowUIController : MonoBehaviour
{
    public Transform player;           // Reference to the player's transform
    public Transform target;           // The current world-space target to point to
    public RectTransform arrowUI;      // Arrow UI element

    private void Update()
    {
        // Exit early if required references are missing
        if (target != null && player != null)
        {
            // --- STEP 1: Get direction from player to target (ignoring Y axis) ---
            Vector3 playerToTarget = target.position - player.position;
            playerToTarget.y = 0; // Flatten the vector

            // --- STEP 2: Convert direction to local space (relative to player facing) ---
            Vector3 localDirection = player.InverseTransformDirection(playerToTarget);

            // --- STEP 3: Convert the 2D direction to an angle ---
            float angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

            // --- STEP 4: Rotate the UI arrow to match the angle ---
            arrowUI.rotation = Quaternion.Euler(0, 0, -angle);
        }
    }

    /// Set a new target for the arrow to point at
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
