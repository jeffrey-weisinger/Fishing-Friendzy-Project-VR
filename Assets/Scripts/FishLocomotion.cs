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
    [Tooltip("How close the fish needs to be to its target destination to consider it reached.")]
    public float destinationThreshold = 0.5f;

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
    [Tooltip("The center of the area where the fish can swim.")]
    public Vector3 boundaryCenter = Vector3.zero;
    [Tooltip("The radius of the spherical area where the fish can swim.")]
    public float boundaryRadius = 10.0f;
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
        Idle,      // Starting state or between actions
        Swimming,
        Pausing
    }

    // --- Unity Methods ---

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // --- Rigidbody Configuration ---
        // Ensure Rigidbody doesn't use gravity and has reasonable drag
        rb.useGravity = false;
        rb.drag = 1.0f;          // Provides some resistance to movement
        rb.angularDrag = 1.0f;   // Provides resistance to rotation

        // Optional: Freeze rotation on axes you don't want the fish to roll on
        // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Make sure the fish starts within bounds
        transform.position = GetRandomPointInBounds(boundaryCenter, boundaryRadius);

        // Start the behavior loop
        StartBehavior();
    }

    void FixedUpdate()
    {
        // FixedUpdate is best for Rigidbody physics operations

        if (currentState == FishState.Swimming)
        {
            MoveTowardsTarget();
            CheckBoundary(); // Continuously check boundaries while swimming
        }
        else if (currentState == FishState.Pausing)
        {
            // Apply damping to stop movement smoothly
            rb.velocity *= 0.9f; // Adjust multiplier for faster/slower stopping
            rb.angularVelocity *= 0.9f;
        }
    }

    // --- Behavior Control ---

    void StartBehavior()
    {
        // Stop any previous behavior coroutine if running
        if (currentBehaviorCoroutine != null)
        {
            StopCoroutine(currentBehaviorCoroutine);
        }
        // Start a new behavior cycle
        currentBehaviorCoroutine = StartCoroutine(BehaviorRoutine());
    }

    IEnumerator BehaviorRoutine()
    {
        currentState = FishState.Idle; // Start in Idle to decide next action

        while (true) // Loop indefinitely
        {
            // Decide randomly whether to swim or pause next
            bool shouldSwim = Random.Range(0, 3) > 0; // 2/3 chance to swim, 1/3 chance to pause

            if (shouldSwim)
            {
                // --- Swim Phase ---
                currentState = FishState.Swimming;
                targetPosition = GetRandomPointInBounds(boundaryCenter, boundaryRadius);
                float swimDuration = Random.Range(minSwimTime, maxSwimTime);
                float swimTimer = 0f;

                // Swim until timer runs out OR destination is reached
                while (swimTimer < swimDuration && Vector3.Distance(transform.position, targetPosition) > destinationThreshold)
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

                yield return new WaitForSeconds(pauseDuration); // Wait for the pause duration
            }

            // After swimming or pausing, transition back to Idle briefly to pick next action
            currentState = FishState.Idle;
            yield return null; // Wait one frame before deciding next action
        }
    }

    // --- Movement & Rotation ---

    void MoveTowardsTarget()
    {
        if (targetPosition == null) return;

        // Calculate direction to target
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        if (directionToTarget != Vector3.zero) // Avoid zero vector issues
        {
            // --- Rotation ---
            // Calculate desired rotation to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Smoothly rotate towards the target rotation
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);

            // --- Movement ---
            // Move the fish forward in its current facing direction
            Vector3 forwardMovement = transform.forward * moveSpeed;
            rb.velocity = forwardMovement; // Directly set velocity for simple movement
            // Alternative: Use AddForce for more physical feel
            // rb.AddForce(transform.forward * moveSpeed, ForceMode.Acceleration);
        }

        // Optional: Check if destination reached here as well (though coroutine handles timeout)
        if (Vector3.Distance(transform.position, targetPosition) <= destinationThreshold)
        {
            // Reached destination, start next behavior phase immediately
            // We can trigger the coroutine to advance by stopping and starting it,
            // or by adding a flag that the coroutine checks. Starting/Stopping is simpler here.
            StartBehavior();
        }
    }

    // --- Boundary Management ---

    void CheckBoundary()
    {
        float distanceFromCenter = Vector3.Distance(transform.position, boundaryCenter);

        if (distanceFromCenter > boundaryRadius)
        {
            // Fish is outside bounds - force it to turn back towards the center
            Vector3 directionToCenter = (boundaryCenter - transform.position).normalized;

            // Make the target position slightly inside the boundary towards the center
            targetPosition = boundaryCenter + directionToCenter * (boundaryRadius * 0.9f); // Target 90% radius towards center

            // Optionally increase turn speed temporarily for boundary avoidance
            float effectiveTurnSpeed = turnSpeed * boundaryAvoidanceStrength;

            // Rotate towards the new target (center-ish)
             if (directionToCenter != Vector3.zero)
             {
                Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
                Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, effectiveTurnSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(newRotation);
             }

            // Ensure the fish is still trying to swim
             currentState = FishState.Swimming;

            // We don't need to restart the whole behavior, just override the target
            // and let the normal swimming logic take over.
        }
    }

    Vector3 GetRandomPointInBounds(Vector3 center, float radius)
    {
        // Get a random point within a sphere
        return center + Random.insideUnitSphere * radius;
    }

    // --- Gizmos for Visualization (Optional) ---
    void OnDrawGizmosSelected()
    {
        // Draw the boundary sphere in the Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(boundaryCenter, boundaryRadius);

        // Draw the current target position
        if (currentState == FishState.Swimming)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPosition, 0.3f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}

