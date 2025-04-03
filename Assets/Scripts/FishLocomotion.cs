using UnityEngine;
using System.Collections; // Required for IEnumerator

// Ensures the GameObject has a Rigidbody component
[RequireComponent(typeof(Rigidbody))]
public class FishLocomotion : MonoBehaviour
{
    // --- Public Variables (Configurable in Inspector) ---

    [Header("Movement Settings")]
    [Tooltip("Speed at which the fish swims forward.")]
    public float moveSpeed = 2.0f;
    [Tooltip("Speed at which the fish turns.")]
    public float turnSpeed = 1.0f;
    [Tooltip("How close the fish needs to be to its target destination (horizontally) to consider it reached.")]
    public float destinationThreshold = 0.5f;
    [Tooltip("The fixed Y-level the fish should swim at (e.g., the water surface).")]
    public float surfaceYLevel = 0.0f; // *** ADDED: Define the surface height ***

    [Header("Behavior Timing")]
    [Tooltip("Minimum time the fish will swim towards a destination.")]
    public float minSwimTime = 3.0f;
    [Tooltip("Maximum time the fish will swim towards a destination.")]
    public float maxSwimTime = 8.0f;
    [Tooltip("Minimum time the fish will pause.")]
    public float minPauseTime = 1.0f;
    [Tooltip("Maximum time the fish will pause.")]
    public float maxPauseTime = 4.0f;

    [Header("Swimming Boundaries")]
    [Tooltip("The center of the area where the fish can swim (Y component ignored for movement, used for gizmo).")]
    public Vector3 boundaryCenter = Vector3.zero;
    [Tooltip("The horizontal radius of the cylindrical area where the fish can swim.")]
    public float boundaryRadius = 20.0f;
    [Tooltip("How strongly the fish turns back when hitting a boundary (higher value = sharper turn).")]
    public float boundaryAvoidanceStrength = 2.0f;

    // --- Private Variables ---
    private Rigidbody rb;
    private Vector3 targetPosition;
    private FishState currentState = FishState.Idle;
    private Coroutine currentBehaviorCoroutine;

    // --- State Enum ---
    private enum FishState
    {
        Idle,       // Starting state or between actions
        Swimming,
        Pausing
    }

    // --- Unity Methods ---

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // --- Rigidbody Configuration ---
        rb.useGravity = false;
        rb.drag = 1.0f;
        rb.angularDrag = 1.0f;

        // *** MODIFIED: Freeze Y position and X/Z rotation for surface movement ***
        rb.constraints = RigidbodyConstraints.FreezePositionY |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        // Make sure the fish starts on the surface and within horizontal bounds
        Vector3 startPos = GetRandomPointOnSurface(boundaryCenter, boundaryRadius);
        // Ensure the initial Y position is correct *before* applying constraints fully take effect
        startPos.y = surfaceYLevel;
        transform.position = startPos;
        // Force Rigidbody position after setting transform position if needed
        rb.position = startPos;


        // Start the behavior loop
        StartBehavior();
    }

    void FixedUpdate()
    {
        // FixedUpdate is best for Rigidbody physics operations

        // Force the Y position every physics update to be absolutely sure.
        // This might be redundant with constraints but adds extra safety.
        Vector3 currentPos = rb.position;
        if (!Mathf.Approximately(currentPos.y, surfaceYLevel)) // Avoids tiny floating point adjustments if already correct
        {
             rb.position = new Vector3(currentPos.x, surfaceYLevel, currentPos.z);
        }


        if (currentState == FishState.Swimming)
        {
            MoveTowardsTarget();
            CheckBoundary(); // Continuously check boundaries while swimming
        }
        else if (currentState == FishState.Pausing)
        {
            // Apply damping to stop movement smoothly
            // Only damp horizontal velocity since Y is frozen
            Vector3 horizontalVelocity = rb.velocity;
            horizontalVelocity.y = 0;
            rb.velocity = horizontalVelocity * 0.9f; // Adjust multiplier for faster/slower stopping

            // Angular velocity damping (primarily Y-axis rotation now)
            rb.angularVelocity *= 0.9f;
        }
    }

    // --- Behavior Control ---

    void StartBehavior()
    {
        if (currentBehaviorCoroutine != null)
        {
            StopCoroutine(currentBehaviorCoroutine);
        }
        currentBehaviorCoroutine = StartCoroutine(BehaviorRoutine());
    }

    IEnumerator BehaviorRoutine()
    {
        currentState = FishState.Idle;

        while (true)
        {
            bool shouldSwim = Random.Range(0, 3) > 0;

            if (shouldSwim)
            {
                // --- Swim Phase ---
                currentState = FishState.Swimming;
                // *** MODIFIED: Get target on the surface plane ***
                targetPosition = GetRandomPointOnSurface(boundaryCenter, boundaryRadius);
                float swimDuration = Random.Range(minSwimTime, maxSwimTime);
                float swimTimer = 0f;

                // *** MODIFIED: Check horizontal distance ***
                while (swimTimer < swimDuration && HorizontalDistance(transform.position, targetPosition) > destinationThreshold)
                {
                    // Movement logic happens in FixedUpdate
                    swimTimer += Time.deltaTime;
                    yield return null; // Wait for the next frame
                }
            }
            else
            {
                // --- Pause Phase ---
                currentState = FishState.Pausing;
                float pauseDuration = Random.Range(minPauseTime, maxPauseTime);
                yield return new WaitForSeconds(pauseDuration);
            }

            currentState = FishState.Idle;
            yield return null;
        }
    }

    // --- Movement & Rotation ---

    void MoveTowardsTarget()
    {
        if (targetPosition == null) return;

        // Target position should already be on the correct Y plane from GetRandomPointOnSurface
        Vector3 targetPosOnPlane = new Vector3(targetPosition.x, surfaceYLevel, targetPosition.z);
        Vector3 currentPosOnPlane = new Vector3(transform.position.x, surfaceYLevel, transform.position.z);


        // Calculate direction to target *horizontally*
        Vector3 directionToTarget = (targetPosOnPlane - currentPosOnPlane).normalized;

        if (directionToTarget != Vector3.zero) // Avoid zero vector issues
        {
            // --- Rotation (Yaw only) ---
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget); // Will correctly calculate yaw rotation

            // Smoothly rotate towards the target rotation (around Y axis due to constraints)
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);

            // --- Movement (Horizontal only) ---
            // Move the fish forward in its current facing direction
            // transform.forward will be horizontal due to rotation constraints
            Vector3 forwardMovement = transform.forward * moveSpeed;
            // Ensure no accidental vertical velocity creeps in
            forwardMovement.y = 0;
            rb.velocity = forwardMovement;
        }

        // *** MODIFIED: Check horizontal distance for reaching destination ***
        if (HorizontalDistance(transform.position, targetPosition) <= destinationThreshold)
        {
            StartBehavior(); // Reached destination
        }
    }

    // --- Boundary Management ---

    void CheckBoundary()
    {
        // *** MODIFIED: Check horizontal distance from center ***
        float horizontalDistanceFromCenter = HorizontalDistance(transform.position, boundaryCenter);

        if (horizontalDistanceFromCenter > boundaryRadius)
        {
            // Fish is outside horizontal bounds - force it to turn back towards the center plane
            Vector3 centerOnPlane = new Vector3(boundaryCenter.x, surfaceYLevel, boundaryCenter.z);
            Vector3 currentPosOnPlane = new Vector3(transform.position.x, surfaceYLevel, transform.position.z);

            // *** MODIFIED: Calculate direction towards center horizontally ***
            Vector3 directionToCenter = (centerOnPlane - currentPosOnPlane).normalized;


            // Make the target position slightly inside the boundary towards the center
            // Ensure the target Y is correct
             targetPosition = centerOnPlane + directionToCenter * (boundaryRadius * 0.9f); // Target 90% radius towards center


            // Optionally increase turn speed temporarily for boundary avoidance
            float effectiveTurnSpeed = turnSpeed * boundaryAvoidanceStrength;

            // Rotate towards the new target (center-ish)
            if (directionToCenter != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
                Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, effectiveTurnSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(newRotation);
            }

            // Ensure the fish is still trying to swim towards the new valid target
            currentState = FishState.Swimming;

             // We override the target; the normal swimming logic in FixedUpdate will handle movement.
        }
    }

    // *** ADDED: Helper function for horizontal distance ***
    float HorizontalDistance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }


    // *** MODIFIED: Get random point on the horizontal plane at surface level ***
    Vector3 GetRandomPointOnSurface(Vector3 center, float radius)
    {
        // Get a random point within a 2D circle
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        // Create the 3D position on the defined surface plane
        return new Vector3(center.x + randomPoint.x, surfaceYLevel, center.z + randomPoint.y); // Use randomPoint.y for Z
    }

    // --- Gizmos for Visualization (Optional) ---
    void OnDrawGizmosSelected()
    {
        // *** MODIFIED: Draw the boundary as a wire *disk* (or cylinder top) at the surface level ***
        // Use Handles for a cleaner disk, requires UnityEditor but fine for editor visualization
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow;
        // Position the disk at the surface level, oriented horizontally
        UnityEditor.Handles.DrawWireDisc(new Vector3(boundaryCenter.x, surfaceYLevel, boundaryCenter.z), Vector3.up, boundaryRadius);
#else
        // Fallback: Draw a sphere, just remember it represents a cylinder boundary in practice
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(boundaryCenter.x, surfaceYLevel, boundaryCenter.z), boundaryRadius);
#endif

        // Draw the current target position
        if (currentState == FishState.Swimming)
        {
            Gizmos.color = Color.green;
            // Ensure target gizmo is drawn at surface level
            Gizmos.DrawSphere(new Vector3(targetPosition.x, surfaceYLevel, targetPosition.z), 0.3f);
            Gizmos.DrawLine(new Vector3(transform.position.x, surfaceYLevel, transform.position.z),
                            new Vector3(targetPosition.x, surfaceYLevel, targetPosition.z));
        }
    }
}